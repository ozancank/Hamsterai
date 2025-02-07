using DataAccess.EF;
using DataAccess.EF.Concrete.Core;

namespace Application.Features.Packages.Rules;

public class PackageRules(IPackageDal packageDal) : IBusinessRule
{
    internal static Task PackageShouldExists(object? model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }

    internal static Task PackageShouldExistsAndActive(Package? package)
    {
        PackageShouldExists(package);
        if (!package!.IsActive) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }

    internal static Task PackageShouldExists(bool isExists)
    {
        if (!isExists) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }

    internal async Task PackageShouldExistsById(short id)
    {
        var entity = await packageDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        await PackageShouldExists(entity);
    }

    internal async Task PackageShouldExistsAndActiveById(short id)
    {
        var entity = await packageDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        await PackageShouldExists(entity);
        if (!entity) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
    }

    internal async Task PackageShouldNotExistsByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var entity = await packageDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name), enableTracking: false);
        await PackageShouldExists(entity);
    }

    internal async Task PackageShouldNotExistsAndActiveByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var entity = await packageDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name) && x.IsActive, enableTracking: false);
        await PackageShouldExists(entity);
    }

    internal async Task PackageNameAndPeriodCanNotBeDuplicated(string name, PaymentRenewalPeriod period, short? packageId = null)
    {
        var entity = await packageDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name) && x.PaymentRenewalPeriod == period, enableTracking: false);
        if (packageId == null && entity != null) throw new BusinessException(Strings.DynamicExists, name);
        if (packageId != null && entity != null && entity.Id != packageId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task PackageShouldBeRecordInDatabase(IEnumerable<short> ids)
    {
        var entity = await packageDal.GetListAsync(predicate: x => x.IsActive, selector: x => x.Id, enableTracking: false);
        foreach (var id in ids)
            if (!entity.Any(x => x == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
    }

    internal static Task PackageShouldBeRecordInDatabase(IEnumerable<short> ids, IEnumerable<Package> packages)
    {
        foreach (var id in ids)
            if (!packages.Any(x => x.Id == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }

    internal async Task PackageShouldExistsAndActiveByIds(List<short> packageIds)
    {
        var userss = !packageIds.Except(await packageDal.Query().AsNoTracking().Where(x => x.IsActive).Select(x => x.Id).ToListAsync()).Any();
        if (!userss) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.Package);
    }

    internal static void AdditionalPackageShouldBeWithBasePackage(DateTime date, List<(bool, PackageUser)> packageUsers, List<Package> packages, List<PackageUser> packageUserList)
    {
        if (!packageUserList.Any(x => x.EndDate > date && packages.Any(p => p.Id == x.PackageId && p.Type == PackageType.Base))
         && !packageUsers.Any(x => packages.Any(p => p.Id == x.Item2.PackageId && p.Type == PackageType.Base)))
            throw new BusinessException(Strings.AdditionalPackageShouldBeWithBasePackage);
    }
}