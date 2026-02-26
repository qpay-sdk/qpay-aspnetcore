namespace QPay.AspNetCore;

public class QPayOptions
{
    public string BaseUrl { get; set; } = "https://merchant.qpay.mn";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string InvoiceCode { get; set; } = "";
    public string CallbackUrl { get; set; } = "";
    public string WebhookPath { get; set; } = "/api/qpay/webhook";
}
