using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agenda.Application.DTOs.Requests;
using FluentValidation;

namespace Agenda.Application.Validators
{
    public class CreateContactRequestValidator : AbstractValidator<CreateContactRequest>
    {
        public CreateContactRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Nome é obrigatorio.")
                .MinimumLength(2).WithMessage("Nome invalido.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatorio.")
                .EmailAddress().WithMessage("Email invalido.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefone é obrigatorio.")
                .MinimumLength(9).WithMessage("Telefone invalido.");
        }
    }
}
