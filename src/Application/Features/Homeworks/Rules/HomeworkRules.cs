namespace Application.Features.Homeworks.Rules;

public class HomeworkRules(IHomeworkDal homeworkDal,
                           IHomeworkStudentDal homeworkStudentDal) : IBusinessRule
{
    internal static Task HomeworkShouldExists(object homework)
    {
        if (homework == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Homework);
        return Task.CompletedTask;
    }

    internal async Task HomeworkShouldNotExistsById(string id)
    {
        var control = await homeworkDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, $"{Strings.Homework} Id");
    }

    internal static Task HomeworkStudentShouldExists(object homework)
    {
        if (homework == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Homework);
        return Task.CompletedTask;
    }

    internal async Task HomeworkStudentShouldNotExistsById(string id)
    {
        var control = await homeworkStudentDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, $"{Strings.Homework}-{Strings.Student} Id ");
    }

    internal static Task OnlyOneShouldBeFilled(int? classRoomId, List<int> studentIds)
    {
        if ((classRoomId == null && (studentIds == null || studentIds.Count == 0))
            || (classRoomId != null && studentIds != null && studentIds.Count != 0))
            throw new BusinessException(Strings.DynamicOnlyOneShouldBeFilled, $"{Strings.ClassRoom} veya {Strings.Student} seçeneğinden");
        return Task.CompletedTask;
    }

    internal static Task HomeworkSendUserShouldBeTeacher(UserTypes httpUserType)
    {
        if (httpUserType != UserTypes.Teacher) throw new BusinessException(Strings.DynamicUserTypeShouldBe, Strings.Teacher);
        return Task.CompletedTask;
    }
}