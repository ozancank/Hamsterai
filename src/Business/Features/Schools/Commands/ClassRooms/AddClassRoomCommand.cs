using Business.Features.Lessons.Rules;
using Business.Features.Schools.Models.ClassRooms;
using Business.Features.Schools.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Schools.Commands.ClassRooms;

public class AddClassRoomCommand : IRequest<GetClassRoomModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddClassRoomModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public string[] HidePropertyNames { get; } = [];
}

public class AddClassRoomCommandHandler(IMapper mapper,
                                        IClassRoomDal classRoomDal,
                                        ICommonService commonService,
                                        ClassRoomRules classRoomRules) : IRequestHandler<AddClassRoomCommand, GetClassRoomModel>
{
    public async Task<GetClassRoomModel> Handle(AddClassRoomCommand request, CancellationToken cancellationToken)
    {
        request.Model.Branch = request.Model.Branch.Trim().ToUpper();

        await classRoomRules.ClassRoomNoAndBranchAndSchoolIdCanNotBeDuplicated(request.Model.No, request.Model.Branch, request.Model.SchoolId);

        var userId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        var classRoom = mapper.Map<ClassRoom>(request.Model);
        classRoom.Id = await classRoomDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);        
        classRoom.IsActive = true;
        classRoom.CreateUser = userId.Value;
        classRoom.CreateDate = date;
        classRoom.UpdateUser = userId.Value;
        classRoom.UpdateDate = date;

        var added = await classRoomDal.AddAsyncCallback(classRoom, cancellationToken: cancellationToken);
        var result = mapper.Map<GetClassRoomModel>(added);
        return result;
    }
}

public class AddClassRoomCommandValidator : AbstractValidator<AddClassRoomCommand>
{
    public AddClassRoomCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.No).GreaterThan((short)0).WithMessage(Strings.DynamicGratherThan, [$"{Strings.ClassRoom} {Strings.OfNumber}", "0"]);

        RuleFor(x => x.Model.Branch).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.ClassRoom} {Strings.OfBranch}"]);
        RuleFor(x => x.Model.Branch).MinimumLength(1).WithMessage(Strings.DynamicMinLength, [$"{Strings.ClassRoom} {Strings.OfBranch}", "1"]);
        RuleFor(x => x.Model.Branch).MaximumLength(10).WithMessage(Strings.DynamicMaxLength, [$"{Strings.ClassRoom} {Strings.OfBranch}", "10"]);

        RuleFor(x => x.Model.SchoolId).GreaterThan(0).WithMessage(Strings.DynamicGratherThan, [Strings.School, "0"]);
    }
}