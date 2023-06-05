using FluentValidation;
using WebApplication3.Models;

namespace WebApplication3.Validators;

public class InstructorEditValidator : AbstractValidator<UserModel>
{

    public InstructorEditValidator()
    {
        RuleFor(x => x.full_name).NotEmpty().WithMessage("Full Name is required");
        RuleFor(x => x.email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.bio).NotEmpty().WithMessage("BIO is required");
        RuleFor(x => x.user_uid).NotEmpty().WithMessage("User UID is required");
        RuleFor(x => x.imgsrc).NotEmpty().WithMessage("Image Source is required");
    }
    
}

public class InstructorRemoveValidator : AbstractValidator<UserModel>
{

    public InstructorRemoveValidator()
    {
        RuleFor(x => x.user_uid).NotEmpty().WithMessage("User UID is required");
    }
    
}