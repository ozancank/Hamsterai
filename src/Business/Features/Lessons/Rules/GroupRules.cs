using Business.Features.Lessons.Models.Groups;

namespace Business.Features.Lessons.Rules;

public class GroupRules(IGroupDal groupDal) : IBusinessRule
{
    internal static Task GroupShouldExists(GetGroupModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Group);
        return Task.CompletedTask;
    }

    internal static Task GroupShouldExists(Package group)
    {
        if (group == null) throw new BusinessException(Strings.DynamicNotFound, Strings.Group);
        return Task.CompletedTask;
    }

    internal async Task GroupShouldExistsById(byte id)
    {
        var group = await groupDal.IsExistsAsync(predicate: x => x.Id == id, enableTracking: false);
        if (!group) throw new BusinessException(Strings.DynamicNotFound, Strings.Group);
    }

    internal async Task GroupShouldExistsAndActiveById(byte id)
    {
        var group = await groupDal.IsExistsAsync(predicate: x => x.Id == id && x.IsActive, enableTracking: false);
        if (!group) throw new BusinessException(Strings.DynamicNotFound, Strings.Group);
    }

    internal async Task GroupShouldNotExistsByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var control = await groupDal.IsExistsAsync(predicate: x => x.Name == name, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task GroupShouldNotExistsAndActiveByName(string name)
    {
        if (name == null) throw new BusinessException($"{Strings.InvalidValue} : {nameof(name)}");
        var control = await groupDal.IsExistsAsync(predicate: x => x.Name == name && x.IsActive, enableTracking: false);
        if (control) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task GroupNameCanNotBeDuplicated(string name, byte? groupId = null)
    {
        name = name.Trim().ToLower();
        var university = await groupDal.GetAsync(predicate: x => x.Name == name, enableTracking: false);
        if (groupId == null && university != null) throw new BusinessException(Strings.DynamicExists, name);
        if (groupId != null && university != null && university.Id != groupId) throw new BusinessException(Strings.DynamicExists, name);
    }

    internal async Task GroupShouldBeRecordInDatabase(IEnumerable<byte> ids)
    {
        var groups = await groupDal.GetListAsync(predicate: x => x.IsActive, selector: x => x.Id, enableTracking: false);
        foreach (var id in ids)
            if (!groups.Any(x => x == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Group);        
    }

    internal static Task GroupShouldBeRecordInDatabase(IEnumerable<byte> ids, IEnumerable<Package> groups)
    {
        foreach (var id in ids)
            if (!groups.Any(x => x.Id == id))
                throw new BusinessException(Strings.DynamicNotFound, Strings.Group);
        return Task.CompletedTask;
    }
}