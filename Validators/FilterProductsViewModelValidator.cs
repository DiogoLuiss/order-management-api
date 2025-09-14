using FluentValidation;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Validators
{
    public class FilterProductsViewModelValidator : AbstractValidator<FilterProductsViewModel>
    {
        public FilterProductsViewModelValidator()
        {
            Include(new ViewModelPaginationValidator());

            RuleFor(x => x.Name)
            .MaximumLength(255)
            .WithMessage("O campo 'name' deve ter no máximo 255 caracteres.");
        }
    }
}
