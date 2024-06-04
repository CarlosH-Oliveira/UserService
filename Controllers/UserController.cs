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

        /// <summary>
        /// Obtém uma lista de usuários
        /// </summary>
        /// <param name="skip">Determina quantos registros serão "pulados" até o ponto inicial de onde será buscada a lista de usuários</param>
        /// <param name="take">Determina o limite de quantos registros serão retornados</param>
        /// <returns>Retorna um intervalo de usuários determinado pelos parâmetros Skip e Take em formato de lista</returns>
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

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        /// <param name="dto">Este objeto / corpo contém as informações necessárias para a criação de uma nova conta</param>
        /// <returns>Retorna o usuário recém registrado</returns>
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

        /// <summary>
        /// Realiza o login de um usuário
        /// </summary>
        /// <param name="dto">Este objeto / corpo contém as informações necessárias para inicializar a sessão de um usuário</param>
        /// <returns>Retorna os dados do usuário com seu token de acesso</returns>
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

        /// <summary>
        /// Permite um usuário conectado alterar sua senha
        /// </summary>
        /// <param name="changePasswordDTO">Este objeto / corpo contém as informações necessárias para troca de senha</param>
        /// <returns>Retorna uma mensagem de sucesso e envia uma e-mail de confirmação para o usuário</returns>
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

        /// <summary>
        /// Permite que um usuário recupere uma conta da qual tenha esquecido a senha
        /// </summary>
        /// <param name="recoverPasswordDTO">Este objeto / corpo contém as informações necessárias para recuperação de senha</param>
        /// <returns>Retorna uma mensagem de sucesso e envia uma e-mail de confirmação para o usuário</returns>
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

        /// <summary>
        /// Recebe e executa um pedido de alteração de senha
        /// </summary>
        /// <param name="tk">Token de alteração de senha</param>
        /// <returns>Retorna uma mensagem de sucesso</returns>
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

        /// <summary>
        /// Permite um usuário conectado alterar o e-mail em que sua conta está registrada
        /// </summary>
        /// <param name="changeEmailDTO">Este objeto / corpo contém as informações necessárias para troca de e-mail</param>
        /// <param name="tk">Token de acesso gerado através do Login</param>
        /// <returns>Retorna uma mensagem de sucesso e envia uma e-mail de confirmação para o novo endereço escolhido</returns>
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

        /// <summary>
        /// Recebe e executa um pedido de alteração de e-mail
        /// </summary>
        /// <param name="tk">Token de alteração de e-mail</param>
        /// <returns>Retorna uma mensagem de sucesso</returns>
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