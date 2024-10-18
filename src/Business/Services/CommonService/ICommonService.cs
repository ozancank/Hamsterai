using SixLabors.ImageSharp.Formats;

namespace Business.Services.CommonService;

public interface ICommonService : IBusinessService
{
    public long HttpUserId { get; }
    public UserTypes HttpUserType { get; }
    public int? HttpSchoolId { get; }
    public int? HttpConnectionId { get; }

    Task<string> PictureConvert(string base64, string fileName, string folder);
    Task<string> TextToImage(string text, string fileName, string folder, IImageEncoder imageEncoder = null);
}