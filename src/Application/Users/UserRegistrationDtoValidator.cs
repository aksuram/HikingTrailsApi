using FluentValidation;
using HikingTrailsApi.Application.Common.Helpers;
using HikingTrailsApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Users
{
    public class UserRegistrationDtoValidator : AbstractValidator<UserRegistrationDto>
    {
        public UserRegistrationDtoValidator(IApplicationDbContext applicationDbContext)
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
                .WithMessage("Netinkamas naudotojo el. pašto formatas")
                .MustAsync((email, cancellationToken) => UniqueEmail(email, cancellationToken, applicationDbContext))
                .WithMessage("Jau egzistuoja naudotojas su tokiu naudotojo el. paštu");

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Vardas negali būti tuščias")
                .Matches(ValidationHelper.CapitalizedAlphaRegex)
                .WithMessage("Vardas prasideda didžiąja raide, varde galimos tik raidės");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Pavardė negali būti tuščia")
                .Matches(ValidationHelper.CapitalizedAlphaRegex)
                .WithMessage("Pavardė prasideda didžiąja raide, pavardėje galimos tik raidės");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Slaptažodis negali būti tuščias")
                .MinimumLength(8)
                .WithMessage("Slaptažodis negali būti trumpesnis už 8 simbolius")
                .Must(ValidPassword)
                .WithMessage("Slaptažodis privalo turėti bent po vieną skaičių, didžiąją ir mažąją raides");

            RuleFor(x => x.RepeatPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Pakartotinas slaptažodis negali būti tuščias")
                .Equal(x => x.Password)
                .WithMessage("Slaptažodžiai turi sutapti");
        }

        private static async Task<bool> UniqueEmail(string email,
            CancellationToken cancellationToken, IApplicationDbContext applicationDbContext)
        {
            return await applicationDbContext.Users
                .AllAsync(x => x.Email != email, cancellationToken);
        }

        private static bool ValidPassword(string password)
        {
            //TODO: darasyti lt raides, sudeti regexus i lista ir pereiti ir matchinti per lista
            var upper = new Regex("[A-Z]");
            var lower = new Regex("[a-z]");
            var number = new Regex("[0-9]");

            //TODO: Check validity

            return (upper.IsMatch(password) && lower.IsMatch(password) && number.IsMatch(password));
        }
    }
}
