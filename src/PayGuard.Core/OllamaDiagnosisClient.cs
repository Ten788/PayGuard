using System.Net.Http.Json;
using System.Text.Json;

namespace PayGuard.Core;

public sealed class OllamaDiagnosisClient(HttpClient http)
{
    public async Task<string> DiagnoseAsync(
        PaymentTraceEvent evidence,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(evidence);

        var prompt = $"""
            Investigate this PayPal sandbox API trace.

            Separate verified facts from possible causes.
            Never invent API responses or successful repairs.
            Recommend read-only verification before payment actions.

            Operation: {evidence.Operation}
            HTTP status: {evidence.HttpStatus}
            Outcome: {evidence.Outcome}
            Order ID: {evidence.PayPalOrderId}
            Duration milliseconds: {evidence.DurationMilliseconds}

            Respond with four sections:
            Likely Cause
            Verified Evidence
            Safe Next Step
            Uncertainty
            """;

        var request = new
        {
            model = "qwen2.5:3b",
            stream = false,
            options = new
            {
                temperature = 0,
                num_predict = 300
            },
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        using var response = await http.PostAsJsonAsync(
            "http://127.0.0.1:11434/api/chat",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(
                cancellationToken),
            cancellationToken: cancellationToken);

        var content = document.RootElement
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
            throw new InvalidOperationException(
                "Ollama returned an empty diagnosis.");

        return content;
    }
}