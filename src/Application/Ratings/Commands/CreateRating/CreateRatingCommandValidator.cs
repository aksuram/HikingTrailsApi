using FluentValidation;

namespace HikingTrailsApi.Application.Ratings.Commands.CreateRating
{
    public class CreateRatingCommandValidator : AbstractValidator<CreateRatingCommand>
    {
        public CreateRatingCommandValidator()
        {
            RuleFor(x => x)
                .Must(x => (x.PostId == null && x.CommentId != null) || (x.PostId != null && x.CommentId == null))
                .WithMessage("Vertinimą galima priskirti arba tik komentarui arba tik įrašui");
        }
    }
}
