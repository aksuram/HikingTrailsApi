using FluentValidation;
using HikingTrailsApi.Application.Common.Helpers;

namespace HikingTrailsApi.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
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
        }
    }
}
