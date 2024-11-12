using Application.Features.Lessons.Rules;
using Application.Features.Packages.Models.Packages;
using Application.Features.Packages.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Packages.Commands.Packages;

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
        await packageRules.PackageNameAndPeriodCanNotBeDuplicated(request.Model.Name!, request.Model.PaymentRenewalPeriod);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;
        var id = await packageDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);

        string? fileName = null;
        if (request.Model.PictureFile != null)
        {
            fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(request.Model.PictureFile.FileName)}";
            var filePath = Path.Combine(AppOptions.PackagePictureFolderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Model.PictureFile.CopyToAsync(stream, cancellationToken);
        }

        var package = mapper.Map<Package>(request.Model);
        package.Id = id;
        package.IsActive = true;
        package.CreateUser = package.UpdateUser = userId;
        package.CreateDate = package.UpdateDate = date;
        package.PictureUrl = fileName;
        package.Slug = package.Name.ToSlug();

        if (package.OldAmount != null && package.OldAmount > 0)
        {
            var oldAmount = package.OldAmount.Value;
            var unitOldAmount = (oldAmount / (1.0 + package.TaxRatio / 100.0)).RoundDouble();
            var taxOldAmount = (oldAmount - unitOldAmount).RoundDouble();
            package.UnitOldPrice = unitOldAmount;
            package.TaxOldAmount = taxOldAmount;
        }
        else
        {
            package.UnitOldPrice = null;
            package.TaxOldAmount = null;
        }

        var amount = package.Amount;
        var unitAmount = (amount / (1.0 + package.TaxRatio / 100.0)).RoundDouble();
        var taxAmount = (amount - unitAmount).RoundDouble();
        package.UnitPrice = unitAmount;
        package.TaxAmount = taxAmount;

        List<RPackageLesson> packageLessons = [];

        if (request.Model.LessonIds != null && request.Model.LessonIds.Count != 0)
        {
            var lessons = await lessonDal.GetListAsync(
                predicate: x => x.IsActive,
                enableTracking: false,
                cancellationToken: cancellationToken);
            await LessonRules.LessonShouldBeRecordInDatabase(request.Model.LessonIds, lessons);

            foreach (var lessonId in request.Model.LessonIds!)
            {
                packageLessons.Add(new RPackageLesson
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    CreateUser = userId,
                    CreateDate = date,
                    UpdateUser = userId,
                    UpdateDate = date,
                    PackageId = package.Id,
                    LessonId = lessonId,
                });
            }
        }

        await packageDal.ExecuteWithTransactionAsync(async () =>
        {
            var added = await packageDal.AddAsyncCallback(package, cancellationToken: cancellationToken);
            await packageLessonDal.AddRangeAsync(packageLessons, cancellationToken: cancellationToken);
        }, cancellationToken: cancellationToken);

        var result = await packageDal.GetAsyncAutoMapper<GetPackageModel>(
            predicate: x => x.Id == package.Id,
            enableTracking: false,
            include: x => x.Include(u => u.PackageCategory).Include(u => u.RPackageLessons).ThenInclude(u => u.Lesson),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AddPackageCommandValidator : AbstractValidator<AddPackageCommand>
{
    public AddPackageCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);

        RuleFor(x => x.Model.CategoryId).InclusiveBetween((byte)0, byte.MaxValue).WithMessage(Strings.DynamicBetween, [$"{Strings.Main} {Strings.Category}", "0", "255"]);

        RuleFor(x => x.Model.QuestionCredit).GreaterThanOrEqualTo(0).WithMessage(Strings.DynamicGratherThanOrEqual, [$"{Strings.Question} {Strings.OfCount}", "0"]);
    }
}