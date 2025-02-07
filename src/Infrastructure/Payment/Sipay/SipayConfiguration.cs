using Microsoft.Extensions.Configuration;
using OCK.Core.Utilities;

namespace Infrastructure.Payment.Sipay;

public sealed class SipayConfiguration
{
    public static string ApiUrl { get; private set; } = string.Empty;
    public static int MerchantID { get; private set; }
    public static string MerchantKey { get; private set; } = string.Empty;
    public static string AppKey { get; private set; } = string.Empty;
    public static string AppSecret { get; private set; } = string.Empty;
    public static int TimeoutSecond { get; private set; }

    static SipayConfiguration()
    {
        var configuration = ServiceTools.GetService<IConfiguration>();
        var sipayConfig = configuration.GetSection("SipayConfigurations");
        if (sipayConfig == null) return;

        ApiUrl = sipayConfig.GetValue<string>("ApiUrl") ?? string.Empty;
        MerchantID = sipayConfig.GetValue<int>("MerchantID");
        MerchantKey = sipayConfig.GetValue<string>("MerchantKey") ?? string.Empty;
        AppKey = sipayConfig.GetValue<string>("AppKey") ?? string.Empty;
        AppSecret = sipayConfig.GetValue<string>("AppSecret") ?? string.Empty;
        TimeoutSecond = sipayConfig.GetValue<int>("TimeoutSecond");
    }
}