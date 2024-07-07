using FluentValidation;

public class RootDtoValidator : AbstractValidator<RootDto>
{
    public RootDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
