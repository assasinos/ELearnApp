using FluentValidation;
using WebApplication3.Models;

namespace WebApplication3.Validators;

public class CourseValidator : AbstractValidator<CourseModel>
{
    public CourseValidator()
    {
        RuleFor(x => x.course_uid).NotEmpty().WithMessage("Course uid cannot be empty");
        RuleFor(x => x.title).NotEmpty().WithMessage("Course title cannot be empty");
        RuleFor(x => x.description).NotEmpty().WithMessage("Course description cannot be empty");
        RuleFor(x => x.imgsrc).NotEmpty().WithMessage("Course image source cannot be empty");
        RuleFor(x => x.overview).NotEmpty().WithMessage("Course overview cannot be empty");
        RuleFor(x => x.Instructor.user_uid).NotEmpty().WithMessage("Course instructor cannot be empty");
        
    }
}

public class CourseCreationValidator : AbstractValidator<CourseModel>
{
    public CourseCreationValidator()
    {
        RuleFor(x => x.title).NotEmpty().WithMessage("Course title cannot be empty");
        RuleFor(x => x.description).NotEmpty().WithMessage("Course description cannot be empty");
        RuleFor(x => x.imgsrc).NotEmpty().WithMessage("Course image source cannot be empty");
        RuleFor(x => x.overview).NotEmpty().WithMessage("Course overview cannot be empty");
        RuleFor(x => x.Instructor.user_uid).NotEmpty().WithMessage("Course instructor cannot be empty");
        
    }
}