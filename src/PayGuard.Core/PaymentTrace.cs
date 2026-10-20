using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PayGuard.Core;

public sealed record PaymentTraceEvent(
    int Sequence,
    string Operation,
    int? HttpStatus,
    string Outcome,
    string? PayPalOrderId,
    long DurationMilliseconds);

public sealed class PaymentTrace
{
    private static readonly HashSet<string> AllowedOperations =
        new(StringComparer.Ordinal)
        {
            "CREATE_ORDER",
            "GET_ORDER",
            "APPROVE_ORDER",
            "CAPTURE_ORDER",
            "NETWORK_REQUEST",
            "AUTH",
            "TEST"
        };

    private static readonly HashSet<string> AllowedOutcomes =
        new(StringComparer.Ordinal)
        {
            "CREATED",
            "APPROVED",
            "COMPLETED",
            "REJECTED",
            "TIMEOUT",
            "FAILED",
            "OK"
        };
    private readonly List<PaymentTraceEvent> _events = new();

    public IReadOnlyList<PaymentTraceEvent> Events =>
        new ReadOnlyCollection<PaymentTraceEvent>(_events);

    public void Record(
        string operation,
        int? httpStatus,
        string outcome,
        string? paypalOrderId,
        long durationMilliseconds)
    {
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Operation is required.");

        if (string.IsNullOrWhiteSpace(outcome))
            throw new ArgumentException("Outcome is required.");

        if (durationMilliseconds < 0)
            throw new ArgumentOutOfRangeException(
                nameof(durationMilliseconds));

        if (httpStatus is < 100 or > 599)
            throw new ArgumentOutOfRangeException(
                nameof(httpStatus));

        if (!IsSafeIdentifier(operation) ||
            !IsSafeIdentifier(outcome))
            throw new ArgumentException("Unsafe trace identifier.");

        if (paypalOrderId is not null &&
            !IsSafeIdentifier(paypalOrderId))
            throw new ArgumentException("Unsafe PayPal order ID.");
        if (!AllowedOperations.Contains(operation))
            throw new ArgumentException("Unsupported trace operation.");

        if (!AllowedOutcomes.Contains(outcome))
            throw new ArgumentException("Unsupported trace outcome.");
        _events.Add(new PaymentTraceEvent(
            _events.Count + 1,
            operation,
            httpStatus,
            outcome,
            paypalOrderId,
            durationMilliseconds));
    }

    private static bool IsSafeIdentifier(string value)
    {
        if (value.Length is < 1 or > 64)
            return false;

        foreach (char c in value)
        {
            if (!((c >= 'A' && c <= 'Z') ||
                  (c >= '0' && c <= '9') ||
                  c == '_' || c == '-'))
                return false;
        }

        return true;
    }}