using Application.Features.Packages.Rules;
using Application.Features.Schools.Models.ClassRooms;
using Application.Features.Schools.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Schools.Commands.ClassRooms;

public class AddClassRoomCommand : IRequest<GetClassRoomModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddClassRoomModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddClassRoomCommandHandler(IMapper mapper,
                                        IClassRoomDal classRoomDal,
                                        ICommonService commonService,
                                        ClassRoomRules classRoomRules,
                                        PackageRules packageRules) : IRequestHandler<AddClassRoomCommand, GetClassRoomModel>
{
    public async Task<GetClassRoomModel> Handle(AddClassRoomCommand request, CancellationToken cancellationToken)
    {
        request.Model.Branch = request.Model.Branch!.Trim().ToUpper();
        var schoolId = commonService.HttpSchoolId ?? 0;
        var userId = commonService.HttpSchoolId;
        var date = DateTime.Now;

        await classRoomRules.ClassRoomNoAndBranchAndSchoolIdCanNotBeDuplicated(request.Model.No, request.Model.Branch, schoolId);
        await packageRules.PackageShouldExistsAndActiveById(request.Model.PackageId);

        var classRoom = mapper.Map<ClassRoom>(request.Model);
        classRoom.Id = await classRoomDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        classRoom.IsActive = true;
        classRoom.CreateUser = userId!.Value;
        classRoom.CreateDate = date;
        classRoom.UpdateUser = userId.Value;
        classRoom.UpdateDate = date;
        classRoom.SchoolId = schoolId;

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

        RuleFor(x => x.Model.No).GreaterThan((short)0).WithMessage(Strings.DynamicGreaterThan, [$"{Strings.ClassRoom} {Strings.OfNumber}", "0"]);

        RuleFor(x => x.Model.Branch).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.ClassRoom} {Strings.OfBranch}"]);
        RuleFor(x => x.Model.Branch).MinimumLength(1).WithMessage(Strings.DynamicMinLength, [$"{Strings.ClassRoom} {Strings.OfBranch}", "1"]);
        RuleFor(x => x.Model.Branch).MaximumLength(10).WithMessage(Strings.DynamicMaxLength, [$"{Strings.ClassRoom} {Strings.OfBranch}", "10"]);
    }
}