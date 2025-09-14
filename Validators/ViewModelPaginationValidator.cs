using FluentValidation;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Validators
{
    public class ViewModelPaginationValidator : AbstractValidator<ViewModelPagination>
    {
        public ViewModelPaginationValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("O campo 'page' deve ser maior ou igual a 1.");
        }
    }
}
