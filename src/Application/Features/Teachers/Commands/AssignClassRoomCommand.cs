using Application.Features.Teachers.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Teachers.Commands;

public class AssignClassRoomCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public int Id { get; set; }
    public List<int> ClassRoomIds { get; set; } = [];

    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AssignClassRoomCommandHandler(ITeacherDal teacherDal,
                                           IClassRoomDal classRoomDal,
                                           IRTeacherClassRoomDal teacherClassRoomDal,
                                           ICommonService commonService) : IRequestHandler<AssignClassRoomCommand, bool>
{
    public async Task<bool> Handle(AssignClassRoomCommand request, CancellationToken cancellationToken)
    {
        var teacher = await teacherDal.GetAsync(
            predicate: x => x.Id == request.Id,
            include: x => x.Include(u => u.RTeacherClassRooms).ThenInclude(u => u.ClassRoom),
            cancellationToken: cancellationToken);

        await TeacherRules.TeacherShouldExists(request.Id);

        await teacherClassRoomDal.DeleteRangeAsync(teacher.RTeacherClassRooms, cancellationToken);

        if (request.ClassRoomIds != null && request.ClassRoomIds.Count != 0)
        {
            var classRooms = await classRoomDal.GetListAsync(predicate: x => x.SchoolId == teacher.SchoolId, enableTracking: false, cancellationToken: cancellationToken);
            await TeacherRules.AssignClassRoomShouldBeRecordInDatabase(request.ClassRoomIds, classRooms);
            var teacherClassRooms = new List<RTeacherClassRoom>();

            var userId = commonService.HttpUserId;
            var date = DateTime.Now;

            foreach (var id in request.ClassRoomIds)
            {
                teacherClassRooms.Add(new()
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateUser = userId,
                    CreateDate = DateTime.Now,
                    UpdateUser = userId,
                    UpdateDate = date,
                    TeacherId = request.Id,
                    ClassRoomId = id
                });
            }

            await teacherClassRoomDal.AddRangeAsync(teacherClassRooms, cancellationToken: cancellationToken);
        }

        return true;
    }
}

public class AssignClassRoomCommandHandlerHandlerValidator : AbstractValidator<AssignClassRoomCommand>
{
    public AssignClassRoomCommandHandlerHandlerValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.ClassRoomIds).NotNull().WithMessage(Strings.ClassRoomsNotNull);
    }
}