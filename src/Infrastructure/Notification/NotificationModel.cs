using OCK.Core.Interfaces;

namespace Infrastructure.Notification;

public sealed class NotificationModel<T> : IRequestModel
{
    public string? Title { get; set; }
    public string? Body { get; set; }
    public IReadOnlyDictionary<string, string> Datas { get; set; } = new Dictionary<string, string>();

    public IEnumerable<T?>? List { get; set; }
}