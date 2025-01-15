using Application.Features.Lessons.Models.Lessons;
using DataAccess.EF;

namespace Application.Features.Lessons.Rules;

public class LessonRules(ILessonDal lessonDal) : IBusinessRule
{
    internal static Task LessonShouldExists(object? model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
        return Task.CompletedTask;
    }

    internal static Task LessonShouldExists(GetLessonModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
        return Task.CompletedTask;
    }

    internal static Task LessonShouldExists(Lesson Lesson)
    {
        if (Lesson == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
        return Task.CompletedTask;
    }

    internal async Task LessonShouldExistsById(short id)
    {
        var lesson = await lessonDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (!lesson) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
    }

    internal async Task LessonShouldExistsAndActive(short id)
    {
        var lesson = await lessonDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        if (!lesson) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
    }

    internal async Task LessonShouldNotExistsByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var control = await lessonDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name), enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task LessonNameCanNotBeDuplicated(string name, short? lessonId = null)
    {
        var lesson = await lessonDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name));
        if (lessonId == null && lesson != null) throw new BusinessException(Strings.DynamicExists, name);
        if (lessonId != null && lesson != null && lesson.Id != lessonId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal static Task LessonShouldBeRecordInDatabase(IEnumerable<short> ids, IEnumerable<Lesson> lessons)
    {
        foreach (var id in ids)
            if (!lessons.Any(x => x.Id == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
        return Task.CompletedTask;
    }
}