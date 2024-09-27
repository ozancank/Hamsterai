using Business.Features.Schools.Models.ClassRooms;

namespace Business.Features.Schools.Rules;

public class ClassRoomRules(IClassRoomDal classRoomDal) : IBusinessRule
{
    internal static Task ClassRoomShouldExists(GetClassRoomModel model)
    {
        if (model == null) throw new BusinessException(Strings.DynamicNotFound, Strings.ClassRoom);
        return Task.CompletedTask;
    }

    internal static Task ClassRoomShouldExists(ClassRoom entity)
    {
        if (entity == null) throw new BusinessException(Strings.DynamicNotFound, Strings.ClassRoom);
        return Task.CompletedTask;
    }

    internal async Task ClassRoomNoAndBranchAndSchoolIdCanNotBeDuplicated(short no, string branch, int schoolId, int? classRoomId = null)
    {
        branch = branch.Trim().ToUpper();
        var classRoom = await classRoomDal.GetAsync(predicate: x => x.No == no && x.Branch == branch && x.SchoolId == schoolId, enableTracking: false);
        if (classRoomId == null && classRoom != null) throw new BusinessException(Strings.DynamicExists, Strings.ClassRoom);
        if (classRoomId != null && classRoom != null && classRoom.Id != classRoomId) throw new BusinessException(Strings.DynamicExists, Strings.ClassRoom);
    }
}