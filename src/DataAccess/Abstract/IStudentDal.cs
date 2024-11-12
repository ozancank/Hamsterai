namespace DataAccess.Abstract;

public interface IStudentDal : ISyncRepository<Student>, IAsyncRepository<Student>
{
}