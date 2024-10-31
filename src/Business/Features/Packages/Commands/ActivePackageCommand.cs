using Business.Features.Packages.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Business.Features.Packages.Commands;

public class ActivePackageCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public byte Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public string[] HidePropertyNames { get; } = [];
}

public class ActivePackageCommandHandler(IPackageDal packageDal,
                                         ICommonService commonService) : IRequestHandler<ActivePackageCommand, bool>
{
    public async Task<bool> Handle(ActivePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await packageDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await PackageRules.PackageShouldExists(entity);

        entity.UpdateUser = commonService.HttpUserId;
        entity.UpdateDate = DateTime.Now;
        entity.IsActive = true;

        await packageDal.UpdateAsync(entity, cancellationToken: cancellationToken);
        return true;
    }
}