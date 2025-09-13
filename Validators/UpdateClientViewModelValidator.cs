using FluentValidation;

namespace OrderManagementApi.Validators
{
    public class UpdateClientViewModelValidator : AbstractValidator<UpdateClientViewModel>
    {
        public UpdateClientViewModelValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(255).WithMessage("O campo 'name' deve ter no máximo 255 caracteres.")
                .When(x => x.Name != null);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("O campo 'email' deve ser um e-mail válido.")
                .MaximumLength(255).WithMessage("O campo 'email' deve ter no máximo 255 caracteres.")
                .When(x => x.Email != null);

            RuleFor(x => x.Phone)
                .Matches(@"^[0-9+\-\s]*$").WithMessage("O campo 'phone' contém caracteres inválidos.")
                .MaximumLength(20).WithMessage("O campo 'phone' deve ter no máximo 20 caracteres.")
                .When(x => x.Phone != null);
        }
    }
}