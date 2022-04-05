using HikingTrailsApi.Application.Models;
using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("api/user/{id:guid}")]
        public async Task<ActionResult> GetUser([FromRoute] Guid id)
        {
            Result<UserVm> result = await _mediator.Send(new GetUserQuery { Id = id });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
