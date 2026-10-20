namespace PayGuard.Core;

public sealed class BoundAuthorization
{
    private readonly object _sync = new();
    private readonly Dictionary<string, PaymentIntent> _approvals =
        new(StringComparer.Ordinal);
    private readonly HashSet<string> _consumed =
        new(StringComparer.Ordinal);

    public bool Register(string? id, PaymentIntent? intent)
    {
        if (string.IsNullOrWhiteSpace(id) || intent is null)
            return false;

        if (!PaymentAuthorization.Evaluate(
            intent,
            new PaymentProposal(
                intent.MerchantId,
                intent.Currency,
                intent.AmountMinor)).Allowed)
            return false;

        lock (_sync)
        {
            if (_consumed.Contains(id))
                return false;

            return _approvals.TryAdd(id, intent);
        }
    }

    public AuthorizationDecision TryConsume(
        string? id, PaymentProposal? proposal)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new(false, "INVALID_APPROVAL_ID");

        lock (_sync)
        {
            if (_consumed.Contains(id))
                return new(false, "REPLAY_DENIED");

            if (!_approvals.TryGetValue(id, out var intent))
                return new(false, "UNKNOWN_APPROVAL");

            var decision = PaymentAuthorization.Evaluate(
                intent, proposal);

            if (!decision.Allowed)
                return decision;

            _consumed.Add(id);
            return new(true, "AUTHORIZED_ONCE");
        }
    }
}
