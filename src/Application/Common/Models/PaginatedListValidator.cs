using FluentValidation;
using HikingTrailsApi.Application.Common.Interfaces;

namespace HikingTrailsApi.Application.Common.Models
{
    public class PaginatedListValidator : AbstractValidator<IPaginatedListQuery>
    {
        public PaginatedListValidator()
        {
            RuleFor(x => x.PageNumber)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Puslapio numeris negali būti žemesnis už 1");

            RuleFor(x => x.PageSize)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .WithMessage("Puslapio dydis negali būti žemesnis už 1")
                .LessThanOrEqualTo(100)
                .WithMessage("Puslapio dydis negali viršyti 100");
        }
    }
}
