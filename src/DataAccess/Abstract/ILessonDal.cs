namespace DataAccess.Abstract;

public interface ILessonDal : ISyncRepository<Lesson>, IAsyncRepository<Lesson>
{
}