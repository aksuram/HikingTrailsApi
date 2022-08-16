using FluentValidation;

namespace HikingTrailsApi.Application.Comments.Commands.CreateComment
{
    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(x => x.Body)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Komentaras negali būti tuščias")
                .MinimumLength(1)
                .WithMessage("Komentaras per trumpas")
                .MaximumLength(1000)
                .WithMessage("Komentaras per ilgas");
        }
    }
}
