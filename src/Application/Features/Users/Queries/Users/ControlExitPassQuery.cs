using Application.Features.Users.Models.Password;
using Application.Services.CommonService;
using DataAccess.Abstract.Core;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Users.Queries.Users;

public class ControlExitPassQuery : IRequest<bool>, ISecuredRequest<UserTypes>
{
    public required UpdateExitPasswordModel Model { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public class ControlExitPassQueryHandler(IUserDal userDal,
                                         ICommonService commonService) : IRequestHandler<ControlExitPassQuery, bool>
{
    public async Task<bool> Handle(ControlExitPassQuery request, CancellationToken cancellationToken)
    {
        var pass = await userDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == commonService.HttpUserId,
            selector: x => x.ExitPassword,
            cancellationToken: cancellationToken);

        if (pass.IsEmpty()) return true;

        var array = AppOptions.ExitPassKey.Split(";;");
        var key = array[0];
        var iv = array[1];

        var encryptedText = CryptographyTools.EncryptWithAes256(request.Model.Password?.Trim(), key, iv);

        return pass == encryptedText;
    }
}