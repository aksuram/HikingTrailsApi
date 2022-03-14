using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace HikingTrailsApi.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class TestController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("api/test")]
        public ActionResult Test()
        {
            return Ok("Užklausa gauta");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/test2")]
        public ActionResult Test2()
        {
            return Ok("Užklausa gauta2");
        }
    }
}
