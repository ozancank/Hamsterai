namespace DataAccess.Abstract;

public interface IPackageCategoryDal : ISyncRepository<PackageCategory>, IAsyncRepository<PackageCategory>
{
}