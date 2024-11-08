using Application.Features.Packages.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Packages.Commands.Packages;

public class PassivePackageCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public short Id { get; set; }

    public UserTypes[] Roles { get; } = [UserTypes.Administator];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class PassivePackageCommandHandler(IPackageDal packageDal,
                                          ICommonService commonService) : IRequestHandler<PassivePackageCommand, bool>
{
    public async Task<bool> Handle(PassivePackageCommand request, CancellationToken cancellationToken)
    {
        var entity = await packageDal.GetAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        await PackageRules.PackageShouldExists(entity);

        entity.UpdateUser = commonService.HttpUserId;
        entity.UpdateDate = DateTime.Now;
        entity.IsActive = false;

        await packageDal.UpdateAsync(entity, cancellationToken: cancellationToken);
        return true;
    }
}