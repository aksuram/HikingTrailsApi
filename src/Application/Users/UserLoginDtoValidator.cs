using FluentValidation;

namespace HikingTrailsApi.Application.Users
{
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        public UserLoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Naudotojo el. paštas negali būti tuščias")
                .MinimumLength(5)
                .WithMessage("Negalimas naudotojo el. paštas trumpesnis už 5 simbolius")
                .MaximumLength(1000)
                .WithMessage("Negalimas naudotojo el. paštas ilgesnis už 1000 simbolių")
                .EmailAddress()
                .WithMessage("Netinkamas naudotojo el. pašto formatas");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Slaptažodis negali būti tuščias");
        }
    }
}
