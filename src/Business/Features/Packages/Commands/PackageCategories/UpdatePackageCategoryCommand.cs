using Business.Features.Packages.Models.PackageCategories;
using Business.Features.Packages.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Packages.Commands.PackageCategories;

public class UpdatePackageCategoryCommand : IRequest<GetPackageCategoryModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required UpdatePackageCategoryModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class UpdatePackageCategoryCommandHandler(IMapper mapper,
                                                 IPackageCategoryDal packageCategoryDal,
                                                 ICommonService commonService,
                                                 PackageCategoryRules categoryRules) : IRequestHandler<UpdatePackageCategoryCommand, GetPackageCategoryModel>
{
    public async Task<GetPackageCategoryModel> Handle(UpdatePackageCategoryCommand request, CancellationToken cancellationToken)
    {
        var date = DateTime.Now;
        var userId = commonService.HttpUserId;

        var category = await packageCategoryDal.GetAsync(x => x.Id == request.Model.Id, cancellationToken: cancellationToken);

        await PackageCategoryRules.PackageCategoryShouldExists(category);
        await categoryRules.PackageCategoryNameAndParentIdCanNotBeDuplicated(request.Model.Name!, request.Model.ParentId, request.Model.Id);
        if (request.Model.ParentId > 0) await categoryRules.PackageCategoryShouldExistsById(request.Model.ParentId);

        mapper.Map(request.Model, category);
        category.UpdateUser = userId;
        category.UpdateDate = date;
        category.Slug = category.Name.ToSlug();

        if (request.Model.PictureFile != null)
        {
            var fileName = $"{request.Model.Id}_{Guid.NewGuid()}{Path.GetExtension(request.Model.PictureFile.FileName)}";
            var filePath = Path.Combine(AppOptions.PackagePictureFolderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Model.PictureFile.CopyToAsync(stream, cancellationToken);
            category.PictureUrl = fileName;
        }

        var updated = await packageCategoryDal.UpdateAsyncCallback(category, cancellationToken: cancellationToken);
        var result = mapper.Map<GetPackageCategoryModel>(updated);

        return result;
    }
}

public class UpdatePackageCategoryCommandValidator : AbstractValidator<UpdatePackageCategoryCommand>
{
    public UpdatePackageCategoryCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Id).NotEmpty().WithMessage(Strings.IdNotEmpty);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);

        RuleFor(x => x.Model.ParentId).InclusiveBetween((byte)0, byte.MaxValue).WithMessage(Strings.DynamicBetween, [$"{Strings.Main} {Strings.Category}", "0", "255"]);
    }
}