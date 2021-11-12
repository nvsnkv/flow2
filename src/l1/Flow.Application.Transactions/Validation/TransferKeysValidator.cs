using Flow.Domain.Transactions.Transfers;
using Flow.Domain.Transactions.Validation.Transfers;
using FluentValidation;

namespace Flow.Application.Transactions.Validation;

public class TransferKeysValidator : AbstractValidator<TransferKey>
{
    private readonly Func<TransferKey, bool> keysDiffers = TransferKeyValidationRules.SourceNotEqualToSink.Compile();

    public TransferKeysValidator()
    {
        RuleFor(t => t).Custom((t, c) =>
        {
            if (!keysDiffers(t)) { c.AddFailure(nameof(t.Sink), "Sink must be different from source!"); }
        });
    }
}