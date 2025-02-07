namespace DataAccess.Abstract;

public interface ISchoolDal : ISyncRepository<School>, IAsyncRepository<School>
{
}