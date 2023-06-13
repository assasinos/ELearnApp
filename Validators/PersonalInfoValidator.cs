using ELearnApp.Models;
using FluentValidation;

namespace WebApplication3.Validators;

public class PersonalInfoValidator : AbstractValidator<UserModel>
{

    public PersonalInfoValidator()
    {
        RuleFor(x=> x.full_name).NotEmpty().WithMessage("Full name is required");
        RuleFor(x=> x.email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is not valid");
        RuleFor(x => x.password).NotEmpty().WithMessage("Password is required");
    }
    
}
