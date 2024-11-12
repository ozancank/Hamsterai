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

    Task<string> PictureConvert(string? base64, string? fileName, string? folder);

    Task<string> TextToImage(string? text, string? fileName, string? folder, IImageEncoder? imageEncoder = null);

    Task<string> ImageToBase64(string? path);

    void ThrowErrorTry(Exception exception);

    List<string> GetLogs(PageRequest pageRequest, bool onlyError);

    Dictionary<string, Dictionary<string, int>> GetEnums();

    Dictionary<string, List<AppStatics.PropertyDto>> GetEntities();
}