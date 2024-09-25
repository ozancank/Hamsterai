namespace Business.Services.CommonService;

public interface ICommonService : IBusinessService
{
    public long HttpUserId { get; }
    public UserTypes HttpUserType { get; }
    public int? HttpSchoolId { get; }

    Task<string> PictureConvert(string base64, string fileName, string folder);
}