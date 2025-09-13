using FluentValidation;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Validators
{
    public class CreateAlterProductViewModelValidator : AbstractValidator<CreateAlterProductViewModel>
    {
        public CreateAlterProductViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O campo 'name' é obrigatório.")
                .MaximumLength(100).WithMessage("O campo 'Name' deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("O campo 'description' é obrigatório.")
                .MaximumLength(255).WithMessage("O campo 'Description' deve ter no máximo 255 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("O campo 'price' deve ser maior que zero.")
                .LessThanOrEqualTo(1000000).WithMessage("O campo 'price' não pode exceder R$ 1.000.000.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("O campo 'stockQuantity' não pode ser negativo.")
                .LessThanOrEqualTo(10000).WithMessage("O campo 'stockQuantity' deve ser no máximo 10000.");
        }
    }
}
