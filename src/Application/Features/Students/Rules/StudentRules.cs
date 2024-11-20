using Application.Features.Students.Models;
using DataAccess.EF;

namespace Application.Features.Students.Rules;

public class StudentRules(IStudentDal studentDal) : IBusinessRule
{
    internal static Task StudentShouldExists(object student)
    {
        if (student == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Student);
        return Task.CompletedTask;
    }

    internal static Task StudentShouldExistsAndActive(GetStudentModel studentModel)
    {
        StudentShouldExists(studentModel);
        if (!studentModel.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Student);
        return Task.CompletedTask;
    }

    internal static Task StudentShouldExistsAndActive(Student student)
    {
        StudentShouldExists(student);
        if (!student.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Student);
        return Task.CompletedTask;
    }

    internal async Task StudentShouldExists(int id)
    {
        var exists = await studentDal.IsExistsAsync(x => x.Id == id, enableTracking: false);
        await StudentShouldExists(exists);
    }

    internal async Task StudentShouldExistsAndActive(int id)
    {
        var student = await studentDal.GetAsync(predicate: x => x.Id == id, enableTracking: false);
        await StudentShouldExistsAndActive(student);
    }

    internal async Task StudentNoCanNotBeDuplicated(string no, int? studentId = null)
    {
        no = no.Trim();
        var student = await studentDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.StudentNo) == PostgresqlFunctions.TrLower(no), enableTracking: false);
        if (studentId == null && student != null) throw new BusinessException(Strings.DynamicExists, no);
        if (studentId != null && student != null && student.Id != studentId) throw new BusinessException(Strings.DynamicExists, no);
    }

    internal async Task StudentEmailCanNotBeDuplicated(string email, int? studentId = null)
    {
        email = email.Trim();
        var student = await studentDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Email) == PostgresqlFunctions.TrLower(email), enableTracking: false);
        if (studentId == null && student != null) throw new BusinessException(Strings.DynamicExists, email);
        if (studentId != null && student != null && student.Id != studentId) throw new BusinessException(Strings.DynamicExists, email);
    }

    internal async Task StudentPhoneCanNotBeDuplicated(string phone, int? studentId = null)
    {
        phone = phone.Trim();
        var student = await studentDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Phone) == PostgresqlFunctions.TrLower(phone), enableTracking: false);
        if (studentId == null && student != null) throw new BusinessException(Strings.DynamicExists, phone);
        if (studentId != null && student != null && student.Id != studentId) throw new BusinessException(Strings.DynamicExists, phone);
    }

    internal async Task StudentsShouldExistsAndActiveByIds(List<int> studentIds)
    {
        var students = !studentIds.Except(await studentDal.Query().AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync()).Any();
        if (!students) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Student);
    }
}