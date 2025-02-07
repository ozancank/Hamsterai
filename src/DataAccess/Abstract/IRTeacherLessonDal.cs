namespace DataAccess.Abstract;

public interface IRTeacherLessonDal : ISyncRepository<RTeacherLesson>, IAsyncRepository<RTeacherLesson>
{
}