using FluentValidation;
using OrderProcessor.Service.Domain;

namespace OrderProcessor.Service.Validation
{
    public sealed class ProcessRequestValidator : AbstractValidator<ProcessRequest>
    {
        public ProcessRequestValidator()
        {
            RuleFor(x => x.Csv)
                .NotEmpty()
                .WithMessage("CSV content is required.");
        }
    }
}