using Microsoft.Extensions.DependencyInjection;

namespace QPay.AspNetCore;

public static class QPayServiceCollectionExtensions
{
    public static IServiceCollection AddQPay(this IServiceCollection services, Action<QPayOptions> configure)
    {
        var options = new QPayOptions();
        configure(options);

        var config = new QPayConfig
        {
            BaseUrl = options.BaseUrl,
            Username = options.Username,
            Password = options.Password,
            InvoiceCode = options.InvoiceCode,
            CallbackUrl = options.CallbackUrl,
        };

        var client = new QPayClient(config);
        services.AddSingleton(options);
        services.AddSingleton(client);
        services.AddSingleton<IQPayService, QPayService>();

        return services;
    }
}
