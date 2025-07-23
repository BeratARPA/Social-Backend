using AuthService.Commands.Login;
using AuthService.Commands.Logout;
using AuthService.Commands.Register;
using AuthService.Commands.TokenRefresh;
using AuthService.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Social.API.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
        {
            var command = new RegisterCommand(requestDto.Username, requestDto.Password, requestDto.Email);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
        {
            var command = new LoginCommand(requestDto.Username, requestDto.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto requestDto)
        {
            var command = new RefreshTokenCommand(requestDto.RefreshToken);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto requestDto)
        {
            var command = new LogoutCommand(requestDto.RefreshToken);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
