using FluentValidation;
using movie_service.RequestDTOs;

namespace movie_service.Validators;

public class CreateMovieRequestDtoValidator : AbstractValidator<CreateMovieRequestDto>
{
    public CreateMovieRequestDtoValidator()
    {
        RuleFor(m => m.MovieName)
            .NotEmpty().WithMessage("Movie Name is Required")
            .Length(3, 255).WithMessage("Movie name must be between 3 and 255 letters.");

        RuleFor(m => m.DurationMinutes)
            .NotNull().WithMessage("Movie's Duration is Required")
            .InclusiveBetween(15, 240).WithMessage("Movie's Duration must be between 15 minutes and 4 hours");

        RuleFor(m => m.GenresIds)
            .NotEmpty().WithMessage("Movie must be related to at least 1 genre.");
    }
}
