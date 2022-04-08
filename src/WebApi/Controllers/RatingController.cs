using HikingTrailsApi.Application.Common.Models;
using HikingTrailsApi.Application.Ratings;
using HikingTrailsApi.Application.Ratings.Commands.CreateRating;
using HikingTrailsApi.Application.Ratings.Commands.DeleteRating;
using HikingTrailsApi.Application.Ratings.Commands.UpdateRating;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace HikingTrailsApi.WebApi.Controllers
{
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RatingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //TODO: Different paths for creating rating for post, comment and others?
        [Authorize]
        [HttpPost("api/rating/")]
        public async Task<ActionResult> CreateRating([FromBody] RatingCreateDto ratingCreateDto)
        {
            Result<RatingVm> result = await _mediator.Send(
                new CreateRatingCommand
                {
                    IsPositive = ratingCreateDto.IsPositive,
                    CommentId = ratingCreateDto.CommentId,
                    PostId = ratingCreateDto.PostId
                });

            return result.Type switch
            {
                //TODO: Change back to CreatedAtRoute
                //ResultType.Created => CreatedAtRoute("GetPost", new { id = result.Value.Id }, result.Value),
                ResultType.Created => Ok(result.Value),
                ResultType.BadRequest => BadRequest(result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                ResultType.Conflict => Conflict(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [Authorize]
        [HttpDelete("api/rating/{id:guid}")]
        public async Task<ActionResult> DeleteRating([FromRoute] Guid id)
        {
            Result result = await _mediator.Send(new DeleteRatingCommand { Id = id });

            return result.Type switch
            {
                ResultType.NoContent => NoContent(),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [Authorize]
        [HttpPatch("api/rating/{id:guid}")]
        public async Task<ActionResult> UpdateRating([FromRoute] Guid id, [FromBody] RatingUpdateDto ratingUpdateDto)
        {
            Result<RatingVm> result = await _mediator.Send(
                new UpdateRatingCommand
                {
                    Id = id,
                    IsPositive = ratingUpdateDto.IsPositive
                });

            return result.Type switch
            {
                ResultType.Success => Ok(result.Value),
                ResultType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, result.GetErrors()),
                ResultType.NotFound => NotFound(result.GetErrors()),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
