namespace DataAccess.Abstract;

public interface ITeacherDal : ISyncRepository<Teacher>, IAsyncRepository<Teacher>
{
}
