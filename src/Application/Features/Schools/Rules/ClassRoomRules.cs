using Application.Features.Schools.Models.ClassRooms;
using DataAccess.EF;

namespace Application.Features.Schools.Rules;

public class ClassRoomRules(IClassRoomDal classRoomDal) : IBusinessRule
{
    internal static Task ClassRoomShouldExists(GetClassRoomModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.ClassRoom);
        return Task.CompletedTask;
    }

    internal static Task ClassRoomShouldExistsAndActive(GetClassRoomModel model)
    {
        ClassRoomShouldExists(model);
        if (!model.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.ClassRoom);
        return Task.CompletedTask;
    }

    internal static Task ClassRoomShouldExists(ClassRoom entity)
    {
        if (entity == null) throw new BusinessException(Strings.DynamicNotFound, Strings.ClassRoom);
        return Task.CompletedTask;
    }

    internal static Task ClassRoomShouldExistsAndActive(ClassRoom entity)
    {
        ClassRoomShouldExists(entity);
        if (!entity.IsActive) throw new BusinessException(Strings.DynamicNotFoundOrActive, Strings.ClassRoom);
        return Task.CompletedTask;
    }

    internal async Task ClassRoomShouldExistsAndActiveById(int id)
    {
        var classRoom = await classRoomDal.GetAsync(predicate: x => x.Id == id, enableTracking: false);
        await ClassRoomShouldExistsAndActive(classRoom);
    }

    internal async Task ClassRoomNoAndBranchAndSchoolIdCanNotBeDuplicated(short no, string branch, int schoolId, int? classRoomId = null)
    {
        branch = branch.Trim().ToUpper();
        var classRoom = await classRoomDal.GetAsync(predicate: x => x.No == no && PostgresqlFunctions.TrLower(x.Branch) == PostgresqlFunctions.TrLower(branch) && x.SchoolId == schoolId, enableTracking: false);
        if (classRoomId == null && classRoom != null) throw new BusinessException(Strings.DynamicExists, Strings.ClassRoom);
        if (classRoomId != null && classRoom != null && classRoom.Id != classRoomId) throw new BusinessException(Strings.DynamicExists, Strings.ClassRoom);
    }
}