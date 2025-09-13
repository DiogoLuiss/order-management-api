using FluentValidation;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Validators
{
    public class FilterOrdersViewModelValidator : AbstractValidator<FilterOrdersViewModel>
    {
        public FilterOrdersViewModelValidator()
        {
            RuleFor(x => x.ClientName)
                .MaximumLength(100)
                .WithMessage("O campo 'clientName' deve ter no máximo 100 caracteres.")
                .When(x => !string.IsNullOrWhiteSpace(x.ClientName));

            RuleFor(x => x.OrderStatusId)
                .InclusiveBetween((short)1, short.MaxValue)
                .WithMessage("ID de status do pedido inválido.")
                .When(x => x.OrderStatusId.HasValue);
        }
    }
}
