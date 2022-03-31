using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IApplicationIdentityManager _applicationIdentityManager;

        public IdentityController(IApplicationIdentityManager applicationIdentityManager)
        {
            _applicationIdentityManager = applicationIdentityManager;
        }

        [AllowAnonymous]
        [HttpPost("api/login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var result = await _applicationIdentityManager.LogIn(userLoginDto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "User")]
        //[AllowAnonymous]
        [HttpPost("api/register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            var result = await _applicationIdentityManager.RegisterUser(userRegistrationDto);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}
