using FluentValidation;
using movie_service.RequestDTOs;

namespace movie_service.Validators;

public class CreateShowtimeRequestDtoValidator : AbstractValidator<CreateShowtimeRequestDto>
{
    public CreateShowtimeRequestDtoValidator()
    {
        RuleFor(st => st.StartTime)
            .NotNull().WithMessage("Showtime's Start Time is Required");

        RuleFor(st => st.EndTime)
            .NotNull().WithMessage("Showtime's End Time is Required");

        RuleFor(st => st.Price)
            .NotNull().WithMessage("Showtime Ticket's Price is Required");
    }
}
