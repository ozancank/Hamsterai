using Business.Features.Lessons.Rules;
using Business.Features.Packages.Models;
using Business.Features.Packages.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Packages.Commands;

public class AddPackageCommand : IRequest<GetPackageModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddPackageModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddPackageCommandHandler(IMapper mapper,
                                      ILessonDal lessonDal,
                                      IPackageDal packageDal,
                                      ICommonService commonService,
                                      IRPackageLessonDal packageLessonDal,
                                      PackageRules packageRules) : IRequestHandler<AddPackageCommand, GetPackageModel>
{
    public async Task<GetPackageModel> Handle(AddPackageCommand request, CancellationToken cancellationToken)
    {
        await packageRules.PackageNameCanNotBeDuplicated(request.Model.Name!);
        var date = DateTime.Now;

        var package = mapper.Map<Package>(request.Model);
        package.Id = await packageDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);
        package.IsActive = true;
        package.CreateUser = package.UpdateUser = commonService.HttpUserId;
        package.CreateDate = package.UpdateDate = date;

        List<RPackageLesson> packageLessons = [];

        if (request.Model.LessonIds != null && request.Model.LessonIds.Count != 0)
        {
            var lessons = await lessonDal.GetListAsync(
                predicate: x => x.IsActive,
                enableTracking: false,
                cancellationToken: cancellationToken);
            await LessonRules.LessonShouldBeRecordInDatabase(request.Model.LessonIds, lessons);
            var entities = new List<RPackageLesson>();

            foreach (var id in request.Model.LessonIds!)
            {
                entities.Add(new RPackageLesson
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateUser = commonService.HttpUserId,
                    CreateDate = date,
                    UpdateUser = commonService.HttpUserId,
                    UpdateDate = date,
                    PackageId = package.Id,
                    LessonId = id,
                });
            }
        }

        var result = await packageDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await packageDal.AddAsyncCallback(package, cancellationToken: cancellationToken);
            await packageLessonDal.AddRangeAsync(packageLessons, cancellationToken: cancellationToken);
            var result = mapper.Map<GetPackageModel>(added);
            return result;
        }, cancellationToken: cancellationToken);

        return result;
    }
}

public class AddPackageCommandValidator : AbstractValidator<GetPackageModel>
{
    public AddPackageCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);
    }
}