namespace DataAccess.Abstract;

public interface ITeacherLessonDal : ISyncRepository<TeacherLesson>, IAsyncRepository<TeacherLesson>
{
}