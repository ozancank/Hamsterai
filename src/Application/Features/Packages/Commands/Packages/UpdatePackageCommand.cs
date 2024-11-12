using Application.Features.Packages.Models.Packages;
using Application.Features.Packages.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Packages.Commands.Packages;

public class UpdatePackageCommand : IRequest<GetPackageModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdatePackageModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdatePackageCommandHandler(IMapper mapper,
                                         IPackageDal packageDal,
                                         ICommonService commonService,
                                         //ILessonDal lessonDal,
                                         IRPackageLessonDal packageLessonDal,
                                         PackageRules packageRules) : IRequestHandler<UpdatePackageCommand, GetPackageModel>
{
    public async Task<GetPackageModel> Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;
        var userId = commonService.HttpUserId;

        var package = await packageDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await PackageRules.PackageShouldExists(package);
        await packageRules.PackageNameAndPeriodCanNotBeDuplicated(request.Model.Name!, request.Model.PaymentRenewalPeriod, request.Model.Id);

        mapper.Map(request.Model, package);
        package.UpdateUser = userId;
        package.UpdateDate = date;
        package.Slug = package.Name.ToSlug();

        if (request.Model.PictureFile != null)
        {
            var fileName = $"{request.Model.Id}_{Guid.NewGuid()}{Path.GetExtension(request.Model.PictureFile.FileName)}";
            var filePath = Path.Combine(AppOptions.PackagePictureFolderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Model.PictureFile.CopyToAsync(stream, cancellationToken);
            package.PictureUrl = fileName;
        }

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

        var deleteList = await packageLessonDal.GetListAsync(predicate: x => x.PackageId == package.Id, cancellationToken: cancellationToken);

        var packageLessons = request.Model.LessonIds.Select(x => new RPackageLesson
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            CreateUser = userId,
            CreateDate = date,
            UpdateUser = userId,
            UpdateDate = date,
            PackageId = package.Id,
            LessonId = x,
        }).ToList();

        await packageDal.ExecuteWithTransactionAsync(async () =>
        {
            var updated = await packageDal.UpdateAsyncCallback(package, cancellationToken: cancellationToken);
            await packageLessonDal.DeleteRangeAsync(deleteList, cancellationToken: cancellationToken);
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

public class UpdatePackageCommandValidator : AbstractValidator<UpdatePackageCommand>
{
    public UpdatePackageCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);

        RuleFor(x => x.Model.CategoryId).InclusiveBetween((byte)0, byte.MaxValue).WithMessage(Strings.DynamicBetween, [$"{Strings.Main} {Strings.Category}", "0", "255"]);

        RuleFor(x => x.Model.QuestionCredit).GreaterThanOrEqualTo(0).WithMessage(Strings.DynamicGratherThanOrEqual, [$"{Strings.Question} {Strings.OfCount}", "0"]);
    }
}