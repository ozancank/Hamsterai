using DataAccess.EF;

namespace Business.Features.Packages.Rules;

public class PackageCategoryRules(IPackageCategoryDal packageCategoryDal) : IBusinessRule
{
    internal static Task PackageCategoryShouldExists(object model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Category);
        return Task.CompletedTask;
    }

    internal static Task PackageCategoryShouldExists(bool isExists)
    {
        if (!isExists) throw new BusinessException(Strings.DynamicNotFound, Strings.Category);
        return Task.CompletedTask;
    }

    internal async Task PackageCategoryShouldExistsById(byte id)
    {
        var entity = await packageCategoryDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        await PackageCategoryShouldExists(entity);
    }

    internal async Task PackageCategoryShouldExistsAndActiveById(byte id)
    {
        var entity = await packageCategoryDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        await PackageCategoryShouldExists(entity);
        if (!entity) throw new BusinessException(Strings.DynamicNotFound, Strings.Category);
    }

    internal async Task PackageCategoryShouldNotExistsByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var entity = await packageCategoryDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name), enableTracking: false);
        await PackageCategoryShouldExists(entity);
    }

    internal async Task PackageCategoryShouldNotExistsAndActiveByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var entity = await packageCategoryDal.IsExistsAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name) && x.IsActive, enableTracking: false);
        await PackageCategoryShouldExists(entity);
    }

    internal async Task PackageCategoryNameAndParentIdCanNotBeDuplicated(string name, byte parentId, byte? packageCategoryId = null)
    {
        var entity = await packageCategoryDal.GetAsync(predicate: x => PostgresqlFunctions.TrLower(x.Name) == PostgresqlFunctions.TrLower(name) && x.ParentId == parentId, enableTracking: false);
        if (packageCategoryId == null && entity != null) throw new BusinessException(Strings.DynamicExists, name);
        if (packageCategoryId != null && entity != null && entity.Id != packageCategoryId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task PackageCategoryShouldBeRecordInDatabase(IEnumerable<byte> ids)
    {
        var entity = await packageCategoryDal.GetListAsync(predicate: x => x.IsActive, selector: x => x.Id, enableTracking: false);
        foreach (var id in ids)
            if (!entity.Any(x => x == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Category);
    }

    internal static Task PackageCategoryShouldBeRecordInDatabase(IEnumerable<byte> ids, IEnumerable<PackageCategory> packageCategorys)
    {
        foreach (var id in ids)
            if (!packageCategorys.Any(x => x.Id == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Category);
        return Task.CompletedTask;
    }
}