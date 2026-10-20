using PayGuard.Core;

int passed = 0;
int failed = 0;

void Check(string name, PaymentIntent? intent,
    PaymentProposal? proposal, bool allowed, string reason)
{
    var result = PaymentAuthorization.Evaluate(intent, proposal);
    if (result.Allowed == allowed && result.Reason == reason)
    {
        Console.WriteLine($"PASS: {name}");
        passed++;
    }
    else
    {
        Console.WriteLine($"FAIL: {name} ({result.Reason})");
        failed++;
    }
}

var intent = new PaymentIntent("merchant-A", "USD", 4900);
var valid = new PaymentProposal("merchant-A", "USD", 4900);

Check("Exact authorization", intent, valid, true, "MATCH");
Check("Merchant substitution", intent,
    valid with { MerchantId = "merchant-B" },
    false, "MERCHANT_MISMATCH");
Check("Currency substitution", intent,
    valid with { Currency = "EUR" },
    false, "CURRENCY_MISMATCH");
Check("Amount increase", intent,
    valid with { AmountMinor = 9900 },
    false, "AMOUNT_MISMATCH");
Check("Amount decrease", intent,
    valid with { AmountMinor = 100 },
    false, "AMOUNT_MISMATCH");
Check("Zero amount", intent,
    valid with { AmountMinor = 0 },
    false, "INVALID_INPUT");
Check("Negative amount", intent,
    valid with { AmountMinor = -1 },
    false, "INVALID_INPUT");
Check("Empty merchant", intent,
    valid with { MerchantId = "" },
    false, "INVALID_INPUT");
Check("Whitespace merchant", intent,
    valid with { MerchantId = " " },
    false, "INVALID_INPUT");
Check("Empty currency", intent,
    valid with { Currency = "" },
    false, "INVALID_INPUT");
Check("Missing intent", null, valid,
    false, "MISSING_INPUT");
Check("Missing proposal", intent, null,
    false, "MISSING_INPUT");
Check("Both inputs missing", null, null,
    false, "MISSING_INPUT");
Check("Case-sensitive merchant", intent,
    valid with { MerchantId = "Merchant-A" },
    false, "MERCHANT_MISMATCH");
Check("Case-sensitive currency", intent,
    valid with { Currency = "usd" },
    false, "CURRENCY_MISMATCH");
Check("Invalid authorized amount",
    intent with { AmountMinor = 0 }, valid,
    false, "INVALID_INPUT");

var gate = new OneTimeAuthorization();

void GateCheck(string name, AuthorizationDecision actual,
    bool allowed, string reason)
{
    if (actual.Allowed == allowed && actual.Reason == reason)
    {
        Console.WriteLine($"PASS: {name}");
        passed++;
    }
    else
    {
        Console.WriteLine($"FAIL: {name} ({actual.Reason})");
        failed++;
    }
}

GateCheck("First authorization",
    gate.TryConsume("approval-001", intent, valid),
    true, "AUTHORIZED_ONCE");

GateCheck("Replay rejected",
    gate.TryConsume("approval-001", intent, valid),
    false, "REPLAY_DENIED");

GateCheck("Invalid approval ID",
    gate.TryConsume("", intent, valid),
    false, "INVALID_APPROVAL_ID");

GateCheck("Tampered payment rejected",
    gate.TryConsume("approval-002", intent,
        valid with { AmountMinor = 9900 }),
    false, "AMOUNT_MISMATCH");

GateCheck("Rejected attempt does not consume approval",
    gate.TryConsume("approval-002", intent, valid),
    true, "AUTHORIZED_ONCE");
var bound = new BoundAuthorization();

void Assert(string name, bool condition)
{
    if (condition)
    {
        Console.WriteLine($"PASS: {name}");
        passed++;
    }
    else
    {
        Console.WriteLine($"FAIL: {name}");
        failed++;
    }
}

Assert("Register approval",
    bound.Register("bound-001", intent));

Assert("Duplicate registration denied",
    !bound.Register("bound-001",
        intent with { AmountMinor = 9900 }));

Assert("Unknown approval denied",
    bound.TryConsume("unknown", valid).Reason ==
        "UNKNOWN_APPROVAL");

Assert("Bound amount mismatch denied",
    bound.TryConsume("bound-001",
        valid with { AmountMinor = 9900 }).Reason ==
        "AMOUNT_MISMATCH");

Assert("Bound approval succeeds",
    bound.TryConsume("bound-001", valid).Allowed);

Assert("Bound approval replay denied",
    bound.TryConsume("bound-001", valid).Reason ==
        "REPLAY_DENIED");

Assert("Consumed approval cannot register again",
    !bound.Register("bound-001", intent));

Assert("Invalid registration rejected",
    !bound.Register("invalid",
        intent with { AmountMinor = 0 }));
Console.WriteLine($"RESULT: {passed} passed, {failed} failed");
if (failed != 0) Environment.ExitCode = 1;


