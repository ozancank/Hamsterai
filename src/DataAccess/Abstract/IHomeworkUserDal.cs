namespace DataAccess.Abstract;

public interface IHomeworkUserDal : ISyncRepository<HomeworkUser>, IAsyncRepository<HomeworkUser>
{
}