namespace DataAccess.Abstract;

public interface IPostitDal : ISyncRepository<Postit>, IAsyncRepository<Postit>
{
}