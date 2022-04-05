using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Users;
using HikingTrailsApi.Application.Users.Commands.DeleteUser;
using HikingTrailsApi.Application.Users.Commands.UpdateUser;
using HikingTrailsApi.Application.Users.Queries.GetUser;
using HikingTrailsApi.Application.Users.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("api/user/{id:guid}", Name = "GetUser")]
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

        [HttpGet("api/user/")]
        public async Task<ActionResult> GetUserList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            Result<PaginatedList<UserVm>> result = await _mediator.Send(
                new GetUsersQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("api/user/{id:guid}")]
        public async Task<ActionResult> DeleteUser([FromRoute] Guid id)
        {
            Result result = await _mediator.Send(new DeleteUserCommand { Id = id });

            return result.Type switch
            {
                ResultType.Success => NoContent(),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [Authorize]
        [HttpPatch("api/user/{id:guid}")]
        public async Task<ActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateDto userUpdateDto)
        {
            Result<UserVm> result = await _mediator.Send(
                new UpdateUserCommand
                {
                    Id = id,
                    FirstName = userUpdateDto.FirstName,
                    LastName = userUpdateDto.LastName
                });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
