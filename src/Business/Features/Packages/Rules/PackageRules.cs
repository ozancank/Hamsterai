namespace Business.Features.Packages.Rules;

public class PackageRules(IPackageDal packageDal) : IBusinessRule
{
    internal static Task PackageShouldExists(object model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }

    internal static Task PackageShouldExists(bool isExists)
    {
        if (!isExists) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }

    internal async Task PackageShouldExistsById(byte id)
    {
        var entity = await packageDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        await PackageShouldExists(entity);
    }

    internal async Task PackageShouldExistsAndActiveById(byte id)
    {
        var entity = await packageDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        await PackageShouldExists(entity);
        if (!entity) throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
    }

    internal async Task PackageShouldNotExistsByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var entity = await packageDal.IsExistsAsync(predicate: x => x.Name == name, enableTracking: false);
        await PackageShouldExists(entity);
    }

    internal async Task PackageShouldNotExistsAndActiveByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var entity = await packageDal.IsExistsAsync(predicate: x => x.Name == name && x.IsActive, enableTracking: false);
        await PackageShouldExists(entity);
    }

    internal async Task PackageNameCanNotBeDuplicated(string name, byte? packageId = null)
    {
        name = name.EmptyOrTrim().ToLower();
        var entity = await packageDal.GetAsync(predicate: x => x.Name == name, enableTracking: false);
        if (packageId == null && entity != null) throw new BusinessException(Strings.DynamicExists, name);
        if (packageId != null && entity != null && entity.Id != packageId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task PackageShouldBeRecordInDatabase(IEnumerable<byte> ids)
    {
        var entity = await packageDal.GetListAsync(predicate: x => x.IsActive, selector: x => x.Id, enableTracking: false);
        foreach (var id in ids)
            if (!entity.Any(x => x == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
    }

    internal static Task PackageShouldBeRecordInDatabase(IEnumerable<byte> ids, IEnumerable<Package> packages)
    {
        foreach (var id in ids)
            if (!packages.Any(x => x.Id == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Package);
        return Task.CompletedTask;
    }
}