namespace PayGuard.Core;

public sealed class OneTimeAuthorization
{
    private readonly object _sync = new();
    private readonly HashSet<string> _consumed =
        new(StringComparer.Ordinal);

    public AuthorizationDecision TryConsume(
        string? approvalId,
        PaymentIntent? intent,
        PaymentProposal? proposal)
    {
        if (string.IsNullOrWhiteSpace(approvalId))
            return new(false, "INVALID_APPROVAL_ID");

        var decision = PaymentAuthorization.Evaluate(
            intent, proposal);

        if (!decision.Allowed)
            return decision;

        lock (_sync)
        {
            if (!_consumed.Add(approvalId))
                return new(false, "REPLAY_DENIED");

            return new(true, "AUTHORIZED_ONCE");
        }
    }
}
