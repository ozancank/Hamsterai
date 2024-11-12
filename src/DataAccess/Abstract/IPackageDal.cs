namespace DataAccess.Abstract;

public interface IPackageDal : ISyncRepository<Package>, IAsyncRepository<Package>
{
}