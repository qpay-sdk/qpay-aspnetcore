using QPay.Models;

namespace QPay.AspNetCore;

public interface IQPayService
{
    Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request);
    Task<InvoiceResponse> CreateSimpleInvoiceAsync(CreateSimpleInvoiceRequest request);
    Task CancelInvoiceAsync(string invoiceId);
    Task<PaymentDetail> GetPaymentAsync(string paymentId);
    Task<PaymentCheckResponse> CheckPaymentAsync(PaymentCheckRequest request);
    Task<PaymentListResponse> ListPaymentsAsync(PaymentListRequest request);
}
