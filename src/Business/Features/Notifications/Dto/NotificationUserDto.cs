namespace Business.Features.Notifications.Dto;
public record NotificationUserDto(string Title,
                                  string Body,
                                  long ReceivedUserId,
                                  NotificationTypes Type,
                                  string ReasonId = null,
                                  long SenderUserId = 1) : IDto;