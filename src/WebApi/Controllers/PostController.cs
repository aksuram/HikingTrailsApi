using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Posts;
using HikingTrailsApi.Application.Posts.Commands.CreatePost;
using HikingTrailsApi.Application.Posts.Commands.DeletePost;
using HikingTrailsApi.Application.Posts.Commands.UpdatePost;
using HikingTrailsApi.Application.Posts.Queries.GetPost;
using HikingTrailsApi.Application.Posts.Queries.GetPosts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("api/post/")]
        public async Task<ActionResult> CreatePost([FromBody] PostCreateDto postCreateDto)
        {
            Result<PostVm> result = await _mediator.Send(
                new CreatePostCommand
                {
                    Title = postCreateDto.Title,
                    Body = postCreateDto.Body
                });

            return result.Type switch
            {
                ResultType.Created => CreatedAtRoute("GetPost", new { id = result.Value.Id }, result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpGet("api/post/{id:guid}", Name = "GetPost")]
        public async Task<ActionResult> GetPost([FromRoute] Guid id)
        {
            Result<PostVm> result = await _mediator.Send(new GetPostQuery { Id = id });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpGet("api/post/")]
        public async Task<ActionResult> GetPostList([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
        {
            Result<PaginatedList<PostWithUserRatingVm>> result = await _mediator.Send(
                new GetPostsQuery
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

        [Authorize]
        [HttpDelete("api/post/{id:guid}")]
        public async Task<ActionResult> DeletePost([FromRoute] Guid id)
        {
            Result result = await _mediator.Send(new DeletePostCommand { Id = id });

            return result.Type switch
            {
                ResultType.NoContent => NoContent(),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [Authorize]
        [HttpPatch("api/post/{id:guid}")]
        public async Task<ActionResult> UpdatePost([FromRoute] Guid id, [FromBody] PostUpdateDto postUpdateDto)
        {
            Result<PostVm> result = await _mediator.Send(
                new UpdatePostCommand
                {
                    Id = id,
                    Title = postUpdateDto.Title,
                    Body = postUpdateDto.Body
                });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                ResultType.Conflict => Conflict(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
