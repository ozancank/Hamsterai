using SixLabors.ImageSharp.Formats;

namespace Application.Services.CommonService;

public interface ICommonService : IBusinessService
{
    long HttpUserId { get; }
    string HttpUserName { get; }
    UserTypes HttpUserType { get; }
    int? HttpSchoolId { get; }
    int? HttpConnectionId { get; }
    byte? HttpPackageId { get; }
    bool IsByPass { get; }

    Task<string> PictureConvert(string? base64, string? fileName, string? folder, CancellationToken cancellationToken = default);

    Task<string> TextToImage(string? text, string? fileName, string? folder, IImageEncoder? imageEncoder = null, CancellationToken cancellationToken = default);

    Task<string> ImageToBase64(string? path, CancellationToken cancellationToken = default);

    Task<string> ImageToBase64WithResize(string? path, int maxDimension = 512, CancellationToken cancellationToken = default);

    void ThrowErrorTry(Exception exception);

    List<string> GetLogs(PageRequest pageRequest, bool onlyError);

    Dictionary<string, Dictionary<string, int>> GetEnums();

    Dictionary<string, List<AppStatics.PropertyDto>> GetEntities();

    Task<string?> GetUserAIUrl(long userId, CancellationToken cancellationToken = default);

    Task<string?> GetLessonNamesForAI(CancellationToken cancellationToken = default);
}