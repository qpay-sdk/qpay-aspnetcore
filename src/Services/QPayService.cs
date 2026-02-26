using QPay.Models;

namespace QPay.AspNetCore;

public class QPayService : IQPayService
{
    private readonly QPayClient _client;

    public QPayService(QPayClient client)
    {
        _client = client;
    }

    public Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
        => _client.CreateInvoiceAsync(request);

    public Task<InvoiceResponse> CreateSimpleInvoiceAsync(CreateSimpleInvoiceRequest request)
        => _client.CreateSimpleInvoiceAsync(request);

    public Task CancelInvoiceAsync(string invoiceId)
        => _client.CancelInvoiceAsync(invoiceId);

    public Task<PaymentDetail> GetPaymentAsync(string paymentId)
        => _client.GetPaymentAsync(paymentId);

    public Task<PaymentCheckResponse> CheckPaymentAsync(PaymentCheckRequest request)
        => _client.CheckPaymentAsync(request);

    public Task<PaymentListResponse> ListPaymentsAsync(PaymentListRequest request)
        => _client.ListPaymentsAsync(request);
}
