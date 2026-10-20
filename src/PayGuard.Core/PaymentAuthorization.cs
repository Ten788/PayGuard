namespace PayGuard.Core;

public sealed record PaymentIntent(
    string MerchantId,
    string Currency,
    long AmountMinor);

public sealed record PaymentProposal(
    string MerchantId,
    string Currency,
    long AmountMinor);

public sealed record AuthorizationDecision(
    bool Allowed,
    string Reason);

public static class PaymentAuthorization
{
    public static AuthorizationDecision Evaluate(
        PaymentIntent? intent,
        PaymentProposal? proposal)
    {
        if (intent is null || proposal is null)
            return new(false, "MISSING_INPUT");

        if (string.IsNullOrWhiteSpace(intent.MerchantId) ||
            string.IsNullOrWhiteSpace(proposal.MerchantId) ||
            string.IsNullOrWhiteSpace(intent.Currency) ||
            string.IsNullOrWhiteSpace(proposal.Currency) ||
            intent.AmountMinor <= 0 ||
            proposal.AmountMinor <= 0)
            return new(false, "INVALID_INPUT");

        if (!string.Equals(
            intent.MerchantId, proposal.MerchantId,
            StringComparison.Ordinal))
            return new(false, "MERCHANT_MISMATCH");

        if (!string.Equals(
            intent.Currency, proposal.Currency,
            StringComparison.Ordinal))
            return new(false, "CURRENCY_MISMATCH");

        if (intent.AmountMinor != proposal.AmountMinor)
            return new(false, "AMOUNT_MISMATCH");

        return new(true, "MATCH");
    }
}
