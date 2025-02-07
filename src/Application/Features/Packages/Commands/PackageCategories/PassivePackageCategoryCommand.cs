using Application.Features.Packages.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Packages.Commands.PackageCategories;

public class PassivePackageCategoryCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassivePackageCategoryCommandHandler(IPackageCategoryDal packageCategoryDal,
                                                  ICommonService commonService) : IRequestHandler<PassivePackageCategoryCommand, bool>
{
    public async Task<bool> Handle(PassivePackageCategoryCommand request, CancellationToken cancellationToken)
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