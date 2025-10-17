using FluentValidation;
using FluentValidation.Validators;
using Lab3.Features;

namespace Lab3.Validators;

public class CreateBookValidator : AbstractValidator<CreateBookRequest>
{
   public CreateBookValidator()
   {
      RuleFor(x => x.Id)
         .NotEmpty().WithMessage("Id is required.")
         .GreaterThan(0).WithMessage("Id must be a positive integer.");
      
      RuleFor(x => x.Title)
         .NotEmpty().WithMessage("Title is required.")
         .MinimumLength(3).WithMessage("A valid title should have at least 3 characters.")
         .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
      
      RuleFor(x => x.Author)
         .NotEmpty().WithMessage("Author is required.")
         .MinimumLength(2).WithMessage("A valid author name should have at least 2 characters.");
      
      RuleFor(x => x.Year)
         .NotEmpty().WithMessage("Year is required.")
         .LessThanOrEqualTo(DateTime.Now.Year).WithMessage("Invalid Year.");
   }
}