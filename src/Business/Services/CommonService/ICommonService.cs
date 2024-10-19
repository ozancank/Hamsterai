using OCK.Core.Logging;
using SixLabors.ImageSharp.Formats;

namespace Business.Services.CommonService;

public interface ICommonService : IBusinessService
{
    long HttpUserId { get; }
    UserTypes HttpUserType { get; }
    int? HttpSchoolId { get; }
    int? HttpConnectionId { get; }
    byte? HttpGroupId { get; }

    Task<string> PictureConvert(string base64, string fileName, string folder);
    Task<string> TextToImage(string text, string fileName, string folder, IImageEncoder imageEncoder = null);

    void ThrowErrorTry(Exception exception);
    List<string> GetLogs(PageRequest pageRequest, bool onlyError);
}