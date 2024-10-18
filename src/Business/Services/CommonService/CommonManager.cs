using Business.Features.Users.Rules;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Reflection;

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
        if (base64.IsEmpty() || fileName.IsEmpty() || folder.IsEmpty()) return string.Empty;

        await UserRules.PictureShouldAllowedType(fileName);
        var filePath = System.IO.Path.Combine(folder, fileName);
        var imageBytes = Convert.FromBase64String(base64);
        await File.WriteAllBytesAsync(filePath, imageBytes);
        return filePath;
    }

    public async Task<string> TextToImage(string text, string fileName, string folder, IImageEncoder imageEncoder = null)
    {
        if (text.IsEmpty() || fileName.IsEmpty() || folder.IsEmpty()) return string.Empty;

        imageEncoder ??= new PngEncoder();

        text = text.TextSplitLine();

        var collection = new FontCollection();
        var family = collection.Add(System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName,"Fonts", "Arial.ttf"));
        var font = family.CreateFont(20);

        var textOptions = new RichTextOptions(font)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            WrappingLength = 600,
        };

        using var tempImage = new Image<Rgba32>(1, 1);
        var glyphs = TextBuilder.GenerateGlyphs(text, textOptions);
        var textSize = glyphs.Bounds;

        using var image = new Image<Rgba32>((int)textSize.Width + 10, (int)textSize.Height + 10);
        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);
            ctx.DrawText(textOptions, text, Color.Black);
        });

        var filePath = System.IO.Path.Combine(folder, fileName);
        await image.SaveAsync(filePath, imageEncoder);

        return filePath;
    }
}