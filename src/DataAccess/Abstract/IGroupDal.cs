namespace DataAccess.Abstract;

public interface IGroupDal : ISyncRepository<Package>, IAsyncRepository<Package>
{
}
