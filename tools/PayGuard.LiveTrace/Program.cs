using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using PayGuard.Core;

var clientId = Environment.GetEnvironmentVariable("PAYGUARD_CLIENT_ID");
var secret = Environment.GetEnvironmentVariable("PAYGUARD_CLIENT_SECRET");

if (string.IsNullOrWhiteSpace(clientId) ||
    string.IsNullOrWhiteSpace(secret))
{
    Console.Error.WriteLine("Missing sandbox credentials.");
    return 1;
}

using var http = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(20)
};

var authBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{secret}");
var auth = Convert.ToBase64String(authBytes);

using var tokenRequest = new HttpRequestMessage(
    HttpMethod.Post,
    "https://api-m.sandbox.paypal.com/v1/oauth2/token");

tokenRequest.Headers.Authorization =
    new AuthenticationHeaderValue("Basic", auth);

tokenRequest.Content = new FormUrlEncodedContent(
    new Dictionary<string, string>
    {
        ["grant_type"] = "client_credentials"
    });

using var tokenResponse = await http.SendAsync(tokenRequest);

if (!tokenResponse.IsSuccessStatusCode)
{
    Console.Error.WriteLine(
        $"OAuth HTTP {(int)tokenResponse.StatusCode}");
    return 2;
}

using var tokenJson = JsonDocument.Parse(
    await tokenResponse.Content.ReadAsStringAsync());

var token = tokenJson.RootElement
    .GetProperty("access_token").GetString();

if (string.IsNullOrWhiteSpace(token))
    throw new InvalidOperationException("OAuth token missing.");

http.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);

var trace = new PaymentTrace();

var fullInvestigation = args.Length == 1 &&
    args[0] == "--full-investigation";

if ((args.Length == 1 && args[0] == "--fault-invalid-order") ||
    fullInvestigation)
{
    const string invalidId = "NONEXISTENT-ORDER-000000";
    var faultTimer = Stopwatch.StartNew();

    using var faultResponse = await http.GetAsync(
        $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{invalidId}");

    faultTimer.Stop();

    if (faultResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
    {
        Console.Error.WriteLine(
            $"Unexpected fault HTTP {(int)faultResponse.StatusCode}");
        return 5;
    }

    trace.Record(
        "GET_ORDER",
        (int)faultResponse.StatusCode,
        "REJECTED",
        invalidId,
        faultTimer.ElapsedMilliseconds);

    foreach (var e in trace.Events)
    {
        Console.WriteLine(
            $"TRACE {e.Sequence}: {e.Operation} " +
            $"HTTP={e.HttpStatus} STATUS={e.Outcome} " +
            $"ORDER={e.PayPalOrderId} MS={e.DurationMilliseconds}");
    }

    Console.WriteLine("FAULT: INVALID_ORDER_ID");
    Console.WriteLine("DIAGNOSIS: Requested order does not exist.");
    Console.WriteLine("REPAIR: Use an ID returned by CREATE_ORDER.");
    Console.WriteLine("AI ANALYSIS: STARTING");

    using (var aiHttp = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(120)
    })
    {
        var aiClient = new OllamaDiagnosisClient(aiHttp);
        var evidence = trace.Events.Single();

        try
        {
            var aiDiagnosis = await aiClient.DiagnoseAsync(evidence);
            Console.WriteLine("AI MODEL: qwen2.5:3b");
            Console.WriteLine("AI DIAGNOSIS BEGIN");
            Console.WriteLine(aiDiagnosis);
            Console.WriteLine("AI DIAGNOSIS END");
            Console.WriteLine("AI STATUS: GENERATED - NOT VERIFIED");
        }
        catch (Exception ex)
        {
            Console.WriteLine("AI STATUS: UNAVAILABLE");
            Console.WriteLine($"AI ERROR TYPE: {ex.GetType().Name}");
        }
    }
    Console.WriteLine("RESULT: CONTROLLED PAYPAL FAILURE VERIFIED");
    Console.WriteLine("PAYMENT CAPTURED: NO");

    if (!fullInvestigation)
        return 0;

    Console.WriteLine("RECOVERY PHASE: STARTING");
}

var verifyRecovery = fullInvestigation ||
    (args.Length == 1 && args[0] == "--verify-recovery");

if (args.Length > 0 && !verifyRecovery)
{
    Console.Error.WriteLine("Unsupported application mode.");
    return 6;
}
var payload = new
{
    intent = "CAPTURE",
    purchase_units = new[]
    {
        new
        {
            amount = new
            {
                currency_code = "USD",
                value = "1.00"
            }
        }
    }
};

using var createRequest = new HttpRequestMessage(
    HttpMethod.Post,
    "https://api-m.sandbox.paypal.com/v2/checkout/orders");

createRequest.Headers.Add(
    "PayPal-Request-Id",
    Guid.NewGuid().ToString("N"));

createRequest.Headers.Add("Prefer", "return=representation");
createRequest.Content = JsonContent.Create(payload);

var timer = Stopwatch.StartNew();
using var createResponse = await http.SendAsync(createRequest);
timer.Stop();

if (!createResponse.IsSuccessStatusCode)
{
    Console.Error.WriteLine(
        $"Create HTTP {(int)createResponse.StatusCode}");
    return 3;
}

using var createdJson = JsonDocument.Parse(
    await createResponse.Content.ReadAsStringAsync());

var orderId = createdJson.RootElement.GetProperty("id").GetString();
var status = createdJson.RootElement.GetProperty("status").GetString();

if (string.IsNullOrWhiteSpace(orderId) || status != "CREATED")
    throw new InvalidOperationException("Unexpected create response.");

trace.Record("CREATE_ORDER",
    (int)createResponse.StatusCode,
    "CREATED",
    orderId,
    timer.ElapsedMilliseconds);

timer.Restart();
using var getResponse = await http.GetAsync(
    $"https://api-m.sandbox.paypal.com/v2/checkout/orders/{orderId}");
timer.Stop();

if (!getResponse.IsSuccessStatusCode)
{
    Console.Error.WriteLine(
        $"Retrieve HTTP {(int)getResponse.StatusCode}");
    return 4;
}

using var retrievedJson = JsonDocument.Parse(
    await getResponse.Content.ReadAsStringAsync());

var retrievedId = retrievedJson.RootElement.GetProperty("id").GetString();
var retrievedStatus = retrievedJson.RootElement
    .GetProperty("status").GetString();

if (retrievedId != orderId || retrievedStatus != "CREATED")
    throw new InvalidOperationException("Retrieved order mismatch.");

trace.Record("GET_ORDER",
    (int)getResponse.StatusCode,
    "CREATED",
    retrievedId,
    timer.ElapsedMilliseconds);

foreach (var e in trace.Events)
{
    Console.WriteLine(
        $"TRACE {e.Sequence}: {e.Operation} " +
        $"HTTP={e.HttpStatus} STATUS={e.Outcome} " +
        $"ORDER={e.PayPalOrderId} MS={e.DurationMilliseconds}");
}

if (verifyRecovery)
{
    var createEvent = trace.Events.Single(
        e => e.Operation == "CREATE_ORDER");
    var getEvent = trace.Events.Single(
        e => e.Operation == "GET_ORDER" &&
             e.HttpStatus == 200 &&
             e.Outcome == "CREATED" &&
             e.PayPalOrderId == createEvent.PayPalOrderId);

    if (createEvent.HttpStatus != 201 ||
        getEvent.HttpStatus != 200 ||
        createEvent.PayPalOrderId != getEvent.PayPalOrderId ||
        createEvent.Outcome != "CREATED" ||
        getEvent.Outcome != "CREATED")
    {
        throw new InvalidOperationException(
            "Recovery verification evidence mismatch.");
    }

    Console.WriteLine("RECOVERY VERIFICATION: PASS");
    Console.WriteLine("EVIDENCE: CREATE_ORDER 201 + GET_ORDER 200");
    Console.WriteLine("VERIFIED STATE: CREATED");
    Console.WriteLine("ORIGINAL FAILED ORDER REPAIRED: NO");
    Console.WriteLine("AI AUTHORIZED PAYMENT ACTIONS: NO");
    Console.WriteLine("RESULT: VERIFIED SANDBOX RECOVERY PATH PASS");
}
else
{
    Console.WriteLine("RESULT: LIVE PAYPAL FLIGHT RECORDER PASS");
}
Console.WriteLine("PAYMENT CAPTURED: NO");
return 0;