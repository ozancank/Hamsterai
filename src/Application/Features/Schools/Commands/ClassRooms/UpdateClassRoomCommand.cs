﻿using Application.Features.Packages.Rules;
using Application.Features.Schools.Models.ClassRooms;
using Application.Features.Schools.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Schools.Commands.ClassRooms;

public class UpdateClassRoomCommand : IRequest<GetClassRoomModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdateClassRoomModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.School];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdateClassRoomCommandHandler(IMapper mapper,
                                           IClassRoomDal classRoomDal,
                                           ICommonService commonService,
                                           ClassRoomRules classRoomRules,
                                           PackageRules packageRules) : IRequestHandler<UpdateClassRoomCommand, GetClassRoomModel>
{
    public async Task<GetClassRoomModel> Handle(UpdateClassRoomCommand request, CancellationToken cancellationToken)
    {
        request.Model.Branch = request.Model.Branch!.Trim().ToUpper();
        var schoolId = commonService.HttpSchoolId ?? 0;

        var classRoom = await classRoomDal.GetAsync(x => x.Id == request.Model.Id && x.SchoolId == schoolId, cancellationToken: cancellationToken);

        await classRoomRules.ClassRoomNoAndBranchAndSchoolIdCanNotBeDuplicated(request.Model.No, request.Model.Branch, schoolId, classRoom.Id);
        await packageRules.PackageShouldExistsAndActiveById(request.Model.PackageId);

        mapper.Map(request.Model, classRoom);
        classRoom.UpdateUser = commonService.HttpUserId;
        classRoom.UpdateDate = DateTime.Now;
        schoolId = classRoom.SchoolId;

        var updated = await classRoomDal.UpdateAsyncCallback(classRoom, cancellationToken: cancellationToken);
        var result = mapper.Map<GetClassRoomModel>(updated);
        return result;
    }
}

public class UpdateClassRoomCommandValidator : AbstractValidator<UpdateClassRoomCommand>
{
    public UpdateClassRoomCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.No).GreaterThan((short)0).WithMessage(Strings.DynamicGreaterThan, [$"{Strings.ClassRoom} {Strings.OfNumber}", "0"]);

        RuleFor(x => x.Model.Branch).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [$"{Strings.ClassRoom} {Strings.OfBranch}"]);
        RuleFor(x => x.Model.Branch).MinimumLength(1).WithMessage(Strings.DynamicMinLength, [$"{Strings.ClassRoom} {Strings.OfBranch}", "1"]);
        RuleFor(x => x.Model.Branch).MaximumLength(10).WithMessage(Strings.DynamicMaxLength, [$"{Strings.ClassRoom} {Strings.OfBranch}", "10"]);

        RuleFor(x => x.Model.SchoolId).GreaterThan(0).WithMessage(Strings.DynamicGreaterThan, [Strings.School, "0"]);
    }
}