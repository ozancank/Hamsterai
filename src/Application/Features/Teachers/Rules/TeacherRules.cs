using Application.Features.Teachers.Models;
using DataAccess.EF;

namespace Application.Features.Teachers.Rules;

public class TeacherRules(ITeacherDal teacherDal) : IBusinessRule
{
    internal static Task TeacherShouldExists(object teacher)
    {
        if (teacher == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Teacher);
        return Task.CompletedTask;
    }

    internal static Task TeacherShouldExists(GetTeacherModel teacherModel)
    {
        if (teacherModel == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Teacher);
        return Task.CompletedTask;
    }

    internal static Task TeacherShouldExists(Teacher teacher)
    {
        if (teacher == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Teacher);
        return Task.CompletedTask;
    }

    internal async Task TeacherShouldExists(int id)
    {
        var exists = await teacherDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        if (exists) throw new BusinessException(Strings.DynamicNotFound, Strings.Teacher);
    }

    internal async Task TeacherEmailCanNotBeDuplicated(string email, int? teacherId = null)
    {
        email = email.Trim();
        var teacher = await teacherDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Email) == PostgresqlFunctions.TrLower(email), enableTracking: false);
        if (teacherId == null && teacher != null) throw new BusinessException(Strings.DynamicExists, email);
        if (teacherId != null && teacher != null && teacher.Id != teacherId) throw new BusinessException(Strings.DynamicExists, email);
    }

    internal async Task TeacherPhoneCanNotBeDuplicated(string phone, int? teacherId = null)
    {
        phone = phone.Trim();
        var teacher = await teacherDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Phone) == PostgresqlFunctions.TrLower(phone), enableTracking: false);
        if (teacherId == null && teacher != null) throw new BusinessException(Strings.DynamicExists, phone);
        if (teacherId != null && teacher != null && teacher.Id != teacherId) throw new BusinessException(Strings.DynamicExists, phone);
    }

    internal static Task AssignClassRoomShouldBeRecordInDatabase(IList<int> ids, IEnumerable<ClassRoom> classRooms)
    {
        foreach (var id in ids)
            if (!classRooms.Any(x => x.Id == id))
                throw new BusinessException(Strings.WrongClassRoomIds);
        return Task.CompletedTask;
    }
}