using Business.Features.Lessons.Rules;
using Business.Features.Packages.Models;
using Business.Features.Packages.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Packages.Commands;

public class AddLessonInPackageCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddLessonInPackageModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddLessonInPackageCommandHandler(ILessonDal lessonDal,
                                              IPackageDal packageDal,
                                              IRPackageLessonDal packageLessonDal,
                                              ICommonService commonService,
                                              PackageRules packageRules) : IRequestHandler<AddLessonInPackageCommand, bool>
{
    public async Task<bool> Handle(AddLessonInPackageCommand request, CancellationToken cancellationToken)
    {
        await packageRules.PackageShouldExistsAndActiveById(request.Model.PackageId);

        var entity = await packageDal.GetAsync(
            predicate: x => x.Id == request.Model.PackageId,
            include: x => x.Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            cancellationToken: cancellationToken);

        await packageLessonDal.DeleteRangeAsync(entity.RPackageLessons, cancellationToken);

        var date = DateTime.Now;

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
                    PackageId = request.Model.PackageId,
                    LessonId = id,
                });
            }

            await packageLessonDal.AddRangeAsync(entities, cancellationToken: cancellationToken);
        }
        return true;
    }
}

public class AddLessonInPackageValidator : AbstractValidator<AddLessonInPackageModel>
{
    public AddLessonInPackageValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.PackageId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Package]);
        RuleFor(x => x.PackageId).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Package, "1", "255"]);

        RuleFor(x => x.LessonIds).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Lesson]);
        RuleForEach(x => x.LessonIds).InclusiveBetween((byte)1, (byte)255).WithMessage(Strings.DynamicBetween, [Strings.Lesson, "1", "255"]);
    }
}