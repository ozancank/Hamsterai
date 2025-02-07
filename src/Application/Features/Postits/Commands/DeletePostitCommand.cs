using Application.Features.Postits.Rules;
using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Postits.Commands;

public class DeletePostitCommand : IRequest<bool>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required Guid Id { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class DeletePostitCommandHandler(IPostitDal postitDal,
                                        ICommonService commonService) : IRequestHandler<DeletePostitCommand, bool>
{
    public async Task<bool> Handle(DeletePostitCommand request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;

        var postit = await postitDal.GetAsync(predicate: x => x.CreateUser == userId && x.Id == request.Id, cancellationToken: cancellationToken);

        await PostitRules.PostitShouldExists(postit);

        if (postit.PictureFileName.IsNotEmpty())
        {
            var filePath = Path.Combine(AppOptions.PostitPictureFolderPath, postit.PictureFileName!);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        await postitDal.DeleteAsync(postit, cancellationToken: cancellationToken);
        return true;
    }
}