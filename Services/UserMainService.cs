using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using UserService.Data;
using UserService.Models.User;
using UserService.Models.User.DTOS;
using UserService.Models.UserMainService;

namespace UserService.Services
{
    public class UserMainService
    {
        private IMapper _mapper;

        private UserContext _userContext;

        private EmailService _emailService;

        private PasswordHandler _passwordHandler;

        private TokenHandler _tokenHandler;

        public UserMainService(IMapper mapper, UserContext userContext, EmailService emailService, PasswordHandler passwordHandler, TokenHandler tokenHandler)
        {
            _mapper = mapper;

            _userContext = userContext;

            _emailService = emailService;

            _passwordHandler = passwordHandler;

            _tokenHandler = tokenHandler;

        }

        public GetUsersResult GetUsers (int skip, int take)
        {
            GetUsersResult result = new GetUsersResult();
            try
            {
                List<ReadUserDTO> users = _mapper.Map<List<ReadUserDTO>>(_userContext.Users.Skip(skip).Take(take).ToList());

                result.ReadUsers = users;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public CreateUserResult CreateUser (CreateUserDTO dto)
        {
            CreateUserResult result = new CreateUserResult();

            try
            {
                User? user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == dto.Email);

                if (user != null)
                {
                    result.Error = true;

                    result.StatusCode = 404;

                    result.Message = "User already exists";
                }

                if (_emailService.Validate(dto.Email))
                {
                    var isStrongPassword = _passwordHandler.Validate(dto.Password);

                    if (isStrongPassword)
                    {
                        try
                        {
                            _emailService.SendEmail(dto.Email, "UserService SignIn", "SignIn Confirmed by UserService");
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                result.Error = true;

                                result.StatusCode = 500;

                                result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                            }
                            else
                            {
                                result.Error = true;

                                result.StatusCode = 500;

                                result.Message = ex.Message;
                            }
                        }

                        User newUser = _mapper.Map<User>(dto);

                        var now = DateTime.UtcNow;

                        newUser.CreatedAt = now;

                        newUser.Password = _passwordHandler.Encode(newUser.Password);

                        _userContext.Users.Add(newUser);

                        _userContext.SaveChanges();

                        ReadUserDTO readUserDTO = _mapper.Map<ReadUserDTO>(newUser);

                        readUserDTO.AccessToken = _tokenHandler.GenerateLoginToken(readUserDTO.Email);

                        result.ReadUser = readUserDTO;
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 400;

                        result.Message = "Invalid password";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "Invalid e-mail";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public LoginResult Login(LoginDTO dto)
        {
            LoginResult result = new LoginResult();

            try
            {
                dto.Password = _passwordHandler.Encode(dto.Password);

                User? user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == dto.Email && obj.Password == dto.Password);

                if (user == null)
                {
                    result.Error = true;

                    result.StatusCode = 404;

                    result.Message = "User not found";
                }

                user.LastLogin = DateTime.UtcNow;

                _userContext.SaveChanges();

                ReadUserDTO readUserDTO = _mapper.Map<ReadUserDTO>(user);

                readUserDTO.AccessToken = _tokenHandler.GenerateLoginToken(readUserDTO.Email);

                result.ReadUser = readUserDTO;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public UpdatePasswordResult UpdatePassword (ChangePasswordDTO changePasswordDTO)
        {
            UpdatePasswordResult result = new UpdatePasswordResult();

            try
            {
                User? user = null;

                Claim? email = null;

                if (_tokenHandler.VerifyExpirationTime(changePasswordDTO.AccessToken))
                {
                    var tokenClaims = _tokenHandler.ReadToken(changePasswordDTO.AccessToken);

                    email = tokenClaims.FirstOrDefault(claim => claim.Type == "email");

                    if (email != null && email.Value != null)
                    {
                        if (_emailService.Validate(email.Value))
                        {
                            user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == email.Value);
                        }
                        else
                        {
                            result.Error = true;

                            result.StatusCode = 400;

                            result.Message = "Invalid email provided. Please, sign in again";
                        }
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 400;

                        result.Message = "Invalid access token information. Please, sign in again";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "Expired access token. Please, sign in again";
                }

                if (_passwordHandler.Validate(changePasswordDTO.NewPassword) && _emailService.Validate(email.Value))
                {
                    if (user != null)
                    {
                        user.AuxPassword = _passwordHandler.Encode(changePasswordDTO.NewPassword);

                        _userContext.SaveChanges();
                        try
                        {
                            _emailService.SendEmail(user.Email, "Password Change", "You requested a password change. Please, click on the link to finish the proccess\nhttps://localhost:7053/User/Update/Password/Execute?tk=" + _tokenHandler.GeneratePasswordChangeToken(user.Email));
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                result.Error = true;

                                result.StatusCode = 500;

                                result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                            }
                            else
                            {
                                result.Error = true;

                                result.StatusCode = 500;

                                result.Message = ex.Message;
                            }
                        }
                        result.SuccessMessage = "An e-mail was sent to you to complete the process.";
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 404;

                        result.Message = "User not found";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "The email or password provided is invalid";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public RecoverPasswordResult RecoverPassword(RecoverPasswordDTO recoverPasswordDTO)
        {
            RecoverPasswordResult result = new RecoverPasswordResult();
            try
            {
                if (_passwordHandler.Validate(recoverPasswordDTO.NewPassword) && _emailService.Validate(recoverPasswordDTO.Email))
                {
                    User? user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == recoverPasswordDTO.Email);

                    if (user != null)
                    {
                        user.AuxPassword = _passwordHandler.Encode(recoverPasswordDTO.NewPassword);

                        _userContext.SaveChanges();
                        try
                        {
                            _emailService.SendEmail(user.Email, "Password Change", "You requested a password change. Please, click on the link to finish the proccess\nhttps://localhost:7053/User/Update/Password/Execute?tk=" + _tokenHandler.GeneratePasswordChangeToken(user.Email));
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                result.Error = true;

                                result.StatusCode = 500;

                                result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                            }
                            else
                            {
                                result.Error = true;

                                result.StatusCode = 500;

                                result.Message = ex.Message;
                            }
                        }
                        result.SuccessMessage = "An e-mail was sent to you to complete the process.";
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 404;

                        result.Message = "User not found";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "The email or password provided is invalid";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public ExecutePasswordUpdateResult ExecutePasswordUpdate (string tk)
        {
            ExecutePasswordUpdateResult result = new ExecutePasswordUpdateResult ();

            Claim? email = null;

            try
            {
                if (_tokenHandler.VerifyExpirationTime(tk))
                {
                    var tokenClaims = _tokenHandler.ReadToken(tk);

                    email = tokenClaims.FirstOrDefault(claim => claim.Type == "email");

                    if (email != null && email.Value != null)
                    {
                        User? user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == email.Value);

                        if (user == null)
                        {
                            result.Error = true;

                            result.StatusCode = 404;

                            result.Message = "User not found";
                        }
                        else
                        {
                            if (user.AuxPassword != null)
                            {
                                user.Password = user.AuxPassword;

                                user.AuxPassword = null;

                                _userContext.SaveChanges();

                                result.SuccessMessage = "Password has been changed successfully";
                            }
                            else
                            {
                                result.Error = true;

                                result.StatusCode = 400;

                                result.Message = "No password change request was found";
                            }
                        }
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 400;

                        result.Message = "Email is invalid or was not informed";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "Invalid token";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public UpdateEmailResult UpdateEmail (ChangeEmailDTO changeEmailDTO, string tk)
        {
            UpdateEmailResult result = new UpdateEmailResult();

            try
            {
                User? user = null;

                if (_tokenHandler.VerifyExpirationTime(tk))
                {
                    var tokenClaims = _tokenHandler.ReadToken(tk);

                    var email = tokenClaims.FirstOrDefault(claim => claim.Type == "email");

                    if (email != null && email.Value != null)
                    {
                        if (_emailService.Validate(email.Value))
                        {
                            user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == email.Value);
                        }
                        else
                        {
                            result.Error = true;

                            result.StatusCode = 400;

                            result.Message = "Invalid email provided. Please, sign in again";
                        }
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 400;

                        result.Message = "Invalid access token information. Please, sign in again";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "Expired access token. Please, sign in again";
                }

                if (_emailService.Validate(changeEmailDTO.NewEmail) && user != null)
                {
                    User? alreadyExistsUser = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == changeEmailDTO.NewEmail);

                    if (alreadyExistsUser != null)
                    {
                        result.Error = true;

                        result.StatusCode = 400;

                        result.Message = "The new email informed already has a registered account";
                    }

                    try
                    {
                        _emailService.SendEmail(changeEmailDTO.NewEmail, "UserService Email Change", "You've requested a change on your account's e-mail. Please, click on the link bellow to finish the proccess:\nhttps://localhost:7053/User/Update/Email/Execute?tk=" + _tokenHandler.GenerateEmailChangeToken(user.Email, changeEmailDTO.NewEmail));
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            result.Error = true;

                            result.StatusCode = 500;

                            result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                        }
                        else
                        {
                            result.Error = true;

                            result.StatusCode = 500;

                            result.Message = ex.Message;
                        }
                    }
                    result.SuccessMessage = "An e-mail was sent to you to complete the process.";
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "Invalid Email. The new e-mail informed may have a account registered already.";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }

        public ExecuteEmailUpdateResult ExecuteEmailUpdate (string tk)
        {
            ExecuteEmailUpdateResult result = new ExecuteEmailUpdateResult ();

            try
            {
                if (_tokenHandler.VerifyExpirationTime(tk))
                {
                    var tokenClaims = _tokenHandler.ReadToken(tk);

                    var email = tokenClaims.FirstOrDefault(claim => claim.Type == "email");

                    var newEmail = tokenClaims.FirstOrDefault(claim => claim.Type == "newEmail");

                    if ((email != null && newEmail != null) && (email.Value != null && newEmail.Value != null))
                    {
                        User? user = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == email.Value);

                        if (user != null)
                        {
                            User? alreadyExistsUser = _userContext.Users.FirstOrDefault<User>(obj => obj.Email == newEmail.Value);

                            if (alreadyExistsUser != null)
                            {
                                result.Error = true;

                                result.StatusCode = 400;

                                result.Message = "The new email informed already has a registered account";
                            }

                            try
                            {
                                user.Email = newEmail.Value;

                                _userContext.SaveChanges();

                                result.SuccessMessage = "Email has been changed successfully";
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null)
                                {
                                    result.Error = true;

                                    result.StatusCode = 500;

                                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                                }
                                else
                                {
                                    result.Error = true;

                                    result.StatusCode = 500;

                                    result.Message = ex.Message;
                                }
                            }
                        }
                        else
                        {
                            result.Error = true;

                            result.StatusCode = 400;

                            result.Message = "No valid user was found for the e-mail change";
                        }
                    }
                    else
                    {
                        result.Error = true;

                        result.StatusCode = 400;

                        result.Message = "The information given is incomplete";
                    }
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 400;

                    result.Message = "Invalid token";
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message + "\nInner Exception:\n" + ex.InnerException.Message;
                }
                else
                {
                    result.Error = true;

                    result.StatusCode = 500;

                    result.Message = ex.Message;
                }
            }

            return result;
        }
    }
}
