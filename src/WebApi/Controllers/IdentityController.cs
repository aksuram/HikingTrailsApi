using HikingTrailsApi.Application.Common.Interfaces;
using HikingTrailsApi.Application.Models;
using HikingTrailsApi.Application.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IApplicationIdentityManager _applicationIdentityManager;

        public IdentityController(IApplicationIdentityManager applicationIdentityManager)
        {
            _applicationIdentityManager = applicationIdentityManager;
        }

        [HttpPost("api/login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            Result<UserLoginVm> result = await _applicationIdentityManager.LogIn(userLoginDto);

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                ResultType.Unauthorized => Unauthorized(result.GetErrors()),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPost("api/register")]
        public async Task<ActionResult> Register([FromBody] UserRegistrationDto userRegistrationDto)
        {
            Result result = await _applicationIdentityManager.RegisterUser(userRegistrationDto);

            return result.Type switch
            {
                ResultType.Success => Ok(),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
