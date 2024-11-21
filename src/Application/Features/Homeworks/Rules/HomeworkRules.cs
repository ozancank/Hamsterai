using DataAccess.EF;

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
        var control = await homeworkDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Id) == PostgresqlFunctions.TrLower(id), enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, $"{Strings.Homework} Id");
    }

    internal static Task HomeworkStudentShouldExists(object homework)
    {
        if (homework == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Homework);
        return Task.CompletedTask;
    }

    internal static Task HomeworkUserShouldExists(object homework)
    {
        if (homework == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Homework);
        return Task.CompletedTask;
    }

    internal async Task HomeworkStudentShouldNotExistsById(string id)
    {
        var control = await homeworkStudentDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Id) == PostgresqlFunctions.TrLower(id), enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, $"{Strings.Homework}-{Strings.Student} Id ");
    }

    internal static Task OnlyOneShouldBeFilled(int? classRoomId, List<int> studentIds, List<long> userIds, List<short> packageIds)
    {
        int filledCount = 0;

        if (classRoomId != null && classRoomId > 0) filledCount++;
        if (studentIds != null && studentIds.Count > 0) filledCount++;
        if (userIds != null && userIds.Count > 0) filledCount++;
        if (packageIds != null && packageIds.Count > 0) filledCount++;

        if (filledCount != 1)
        {
            throw new BusinessException(Strings.DynamicOnlyOneShouldBeFilled, $"{Strings.ClassRoom}, {Strings.Student}, {Strings.User}, {Strings.Package}");
        }

        return Task.CompletedTask;
    }

    internal static Task HomeworkSendUserShouldBeTeacher(UserTypes httpUserType)
    {
        if (httpUserType != UserTypes.Administator && httpUserType != UserTypes.Teacher) throw new BusinessException(Strings.DynamicUserTypeShouldBe, Strings.Teacher);
        return Task.CompletedTask;
    }
}