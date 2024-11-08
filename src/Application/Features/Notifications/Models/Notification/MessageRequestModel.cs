namespace Application.Features.Notifications.Models.Notification;

public class MessageRequestModel : IRequestModel
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}