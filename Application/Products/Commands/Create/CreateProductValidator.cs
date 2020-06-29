using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Products.Commands.Create
{
    class CreateProductValidator: AbstractValidator<CreateProduct>
    {
        public CreateProductValidator()
        {
            RuleFor(v => v.Name)
                .MaximumLength(50)
                .NotEmpty();

            RuleFor(v => v.Description)
                .MaximumLength(200)
                .NotEmpty();

            RuleFor(v => v.Price)
              .NotEmpty().GreaterThan(0);
        }
    }
}
