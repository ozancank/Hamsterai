using Application.Features.Packages.Models.PackageCategories;
using Application.Features.Packages.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Packages.Commands.PackageCategories;

public class AddPackageCategoryCommand : IRequest<GetPackageCategoryModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddPackageCategoryModel Model { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddPackageCategoryCommandHandler(IMapper mapper,
                                              IPackageCategoryDal packageCategoryDal,
                                              ICommonService commonService,
                                              PackageCategoryRules categoryRules) : IRequestHandler<AddPackageCategoryCommand, GetPackageCategoryModel>
{
    public async Task<GetPackageCategoryModel> Handle(AddPackageCategoryCommand request, CancellationToken cancellationToken)
    {
        await categoryRules.PackageCategoryNameAndParentIdCanNotBeDuplicated(request.Model.Name!, request.Model.ParentId);
        if (request.Model.ParentId > 0)
            await categoryRules.PackageCategoryShouldExistsById(request.Model.ParentId);

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;
        var id = await packageCategoryDal.GetNextPrimaryKeyAsync(x => x.Id, cancellationToken: cancellationToken);

        string? fileName = null;
        if (request.Model.PictureFile != null)
        {
            fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(request.Model.PictureFile.FileName)}";
            var filePath = Path.Combine(AppOptions.PackagePictureFolderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.Model.PictureFile.CopyToAsync(stream, cancellationToken);
        }

        var category = mapper.Map<PackageCategory>(request.Model);
        category.Id = id;
        category.IsActive = true;
        category.CreateUser = category.UpdateUser = userId;
        category.CreateDate = category.UpdateDate = date;
        category.PictureUrl = fileName;
        category.Slug = category.Name.ToSlug();

        var added = await packageCategoryDal.AddAsyncCallback(category, cancellationToken: cancellationToken);
        var result = mapper.Map<GetPackageCategoryModel>(added);

        return result;
    }
}

public class AddPackageCategoryCommandValidator : AbstractValidator<AddPackageCategoryCommand>
{
    public AddPackageCategoryCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.Name).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Name]);
        RuleFor(x => x.Model.Name).MinimumLength(2).WithMessage(Strings.DynamicMinLength, [Strings.Name, "2"]);
        RuleFor(x => x.Model.Name).MaximumLength(50).WithMessage(Strings.DynamicMaxLength, [Strings.Name, "50"]);

        RuleFor(x => x.Model.ParentId).InclusiveBetween((byte)0, byte.MaxValue).WithMessage(Strings.DynamicBetween, [$"{Strings.Main} {Strings.Category}", "0", "255"]);
    }
}