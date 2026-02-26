using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using QPay.Models;

namespace QPay.AspNetCore;

public class QPayWebhookMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _webhookPath;

    public QPayWebhookMiddleware(RequestDelegate next, QPayOptions options)
    {
        _next = next;
        _webhookPath = options.WebhookPath;
    }

    public async Task InvokeAsync(HttpContext context, IQPayService qpay)
    {
        if (context.Request.Method == "POST" && context.Request.Path.Equals(_webhookPath, StringComparison.OrdinalIgnoreCase))
        {
            var body = await JsonSerializer.DeserializeAsync<JsonElement>(context.Request.Body);
            var invoiceId = body.TryGetProperty("invoice_id", out var id) ? id.GetString() : null;

            if (string.IsNullOrEmpty(invoiceId))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = "Missing invoice_id" });
                return;
            }

            try
            {
                var result = await qpay.CheckPaymentAsync(new PaymentCheckRequest
                {
                    ObjectType = "INVOICE",
                    ObjectId = invoiceId,
                });

                var handler = context.RequestServices.GetService(typeof(IQPayEventHandler)) as IQPayEventHandler;
                var isPaid = result.Rows?.Count > 0;

                if (handler != null)
                {
                    await handler.HandlePaymentAsync(new QPayPaymentEvent
                    {
                        InvoiceId = invoiceId,
                        IsPaid = isPaid,
                        Result = result,
                    });
                }

                await context.Response.WriteAsJsonAsync(new { status = isPaid ? "paid" : "unpaid" });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            return;
        }

        await _next(context);
    }
}

public static class QPayApplicationBuilderExtensions
{
    public static IApplicationBuilder UseQPayWebhook(this IApplicationBuilder app)
    {
        return app.UseMiddleware<QPayWebhookMiddleware>();
    }
}
