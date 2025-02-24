using Microsoft.Extensions.Configuration;
using OCK.Core.Utilities;

namespace Infrastructure.Notification.OneSignal;

public sealed class OneSignalConfiguration
{
    public static string BaseUrl { get; private set; } = string.Empty;
    public static string AppId { get; private set; } = string.Empty;
    public static string KeyId { get; private set; } = string.Empty;
    public static string SecretKey { get; private set; } = string.Empty;
    public static string UniqueTemplateEmailAddress { get; private set; } = string.Empty;
    public static string TemplateId { get; private set; } = string.Empty;

    static OneSignalConfiguration()
    {
        var configuration = ServiceTools.GetService<IConfiguration>();
        var configs = configuration.GetSection("OneSignalConfigurations");
        if (configs == null) return;

        BaseUrl = configs.GetValue<string>("BaseUrl") ?? string.Empty;
        AppId = configs.GetValue<string>("AppId") ?? string.Empty;
        KeyId = configs.GetValue<string>("KeyId") ?? string.Empty;
        SecretKey = configs.GetValue<string>("SecretKey") ?? string.Empty;
        UniqueTemplateEmailAddress = configs.GetValue<string>("UniqueTemplateEmailAddress") ?? string.Empty;
        TemplateId = configs.GetValue<string>("TemplateId") ?? string.Empty;
    }
}