using ELearnApp.Models;
using FluentValidation;

namespace WebApplication3.Validators;

public class LessonUpdateValidator :AbstractValidator<LessonModel>
{
    public LessonUpdateValidator()
    {
        RuleFor(x => x.lesson_uid).NotEmpty().WithMessage("Lesson uid cannot be empty");
        RuleFor(x => x.course_uid).NotEmpty().WithMessage("Course uid cannot be empty");
        RuleFor(x => x.lesson_name).NotEmpty().WithMessage("Lesson name cannot be empty");
        RuleFor(x => x.lesson_content).NotEmpty().WithMessage("Lesson content cannot be empty");
        
    }
}
public class LessonCreateValidator :AbstractValidator<LessonModel>
{
    public LessonCreateValidator()
    {
        RuleFor(x => x.lesson_name).NotEmpty().WithMessage("Lesson name cannot be empty");
        RuleFor(x => x.lesson_content).NotEmpty().WithMessage("Lesson content cannot be empty");
        RuleFor(x => x.course_uid).NotEmpty().WithMessage("Course uid cannot be empty");
    }
}

public class LessonDeleteValidator :AbstractValidator<LessonModel>
{
    public LessonDeleteValidator()
    {
        RuleFor(x => x.course_uid).NotEmpty().WithMessage("Course uid cannot be empty");
        RuleFor(x => x.lesson_uid).NotEmpty().WithMessage("Lesson uid cannot be empty");
    }
}