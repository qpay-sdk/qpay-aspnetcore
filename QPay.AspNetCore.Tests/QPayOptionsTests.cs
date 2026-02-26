namespace QPay.AspNetCore.Tests;

public class QPayOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new QPayOptions();

        Assert.Equal("https://merchant.qpay.mn", options.BaseUrl);
        Assert.Equal("", options.Username);
        Assert.Equal("", options.Password);
        Assert.Equal("", options.InvoiceCode);
        Assert.Equal("", options.CallbackUrl);
        Assert.Equal("/api/qpay/webhook", options.WebhookPath);
    }

    [Fact]
    public void CanSetBaseUrl()
    {
        var options = new QPayOptions { BaseUrl = "https://custom.qpay.mn" };
        Assert.Equal("https://custom.qpay.mn", options.BaseUrl);
    }

    [Fact]
    public void CanSetUsername()
    {
        var options = new QPayOptions { Username = "testuser" };
        Assert.Equal("testuser", options.Username);
    }

    [Fact]
    public void CanSetPassword()
    {
        var options = new QPayOptions { Password = "testpass" };
        Assert.Equal("testpass", options.Password);
    }

    [Fact]
    public void CanSetInvoiceCode()
    {
        var options = new QPayOptions { InvoiceCode = "INV_001" };
        Assert.Equal("INV_001", options.InvoiceCode);
    }

    [Fact]
    public void CanSetCallbackUrl()
    {
        var options = new QPayOptions { CallbackUrl = "https://example.com/callback" };
        Assert.Equal("https://example.com/callback", options.CallbackUrl);
    }

    [Fact]
    public void CanSetWebhookPath()
    {
        var options = new QPayOptions { WebhookPath = "/custom/webhook" };
        Assert.Equal("/custom/webhook", options.WebhookPath);
    }

    [Fact]
    public void AllPropertiesCanBeSetTogether()
    {
        var options = new QPayOptions
        {
            BaseUrl = "https://test.qpay.mn",
            Username = "user1",
            Password = "pass1",
            InvoiceCode = "INV_TEST",
            CallbackUrl = "https://test.com/cb",
            WebhookPath = "/test/webhook"
        };

        Assert.Equal("https://test.qpay.mn", options.BaseUrl);
        Assert.Equal("user1", options.Username);
        Assert.Equal("pass1", options.Password);
        Assert.Equal("INV_TEST", options.InvoiceCode);
        Assert.Equal("https://test.com/cb", options.CallbackUrl);
        Assert.Equal("/test/webhook", options.WebhookPath);
    }
}
