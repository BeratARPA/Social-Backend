using AuthService.Commands.ConfirmEmail;
using AuthService.Commands.Login;
using AuthService.Commands.Logout;
using AuthService.Commands.Register;
using AuthService.Commands.TokenRefresh;
using AuthService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Social.API.Controllers
{

    public class AuthController : BaseController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var command = new RegisterCommand(request.Username, request.Password, request.Email, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginCommand(request.Username, request.Password, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var command = new RefreshTokenCommand(request.RefreshToken, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            var command = new LogoutCommand(request.RefreshToken, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        } 

        [AllowAnonymous]
        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation([FromBody] SendEmailConfirmationRequestDto request)
        {
            var command = new SendEmailConfirmationCommand(request.Email, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto request)
        {
            var command = new ConfirmEmailCommand(request.Email, request.Code, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }       
    }
}
