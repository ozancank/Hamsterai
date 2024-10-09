
namespace Business.Features.Homeworks.Rules;

public class HomeworkRules : IBusinessRule
{
    internal static Task HomeworkShouldExists(object homework)
    {
        if (homework == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Homework);
        return Task.CompletedTask;
    }

    internal static Task HomeworkStudentShouldExists(object homework)
    {
        if (homework == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Homework);
        return Task.CompletedTask;
    }

    internal static Task OnlyOneShouldBeFilled(int? classRoomId, List<int> studentIds)
    {
        if ((classRoomId == null && (studentIds == null || studentIds.Count == 0))
            || (classRoomId != null && studentIds != null && studentIds.Count != 0))
            throw new BusinessException(Strings.DynamicOnlyOneShouldBeFilled, $"{Strings.ClassRoom} veya {Strings.Student} seçeneğinden");
        return Task.CompletedTask;
    }
}