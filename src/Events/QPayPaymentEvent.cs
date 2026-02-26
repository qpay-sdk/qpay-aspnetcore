using QPay.Models;

namespace QPay.AspNetCore;

public class QPayPaymentEvent
{
    public string InvoiceId { get; set; } = "";
    public bool IsPaid { get; set; }
    public PaymentCheckResponse? Result { get; set; }
}

public interface IQPayEventHandler
{
    Task HandlePaymentAsync(QPayPaymentEvent paymentEvent);
}
