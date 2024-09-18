using Business.Features.Lessons.Models.Lessons;

namespace Business.Features.Lessons.Rules;

public class LessonRules(ILessonDal lessonDal) : IBusinessRule
{
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

    internal async Task LessonShouldExistsById(byte id)
    {
        var lesson = await lessonDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (!lesson) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
    }

    internal async Task LessonShouldExistsAndActiveById(byte id)
    {
        var lesson = await lessonDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        if (!lesson) throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
    }

    internal async Task LessonShouldNotExistsByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var control = await lessonDal.IsExistsAsync(predicate: x => x.Name == name, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task LessonNameCanNotBeDuplicated(string name, byte? lessonId = null)
    {
        name = name.Trim().ToLower();
        var university = await lessonDal.GetAsync(predicate: x => x.Name == name, enableTracking: false);
        if (lessonId == null && university != null) throw new BusinessException(Strings.DynamicExists, name);
        if (lessonId != null && university != null && university.Id != lessonId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal static Task LessonShouldBeRecordInDatabase(IEnumerable<byte> ids, IEnumerable<Lesson> lessons)
    {
        foreach (var id in ids)
            if (!lessons.Any(x => x.Id == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Lesson);
        return Task.CompletedTask;
    }
}