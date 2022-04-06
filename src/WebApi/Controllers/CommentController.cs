using HikingTrailsApi.Application.Comments;
using HikingTrailsApi.Application.Comments.Commands.CreateComment;
using HikingTrailsApi.Application.Comments.Queries.GetComment;
using HikingTrailsApi.Application.Comments.Queries.GetComments;
using HikingTrailsApi.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("api/post/{postId:guid}/comment/")]
        public async Task<ActionResult> CreateComment([FromRoute] Guid postId, [FromBody] CommentCreateDto commentCreateDto)
        {
            Result<CommentVm> result = await _mediator.Send(
                new CreateCommentCommand
                {
                    PostId = postId,
                    Body = commentCreateDto.Body,
                    ReplyToId = commentCreateDto.ReplyToId
                });

            return result.Type switch
            {
                //TODO: Change back after GetComment is created
                //ResultType.Created => CreatedAtRoute("GetComment", new { id = result.Value.Id }, result.Value),
                ResultType.Created => Ok(result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                ResultType.Conflict => Conflict(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpGet("api/comment/{id:guid}", Name = "GetComment")]
        public async Task<ActionResult> GetComment([FromRoute] Guid id)
        {
            Result<CommentVm> result = await _mediator.Send(new GetCommentQuery { Id = id });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpGet("api/post/{postId:guid}/comment/")]
        public async Task<ActionResult> GetCommentList([FromRoute] Guid postId)
        {
            Result<FormattedCommentListVm> result = await _mediator.Send(new GetCommentsQuery{ PostId = postId });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
