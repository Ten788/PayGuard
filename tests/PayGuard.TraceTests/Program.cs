using PayGuard.Core;

int passed = 0;

void Check(string name, Action action)
{
    action();
    Console.WriteLine($"PASS: {name}");
    passed++;
}

void Reject<T>(Action action) where T : Exception
{
    try
    {
        action();
    }
    catch (T)
    {
        return;
    }

    throw new Exception($"Expected {typeof(T).Name}");
}

var trace = new PaymentTrace();

Check("Initially empty", () =>
{
    if (trace.Events.Count != 0)
        throw new Exception("Unexpected events");
});

Check("Record successful order", () =>
{
    trace.Record("CREATE_ORDER", 201, "CREATED", "ORDER-123", 42);

    var e = trace.Events[0];
    if (e.Sequence != 1 ||
        e.Operation != "CREATE_ORDER" ||
        e.HttpStatus != 201 ||
        e.Outcome != "CREATED" ||
        e.PayPalOrderId != "ORDER-123" ||
        e.DurationMilliseconds != 42)
        throw new Exception("Recorded fields mismatch");
});

Check("Record failed operation", () =>
{
    trace.Record("CAPTURE_ORDER", 422, "REJECTED", "ORDER-123", 17);

    if (trace.Events.Count != 2 ||
        trace.Events[1].Sequence != 2 ||
        trace.Events[1].HttpStatus != 422)
        throw new Exception("Ordering mismatch");
});

Check("Missing HTTP status supported", () =>
{
    trace.Record("NETWORK_REQUEST", null, "TIMEOUT", null, 2000);

    if (trace.Events[2].HttpStatus != null ||
        trace.Events[2].PayPalOrderId != null)
        throw new Exception("Null fields mismatch");
});

Check("Empty operation rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("", 200, "OK", null, 0)));

Check("Empty outcome rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("TEST", 200, " ", null, 0)));

Check("Negative duration rejected", () =>
    Reject<ArgumentOutOfRangeException>(() =>
        trace.Record("TEST", 200, "OK", null, -1)));

Check("Invalid low HTTP status rejected", () =>
    Reject<ArgumentOutOfRangeException>(() =>
        trace.Record("TEST", 99, "OK", null, 0)));

Check("Invalid high HTTP status rejected", () =>
    Reject<ArgumentOutOfRangeException>(() =>
        trace.Record("TEST", 600, "OK", null, 0)));

Check("Rejected events do not append", () =>
{
    if (trace.Events.Count != 3)
        throw new Exception("Rejected input changed trace");
});

Check("Events collection cannot be modified", () =>
{
    var list = (System.Collections.Generic.IList<PaymentTraceEvent>)trace.Events;

    Reject<NotSupportedException>(() =>
        list.Add(new PaymentTraceEvent(4, "X", 200, "OK", null, 0)));
});

Check("Bearer token text rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("AUTH", 200, "Bearer abc.def.ghi", null, 1)));

Check("JSON payload rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("CREATE_ORDER", 200, "{\"token\":\"secret\"}", null, 1)));

Check("Unsafe operation rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("POST /v2/checkout/orders", 200, "OK", null, 1)));

Check("Unsafe order ID rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("CREATE_ORDER", 201, "CREATED", "id with spaces", 1)));

Check("Oversized outcome rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("TEST", 200, new string('A', 65), null, 1)));

Check("Lowercase operation rejected", () =>
    Reject<ArgumentException>(() =>
        trace.Record("create_order", 200, "OK", null, 1)));

Check("Rejected security inputs do not append", () =>
{
    if (trace.Events.Count != 3)
        throw new Exception("Rejected data was recorded");
});
Console.WriteLine($"RESULT: {passed} passed, 0 failed");