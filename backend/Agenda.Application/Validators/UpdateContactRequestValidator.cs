using FluentValidation;
using Agenda.Application.DTOs.Requests;

namespace Agenda.Application.Validators
{
    public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
    {
        public UpdateContactRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(2);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Phone)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}