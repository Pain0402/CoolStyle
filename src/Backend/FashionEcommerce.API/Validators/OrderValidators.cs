using FluentValidation;
using FashionEcommerce.Application.DTOs;

namespace FashionEcommerce.API.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Tên khách hàng không được để trống.")
            .MaximumLength(100);

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không hợp lệ.");

        RuleFor(x => x.CustomerPhone)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .Matches(@"^[0-9]{9,11}$").WithMessage("Số điện thoại không hợp lệ.");

        RuleFor(x => x.ShippingAddress)
            .NotEmpty().WithMessage("Địa chỉ giao hàng không được để trống.")
            .MinimumLength(10).WithMessage("Địa chỉ quá ngắn.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Đơn hàng phải có ít nhất 1 sản phẩm.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).GreaterThan(0).WithMessage("ProductId không hợp lệ.");
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");
        });
    }
}
