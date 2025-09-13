using FluentValidation;
using OrderManagementApi.ViewModel;

public class FilterClientsViewModelValidator : AbstractValidator<FilterClientsViewModel>
{
    public FilterClientsViewModelValidator()
    {
        RuleFor(x => x.NameOrEmail)
            .MaximumLength(255)
            .WithMessage("O campo 'nameOrEmail' deve ter no máximo 255 caracteres.");

        RuleFor(x => x.NameOrEmail)
            .Matches(@"^[a-zA-Z0-9@.\s]*$")
            .WithMessage("O campo 'nameOrEmail' contém caracteres inválidos.")
            .When(x => !string.IsNullOrWhiteSpace(x.NameOrEmail));
    }

}
