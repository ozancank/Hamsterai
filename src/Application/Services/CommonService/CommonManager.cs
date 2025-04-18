﻿using Application.Features.Users.Rules;
using DataAccess.Abstract.Core;
using OCK.Core.Logging.Serilog;
using OCK.Core.Security.Extensions;
using OCK.Core.Security.Headers;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Reflection;

namespace Application.Services.CommonService;

public class CommonManager(IHttpContextAccessor httpContextAccessor,
                           LoggerServiceBase loggerServiceBase,
                           IConfiguration configuration,
                           IUserDal userDal,
                           ILessonDal lessonDal) : ICommonService
{
    public long HttpUserId =>
        Convert.ToInt64(httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");

    public string HttpUserName =>
        httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == CustomClaimTypes.UserName)?.Value.IfNullEmptyString("NaN") ?? "NaN";

    public UserTypes HttpUserType =>
        Enum.TryParse(httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.UserType)?.Value, out UserTypes userType) ? userType : UserTypes.None;

    public int? HttpSchoolId =>
        int.TryParse(httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.SchoolId)?.Value, out int schoolId) ? schoolId : null;

    public int? HttpConnectionId =>
        int.TryParse(httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.ConnectionId)?.Value, out int connectionId) ? connectionId : null;

    public byte? HttpPackageId =>
        byte.TryParse(httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.PackageId)?.Value, out byte packageId) ? packageId : null;

    public bool IsByPass => httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(ByPassOptions.Name, out var byPassKey) ?? false && byPassKey == ByPassOptions.Key;

    public async Task<string> PictureConvert(string? base64, string? fileName, string? folder, CancellationToken cancellationToken = default)
    {
        if (base64.IsEmpty() || fileName.IsEmpty() || folder.IsEmpty()) return string.Empty;

        await UserRules.PictureShouldAllowedType(fileName!);
        var filePath = System.IO.Path.Combine(folder!, fileName!);
        var imageBytes = Convert.FromBase64String(base64!);
        await File.WriteAllBytesAsync(filePath, imageBytes, cancellationToken);
        return filePath;
    }

    public async Task<string> PictureConvertWithResize(string? base64, string? fileName, string? folder, int dimension = 512, CancellationToken cancellationToken = default)
    {
        if (base64.IsEmpty() || fileName.IsEmpty() || folder.IsEmpty()) return string.Empty;
        if (dimension < 1) dimension = 512;

        await UserRules.PictureShouldAllowedType(fileName!);
        var filePath = System.IO.Path.Combine(folder!, fileName!);
        var imageBytes = Convert.FromBase64String(base64!);
        await using var inputStream = new MemoryStream(imageBytes);
        var image = await Image.LoadAsync(inputStream, cancellationToken);
        if (image.Width > dimension || image.Height > dimension)
        {
            var (newWidth, newHeight) = image.Width > image.Height
                ? (dimension, (int)(image.Height * (dimension / (float)image.Width)))
                : ((int)(image.Width * (dimension / (float)image.Height)), dimension);

            image.Mutate(x => x.Resize(newWidth, newHeight));
        }
        await using var outputStream = new MemoryStream();
        var extension = System.IO.Path.GetExtension(fileName)!.ToLowerInvariant();
        var encoder = ImageTools.CreateEncoderForImageSharp(extension);
        await image.SaveAsync(outputStream, encoder, cancellationToken: cancellationToken);
        await File.WriteAllBytesAsync(filePath, outputStream.ToArray(), cancellationToken);
        return filePath;
    }

    public async Task<string> TextToImage(string? text, string? fileName, string? folder, CancellationToken cancellationToken = default)
    {
        if (text.IsEmpty() || fileName.IsEmpty() || folder.IsEmpty()) return string.Empty;

        var extension = System.IO.Path.GetExtension(fileName)!.ToLowerInvariant();
        var imageEncoder = ImageTools.CreateEncoderForImageSharp(extension);

        text = text.TextSplitLine();

        var collection = new FontCollection();
        var family = collection.Add(System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Fonts", "Arial.ttf"));
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

        var filePath = System.IO.Path.Combine(folder!, fileName!);
        await image.SaveAsync(filePath, imageEncoder, cancellationToken);

        return filePath;
    }

    public async Task<string> TextToImageWithResize(string? text, string? fileName, string? folder, int dimension = 512, CancellationToken cancellationToken = default)
    {
        if (text.IsEmpty() || fileName.IsEmpty() || folder.IsEmpty()) return string.Empty;

        var extension = System.IO.Path.GetExtension(fileName)!.ToLowerInvariant();
        var imageEncoder = ImageTools.CreateEncoderForImageSharp(extension);

        text = text.TextSplitLine();

        var collection = new FontCollection();
        var family = collection.Add(System.IO.Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Fonts", "Arial.ttf"));
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

        var filePath = System.IO.Path.Combine(folder!, fileName!);
        await image.SaveAsync(filePath, imageEncoder, cancellationToken);
        await Task.Delay(100, cancellationToken);
        await ImageToBase64WithResize(filePath, dimension, cancellationToken);

        return filePath;
    }

    public async Task<string> ImageToBase64(string? path, CancellationToken cancellationToken = default)
    {
        try
        {
            if (path.IsEmpty()) return string.Empty;
            if (!File.Exists(path)) return string.Empty;
            var imageBytes = await File.ReadAllBytesAsync(path, cancellationToken);
            return Convert.ToBase64String(imageBytes);
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> ImageToBase64WithResize(string? path, int dimension = 512, CancellationToken cancellationToken = default)
    {
        try
        {
            if (path.IsEmpty()) return string.Empty;
            await using var inputStream = File.OpenRead(path!);
            var image = await Image.LoadAsync(inputStream, cancellationToken);

            if (image.Width > dimension || image.Height > dimension)
            {
                int newWidth, newHeight;
                if (image.Width > image.Height)
                {
                    newWidth = dimension;
                    newHeight = (int)(image.Height * (dimension / (float)image.Width));
                }
                else
                {
                    newHeight = dimension;
                    newWidth = (int)(image.Width * (dimension / (float)image.Height));
                }

                image.Mutate(x => x.Resize(newWidth, newHeight));
            }
            if (image.Width > dimension || image.Height > dimension)
            {
                var (newWidth, newHeight) = image.Width > image.Height
                    ? (dimension, (int)(image.Height * (dimension / (float)image.Width)))
                    : ((int)(image.Width * (dimension / (float)image.Height)), dimension);

                image.Mutate(x => x.Resize(newWidth, newHeight));
            }

            await using var memoryStream = new MemoryStream();
            await image.SaveAsJpegAsync(memoryStream, cancellationToken: cancellationToken);
            return Convert.ToBase64String(memoryStream.ToArray());
        }
        catch
        {
            return string.Empty;
        }
    }

    public void ThrowErrorTry(Exception exception)
    {
        if (HttpUserType != UserTypes.Administator) throw new AuthenticationException(Strings.AuthorizationDenied);
        throw exception;
    }

    public List<string> GetLogs(PageRequest pageRequest, bool onlyError)
    {
        if (!(HttpUserType == UserTypes.Administator
            || (httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(configuration.GetSection("ByPassOptions:Name")?.Value!, out var byPassKey)
            && byPassKey == configuration.GetSection("ByPassOptions:Key")?.Value))) throw new AuthenticationException(Strings.AuthorizationDenied);
        var result = loggerServiceBase.GetLogs(pageRequest, onlyError);
        return result;
    }

    public Dictionary<string, Dictionary<string, int>> GetEnums()
    {
        if (HttpUserType != UserTypes.Administator) throw new AuthenticationException(Strings.AuthorizationDenied);

        if (AppStatics.Enums.Count != 0) return AppStatics.Enums;

        var enums = typeof(AppStatics).Assembly.GetTypes()
            .Where(t => t.IsEnum && t.Namespace == "Domain.Constants");

        AppStatics.Enums = enums.ToDictionary(
            enumType => enumType.Name,
            enumType => Enum.GetValues(enumType)!.Cast<object>()!.ToDictionary(value => value.ToString()!, value => Convert.ToInt32(value))!
            ).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value)!;

        return AppStatics.Enums;
    }

    public Dictionary<string, List<AppStatics.PropertyDto>> GetEntities()
    {
        if (HttpUserType != UserTypes.Administator) throw new AuthenticationException(Strings.AuthorizationDenied);

        if (AppStatics.Entities.Count != 0) return AppStatics.Entities;

        var entityProperties = typeof(AppStatics).Assembly.GetTypes()
            .Where(type => typeof(IEntity).IsAssignableFrom(type) && !type.IsAbstract)
            .ToDictionary(
                type => type.Name,
                type => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Select(p => new AppStatics.PropertyDto(
                                p.Name,
                                p.PropertyType.Name,
                                ReflectionTools.IsNullable(p),
                                !p.CanWrite,
                                p.GetMethod?.IsVirtual ?? false
                            ))
                            .ToList()
            );

        AppStatics.Entities = entityProperties;
        return entityProperties;
    }

    public async Task<string?> GetUserAIUrl(long userId, CancellationToken cancellationToken = default)
    {
        var url = await userDal.GetAsync(
            enableTracking: false,
            predicate: x => x.Id == userId,
            selector: x => x.AIUrl,
            cancellationToken: cancellationToken);

        return url;
    }

    public async Task<string?> GetLessonNamesForAI(CancellationToken cancellationToken = default)
    {
        if (HttpUserType != UserTypes.Administator) throw new AuthenticationException(Strings.AuthorizationDenied);

        var lessonNames = await lessonDal.GetListAsync(
            enableTracking: false,
            selector: x => x.Name,
            cancellationToken: cancellationToken);
        var result = lessonNames.Select(x => x.ToSlug('_', true))!;
        return string.Join(",", result);
    }
}