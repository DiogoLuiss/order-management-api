using FluentValidation;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Validators
{
    public class CreateClientViewModelValidator : AbstractValidator<CreateClientViewModel>
    {
        public CreateClientViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O campo 'name' é obrigatório.")
                .MaximumLength(255).WithMessage("O campo 'name' deve ter no máximo 255 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O campo 'email' é obrigatório.")
                .EmailAddress().WithMessage("O campo 'email' deve ser um e-mail válido.")
                .MaximumLength(255).WithMessage("O campo 'email' deve ter no máximo 255 caracteres.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("O campo 'phone' é obrigatório.")
                .Matches(@"^[0-9+\-\s]*$").WithMessage("O campo 'phone' contém caracteres inválidos.")
                .MaximumLength(20).WithMessage("O campo 'phone' deve ter no máximo 20 caracteres.");
        }
    }
}
