using Application.Commands;
using FluentValidation;

namespace Application.Validators;

public class AddBasketValidator : AbstractValidator<AddBasketCommand>
{
    public AddBasketValidator()
    {
        RuleFor(x => x.Products).NotEmpty();
        RuleForEach(x => x.Products).ChildRules(product =>
        {
            product.RuleFor(x => x.Name).NotEmpty();
            product.RuleFor(x => x.Quantity).GreaterThan(0);
            product.RuleFor(x => x.CostPerItem).GreaterThan(0);
        });
    }
}