using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Netptune.Entities.Authentication;
using Netptune.Services.Authentication.Interfaces;

namespace Netptune.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly INetptuneAuthService _authenticationService;

        public AuthController(INetptuneAuthService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json", Type = typeof(AuthenticationTicket))]
        public async Task<IActionResult> Login([FromBody] TokenRequest model)
        {
            var result = await _authenticationService.LogIn(model);

            return result.ToRestResult();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json", Type = typeof(AuthenticationTicket))]
        public async Task<IActionResult> Register([FromBody] TokenRequest model)
        {
            var result = await _authenticationService.Register(model);

            return result.ToRestResult();
        }
    }
}