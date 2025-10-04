using AuthService.Commands.ForgotPassword;
using AuthService.Commands.Login;
using AuthService.Commands.Logout;
using AuthService.Commands.Register;
using AuthService.Commands.ResetPassword;
using AuthService.Commands.TokenRefresh;
using AuthService.Commands.TwoFactor;
using AuthService.Commands.Verification;
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
        [HttpPost("send-verification")]
        public async Task<IActionResult> SendVerification([FromBody] SendVerificationRequestDto request)
        {
            var command = new SendVerificationCommand(request.VerificationChannel, request.VerificationType, request.Target, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequestDto request)
        {
            var command = new VerifyCodeCommand(request.VerificationChannel, request.VerificationType, request.Target, request.Code, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var command = new ForgotPasswordCommand(request.ActionToken, request.Email, request.NewPassword, request.ConfirmPassword, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var command = new ResetPasswordCommand(GetUserId(), request.CurrentPassword, request.NewPassword, request.ConfirmPassword, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("enable-2fa")]
        public async Task<IActionResult> EnableTwoFactor([FromBody] EnableTwoFactorRequestDto request)
        {
            var command = new EnableTwoFactorCommand(GetUserId(), request.Enable, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] VerifyTwoFactorRequestDto request)
        {
            var command = new VerifyTwoFactorCommand(request.TwoFactorToken, request.VerificationCode, GetIp(), GetUserAgent());
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
