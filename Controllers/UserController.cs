using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UserService.Data;
using UserService.Models.User;
using UserService.Models.User.DTOS;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        UserMainService _userMainService;

        public UserController(UserMainService userMainService)
        {
            _userMainService = userMainService;
        }

        [HttpGet("GetList")]
        public IActionResult GetList([FromQuery] int skip = 0, [FromQuery] int take = 25)
        {
            var response = _userMainService.GetUsers(skip, take);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.ReadUsers);
            }
            
        }

        [HttpPost("Create")]
        public IActionResult CreateUser(CreateUserDTO dto)
        {
            var response = _userMainService.CreateUser(dto);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.ReadUser);
            }
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDTO dto)
        {
            var response = _userMainService.Login(dto);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.ReadUser);
            }
        }

        [HttpPost("Update/Password")]
        public IActionResult ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var response = _userMainService.UpdatePassword(changePasswordDTO);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.SuccessMessage);
            }
        }

        [HttpPost("Update/Recover/Password")]
        public IActionResult RecoverPassword(RecoverPasswordDTO recoverPasswordDTO)
        {
            var response = _userMainService.RecoverPassword(recoverPasswordDTO);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.SuccessMessage);
            }
        }

        [HttpGet("Update/Password/Execute")]
        public IActionResult ExecutePasswordChange([FromQuery]string tk)
        {
            var response = _userMainService.ExecutePasswordUpdate(tk);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.SuccessMessage);
            }
        }

        [HttpPost("Update/Email")]
        public IActionResult ChangeEmail(ChangeEmailDTO changeEmailDTO, [FromQuery] string tk)
        {
            var response = _userMainService.UpdateEmail(changeEmailDTO, tk);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.SuccessMessage);
            }
        }

        [HttpGet("Update/Email/Execute")]
        public IActionResult ExecuteEmailChange([FromQuery] string tk)
        {
            var response = _userMainService.ExecuteEmailUpdate(tk);

            if (response.Error == true)
            {
                return StatusCode(response.StatusCode, response.Message);
            }
            else
            {
                return Ok(response.SuccessMessage);
            }
        }
    }
}