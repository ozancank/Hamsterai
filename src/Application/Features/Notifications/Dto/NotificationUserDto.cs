namespace Application.Features.Notifications.Dto;
public record NotificationUserDto(string Title,
                                  string Body,
                                  NotificationTypes Type,
                                  List<long> ReceivedUserId,
                                  IReadOnlyDictionary<string, string> Datas,
                                  string? ReasonId = null,
                                  long SenderUserId = 1) : IDto;