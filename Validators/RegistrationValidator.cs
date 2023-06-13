using ELearnApp.Models;
using FluentValidation;

namespace WebApplication3.Validators;

public class RegistrationValidator : AbstractValidator<RegisterModel>
{
    public RegistrationValidator()
    {
        RuleFor(x => x.password).NotEmpty().WithMessage("Password is required");
        RuleFor(x => x.firstname).NotEmpty().WithMessage("First Name is required");
        RuleFor(x => x.surname).NotEmpty().WithMessage("Surname is required");
        RuleFor(x => x.username).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.email).NotEmpty().WithMessage("E-Mail is required").EmailAddress().WithMessage("E-Mail is not valid");

    }    
}