using FluentValidation;

namespace HikingTrailsApi.Application.Posts.Commands.CreatePost
{
    public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            RuleFor(x => x.Title)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Įrašo pavadinimas negali būti tuščias")
                .MinimumLength(1)
                .WithMessage("Įrašo pavadinimas per trumpas")
                .MaximumLength(200)
                .WithMessage("Įrašo pavadinimas per ilgas");

            RuleFor(x => x.Body)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Įrašas negali būti tuščias")
                .MinimumLength(1)
                .WithMessage("Įrašas per trumpas")
                .MaximumLength(5000)
                .WithMessage("Įrašas per ilgas");
        }
    }
}
