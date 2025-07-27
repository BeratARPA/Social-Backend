using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Social.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected string GetIp() => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        protected string GetUserAgent() => Request.Headers["User-Agent"].ToString() ?? "unknown";

    }
}
