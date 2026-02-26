using Moq;
using QPay.Models;

namespace QPay.AspNetCore.Tests;

public class QPayServiceTests
{
    [Fact]
    public async Task IQPayService_CreateInvoiceAsync_CanBeMocked()
    {
        var mock = new Mock<IQPayService>();
        var request = new CreateInvoiceRequest { InvoiceCode = "TEST", Amount = 50000 };
        var expected = new InvoiceResponse { InvoiceId = "inv_001", QRText = "qr_data" };

        mock.Setup(s => s.CreateInvoiceAsync(request)).ReturnsAsync(expected);

        var result = await mock.Object.CreateInvoiceAsync(request);

        Assert.Equal("inv_001", result.InvoiceId);
        Assert.Equal("qr_data", result.QRText);
        mock.Verify(s => s.CreateInvoiceAsync(request), Times.Once);
    }

    [Fact]
    public async Task IQPayService_CreateSimpleInvoiceAsync_CanBeMocked()
    {
        var mock = new Mock<IQPayService>();
        var request = new CreateSimpleInvoiceRequest
        {
            InvoiceCode = "INV_TEST",
            SenderInvoiceNo = "ORDER-001",
            Amount = 50000
        };
        var expected = new InvoiceResponse { InvoiceId = "inv_002", QRText = "qr_simple" };

        mock.Setup(s => s.CreateSimpleInvoiceAsync(request)).ReturnsAsync(expected);

        var result = await mock.Object.CreateSimpleInvoiceAsync(request);

        Assert.Equal("inv_002", result.InvoiceId);
        Assert.Equal("qr_simple", result.QRText);
    }

    [Fact]
    public async Task IQPayService_CancelInvoiceAsync_CanBeMocked()
    {
        var mock = new Mock<IQPayService>();
        mock.Setup(s => s.CancelInvoiceAsync("inv_003")).Returns(Task.CompletedTask);

        await mock.Object.CancelInvoiceAsync("inv_003");

        mock.Verify(s => s.CancelInvoiceAsync("inv_003"), Times.Once);
    }

    [Fact]
    public async Task IQPayService_GetPaymentAsync_CanBeMocked()
    {
        var mock = new Mock<IQPayService>();
        var expected = new PaymentDetail
        {
            PaymentId = "pay_001",
            PaymentStatus = "PAID"
        };

        mock.Setup(s => s.GetPaymentAsync("pay_001")).ReturnsAsync(expected);

        var result = await mock.Object.GetPaymentAsync("pay_001");

        Assert.Equal("pay_001", result.PaymentId);
        Assert.Equal("PAID", result.PaymentStatus);
    }

    [Fact]
    public async Task IQPayService_CheckPaymentAsync_ReturnsPaidRows()
    {
        var mock = new Mock<IQPayService>();
        var request = new PaymentCheckRequest
        {
            ObjectType = "INVOICE",
            ObjectId = "inv_004"
        };
        var expected = new PaymentCheckResponse
        {
            Count = 1,
            Rows = new List<PaymentCheckRow>
            {
                new() { PaymentId = "pay_002", PaymentStatus = "PAID" }
            }
        };

        mock.Setup(s => s.CheckPaymentAsync(request)).ReturnsAsync(expected);

        var result = await mock.Object.CheckPaymentAsync(request);

        Assert.Single(result.Rows);
        Assert.Equal("pay_002", result.Rows[0].PaymentId);
        Assert.Equal("PAID", result.Rows[0].PaymentStatus);
    }

    [Fact]
    public async Task IQPayService_CheckPaymentAsync_ReturnsEmptyForUnpaid()
    {
        var mock = new Mock<IQPayService>();
        var request = new PaymentCheckRequest
        {
            ObjectType = "INVOICE",
            ObjectId = "inv_unpaid"
        };
        var expected = new PaymentCheckResponse
        {
            Count = 0,
            Rows = new List<PaymentCheckRow>()
        };

        mock.Setup(s => s.CheckPaymentAsync(request)).ReturnsAsync(expected);

        var result = await mock.Object.CheckPaymentAsync(request);

        Assert.Empty(result.Rows);
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public async Task IQPayService_ListPaymentsAsync_ReturnsMultipleRows()
    {
        var mock = new Mock<IQPayService>();
        var request = new PaymentListRequest();
        var expected = new PaymentListResponse
        {
            Count = 2,
            Rows = new List<PaymentListItem>
            {
                new() { PaymentId = "pay_010" },
                new() { PaymentId = "pay_011" }
            }
        };

        mock.Setup(s => s.ListPaymentsAsync(request)).ReturnsAsync(expected);

        var result = await mock.Object.ListPaymentsAsync(request);

        Assert.Equal(2, result.Rows.Count);
        Assert.Equal("pay_010", result.Rows[0].PaymentId);
        Assert.Equal("pay_011", result.Rows[1].PaymentId);
    }

    [Fact]
    public async Task IQPayService_CreateInvoiceAsync_PropagatesException()
    {
        var mock = new Mock<IQPayService>();
        mock.Setup(s => s.CreateInvoiceAsync(It.IsAny<CreateInvoiceRequest>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => mock.Object.CreateInvoiceAsync(new CreateInvoiceRequest())
        );
    }

    [Fact]
    public async Task IQPayService_CancelInvoiceAsync_PropagatesException()
    {
        var mock = new Mock<IQPayService>();
        mock.Setup(s => s.CancelInvoiceAsync("bad_id"))
            .ThrowsAsync(new HttpRequestException("Not found"));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => mock.Object.CancelInvoiceAsync("bad_id")
        );
    }

    [Fact]
    public void QPayPaymentEvent_DefaultValues()
    {
        var evt = new QPayPaymentEvent();

        Assert.Equal("", evt.InvoiceId);
        Assert.False(evt.IsPaid);
        Assert.Null(evt.Result);
    }

    [Fact]
    public void QPayPaymentEvent_CanSetProperties()
    {
        var checkResponse = new PaymentCheckResponse
        {
            Count = 1,
            Rows = new List<PaymentCheckRow>
            {
                new() { PaymentId = "pay_100", PaymentStatus = "PAID" }
            }
        };

        var evt = new QPayPaymentEvent
        {
            InvoiceId = "inv_200",
            IsPaid = true,
            Result = checkResponse
        };

        Assert.Equal("inv_200", evt.InvoiceId);
        Assert.True(evt.IsPaid);
        Assert.NotNull(evt.Result);
        Assert.Single(evt.Result.Rows);
    }
}
