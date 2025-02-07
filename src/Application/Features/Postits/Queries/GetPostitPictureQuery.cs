using Application.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Application.Features.Postits.Queries;

public sealed class GetPostitPictureQuery : IRequest<MemoryStream?>, ISecuredRequest<UserTypes>
{
    public Guid PostitId { get; set; }

    public UserTypes[] Roles { get; } = [];
    public bool AllowByPass => false;
}

public sealed class GetPostitPictureQueryHandler(IPostitDal postitDal,
                                              ICommonService commonService) : IRequestHandler<GetPostitPictureQuery, MemoryStream?>
{
    public async Task<MemoryStream?> Handle(GetPostitPictureQuery request, CancellationToken cancellationToken)
    {
        var userId = commonService.HttpUserId;

        var postit = await postitDal.GetAsync(
            enableTracking: false,
            predicate: x => x.CreateUser == userId && x.Id == request.PostitId,
            cancellationToken: cancellationToken);

        if (postit == null) return null;

        var extension = Path.GetExtension(postit.PictureFileName);
        var fileName = $"PT_{userId}_{postit.LessonId}_{postit.Id}{extension}";
        var filePath = Path.Combine(AppOptions.PostitPictureFolderPath, fileName);

        if (!File.Exists(filePath)) return null;

        var memory = new MemoryStream();
        await using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory, cancellationToken);
        }
        memory.Position = 0;

        return memory;
    }
}