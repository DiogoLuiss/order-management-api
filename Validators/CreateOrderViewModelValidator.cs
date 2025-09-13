using FluentValidation;
using OrderManagementApi.ViewModel;

namespace OrderManagementApi.Validators
{
    public class CreateOrderViewModelValidator : AbstractValidator<CreateOrderViewModel>
    {
        public CreateOrderViewModelValidator()
        {
            RuleFor(x => x.ClientId)
                .GreaterThan(0)
                .WithMessage("O campo 'clientId' deve ser maior que zero.");

            RuleFor(x => x.Items)
                .NotEmpty()
                .WithMessage("O pedido deve conter pelo menos um item.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId)
                    .GreaterThan(0)
                    .WithMessage("O campo 'productId' deve ser maior que zero.");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0)
                    .WithMessage("A quantity é obrigatória e deve ser maior que 0.");
            });
        }
    }
}
