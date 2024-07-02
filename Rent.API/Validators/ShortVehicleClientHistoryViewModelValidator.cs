using FluentValidation;
using Rent.API.Exceptions.ExceptionMessages;
using Rent.API.ViewModels.ShortViewModels;

namespace Rent.API.Validators;

public class ShortVehicleClientHistoryViewModelValidator : AbstractValidator<ShortVehicleClientHistoryViewModel>
{
    public ShortVehicleClientHistoryViewModelValidator()
    {
        RuleFor(vch => vch.StartDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(ExceptionMessages.StartDateGreaterThanCurrentDate);

        RuleFor(vch => vch.EndDate)
            .GreaterThanOrEqualTo(vch => vch.StartDate).WithMessage(ExceptionMessages.EndDateLessThanStartDate);

        RuleFor(vch => vch.VehicleId)
            .NotEmpty().WithMessage(ExceptionMessages.NoVehicleId);

        RuleFor(vch => vch.ClientId)
            .NotEmpty().WithMessage(ExceptionMessages.NoClientId);
    }
}