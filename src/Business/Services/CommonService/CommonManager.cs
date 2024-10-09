using Business.Features.Users.Rules;

namespace Business.Services.CommonService;

public class CommonManager(IHttpContextAccessor httpContextAccessor) : ICommonService
{
    public long HttpUserId =>
        Convert.ToInt64(httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    public UserTypes HttpUserType =>
        Enum.TryParse(httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.UserType)?.Value, out UserTypes userType) ? userType : UserTypes.Student;

    public int? HttpSchoolId =>
        int.TryParse(httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.SchoolId)?.Value, out int schoolId) ? schoolId : null;

    public int? HttpConnectionId =>
        int.TryParse(httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.ConnectionId)?.Value, out int connectionId) ? connectionId : null;

    public async Task<string> PictureConvert(string base64, string fileName, string folder)
    {
        if (base64.IsEmpty() || fileName.IsEmpty()) return await Task.FromResult(string.Empty);

        await UserRules.PictureShouldAllowedType(fileName);
        var filePath = Path.Combine(folder, fileName);
        var imageBytes = Convert.FromBase64String(base64);
        await File.WriteAllBytesAsync(filePath, imageBytes);
        return filePath;
    }
}