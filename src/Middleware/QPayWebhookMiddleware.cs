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
        if (context.Request.Method == "GET" && context.Request.Path.Equals(_webhookPath, StringComparison.OrdinalIgnoreCase))
        {
            var paymentId = context.Request.Query["qpay_payment_id"].ToString();

            if (string.IsNullOrEmpty(paymentId))
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Missing qpay_payment_id");
                return;
            }

            try
            {
                var result = await qpay.CheckPaymentAsync(new PaymentCheckRequest
                {
                    ObjectType = "INVOICE",
                    ObjectId = paymentId,
                });

                var handler = context.RequestServices.GetService(typeof(IQPayEventHandler)) as IQPayEventHandler;
                var isPaid = result.Rows?.Count > 0;

                if (handler != null)
                {
                    await handler.HandlePaymentAsync(new QPayPaymentEvent
                    {
                        InvoiceId = paymentId,
                        IsPaid = isPaid,
                        Result = result,
                    });
                }

                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("SUCCESS");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(ex.Message);
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
