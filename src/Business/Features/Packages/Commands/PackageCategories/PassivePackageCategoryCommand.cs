using Business.Features.Packages.Commands.Packages;
using Business.Features.Packages.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Packages.Commands.PackageCategories;

public class PassivePackageCategoryCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassivePackageCategoryCommandHandler(IPackageCategoryDal packageCategoryDal,
                                                  ICommonService commonService) : IRequestHandler<PassivePackageCommand, bool>
{
    public async Task<bool> Handle(PassivePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await packageCategoryDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await PackageRules.PackageShouldExists(entity);

        entity.UpdateUser = commonService.HttpUserId;
        entity.UpdateDate = DateTime.Now;
        entity.IsActive = false;

        await packageCategoryDal.UpdateAsync(entity, cancellationToken: cancellationToken);
        return true;
    }
}