using FluentValidation;
using Lab3.Features.Books;

namespace Lab3.Validators;

public class UpdateBookValidator : AbstractValidator<UpdateBookRequest>
{
    public UpdateBookValidator()
    {
        RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("Id must not be empty")
            .GreaterThan(0).WithMessage("Id must be a positive integer");
        
        RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage("Title must not be empty")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters long")
            .MaximumLength(100).WithMessage("Title must be at most 100 characters long");
        
        RuleFor(x => x.Author).NotNull().NotEmpty().WithMessage("Author must not be empty")
            .MinimumLength(2).WithMessage("Author name must be at least 2 characters long");
        
        RuleFor(x => x.Year).NotNull().LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Invalid Year");
    }
}