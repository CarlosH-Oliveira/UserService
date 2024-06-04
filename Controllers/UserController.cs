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
        /// Obt�m uma lista de usu�rios
        /// </summary>
        /// <param name="skip">Determina quantos registros ser�o "pulados" at� o ponto inicial de onde ser� buscada a lista de usu�rios</param>
        /// <param name="take">Determina o limite de quantos registros ser�o retornados</param>
        /// <returns>Retorna um intervalo de usu�rios determinado pelos par�metros Skip e Take em formato de lista</returns>
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
        /// Registra um novo usu�rio
        /// </summary>
        /// <param name="dto">Este objeto / corpo cont�m as informa��es necess�rias para a cria��o de uma nova conta</param>
        /// <returns>Retorna o usu�rio rec�m registrado</returns>
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
        /// Realiza o login de um usu�rio
        /// </summary>
        /// <param name="dto">Este objeto / corpo cont�m as informa��es necess�rias para inicializar a sess�o de um usu�rio</param>
        /// <returns>Retorna os dados do usu�rio com seu token de acesso</returns>
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
        /// Permite um usu�rio conectado alterar sua senha
        /// </summary>
        /// <param name="changePasswordDTO">Este objeto / corpo cont�m as informa��es necess�rias para troca de senha</param>
        /// <returns>Retorna uma mensagem de sucesso e envia uma e-mail de confirma��o para o usu�rio</returns>
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
        /// Permite que um usu�rio recupere uma conta da qual tenha esquecido a senha
        /// </summary>
        /// <param name="recoverPasswordDTO">Este objeto / corpo cont�m as informa��es necess�rias para recupera��o de senha</param>
        /// <returns>Retorna uma mensagem de sucesso e envia uma e-mail de confirma��o para o usu�rio</returns>
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
        /// Recebe e executa um pedido de altera��o de senha
        /// </summary>
        /// <param name="tk">Token de altera��o de senha</param>
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
        /// Permite um usu�rio conectado alterar o e-mail em que sua conta est� registrada
        /// </summary>
        /// <param name="changeEmailDTO">Este objeto / corpo cont�m as informa��es necess�rias para troca de e-mail</param>
        /// <param name="tk">Token de acesso gerado atrav�s do Login</param>
        /// <returns>Retorna uma mensagem de sucesso e envia uma e-mail de confirma��o para o novo endere�o escolhido</returns>
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
        /// Recebe e executa um pedido de altera��o de e-mail
        /// </summary>
        /// <param name="tk">Token de altera��o de e-mail</param>
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