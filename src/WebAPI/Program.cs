﻿using Application;
using Application.Services.QuestionService;
using Application.Services.UserService;
using Domain.Constants;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OCK.Core.Caching;
using OCK.Core.Caching.Microsoft;
using OCK.Core.Exceptions;
using OCK.Core.Extensions;
using OCK.Core.Security.Encryption;
using OCK.Core.Security.Headers;
using OCK.Core.Security.JWT;
using OCK.Core.Utilities;
using OCK.Core.Versioning;
using System.Diagnostics;
using System.Globalization;
using WebAPI.HostedServices;
using static Infrastructure.Constants.InfrastructureDelegates;
using static OCK.Core.Constants.Delegates;

var builder = WebApplication.CreateBuilder(args);

SetAppOptions(builder, out CultureInfo defaultCulture);

Services(builder);

Delegates();

var app = builder.Build();
await Middlewares(builder, app, defaultCulture);
app.Run();

static void SetAppOptions(WebApplicationBuilder builder, out CultureInfo defaultCulture)
{
    defaultCulture = new CultureInfo("tr-TR");
    CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    builder.Configuration.GetSection("ByPassOptions").Get<ByPassOptions>();
    builder.Configuration.GetSection("AppOptions").Get<Domain.Constants.AppOptions>();
    Domain.Constants.AppOptions.InitOptions();
}

static void Services(WebApplicationBuilder builder)
{
    Console.WriteLine("API Starting...");
    Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
    Console.WriteLine(Directory.GetCurrentDirectory());

    builder.Services.AddSingleton(FirebaseApp.Create(new FirebaseAdmin.AppOptions()
    {
        Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sedusssoru-private-key.json")),
    }));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHealthChecks();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddHttpClient();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSingleton<Stopwatch>();
    builder.Services.AddSingleton<ITokenHelper, JwtHelper>();
    builder.Services.AddSingleton<ICacheManager, DistributedCacheManager>();
    builder.Services.AddCustomApiVersioning(x =>
    {
        x.AddUrlSegmentApiVersionReader();
        x.EnableVersionedApiExplorer = true;
    });
    builder.Services.AddCors(opt =>
    {
        opt.AddPolicy("AllowEveryThing", builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        });
    });
    //if (!builder.Environment.IsDevelopment())
    builder.Services.AddHostedService<QuestionHostedService>();
    builder.Services.AddHostedService<SimilarHostedService>();
    builder.Services.AddHostedService<GainHostedService>();

    builder.Services.Configure<FormOptions>(options =>
    {
        options.MultipartBodyLengthLimit = 600 * 1024 * 1024;
        options.ValueLengthLimit = 1024 * 1024 * 1024;
        options.MemoryBufferThreshold = 1024 * 1024 * 1024;
    });

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Limits.MaxRequestBodySize = 600 * 1024 * 1024;
    });

    SwaggerAndToken(builder);

    ServiceTools.Create(builder);
    builder.Services.AddBusinessServices();
    ServiceTools.Create(builder);
}

static async Task Middlewares(WebApplicationBuilder builder, WebApplication app, CultureInfo defaultCulture)
{
    var localizationOptions = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("tr-TR"),
        SupportedCultures = [defaultCulture],
        SupportedUICultures = [defaultCulture]
    };
    app.UseRequestLocalization(localizationOptions);

    app.UseCors("AllowEveryThing");

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        //KnownProxies = { System.Net.IPAddress.Parse("185.195.255.123") }
    });

    if (!app.Environment.IsDevelopment())
        app.UseMiddleware<HeaderAuthMiddleware>([Strings.XApiKey, Strings.SwaggerPath, AppStatics.Domains, AppStatics.EndPoints]);

    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // Dokümantasyon genişlemesi kapalı
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example); // Modellerin render edilmesi 'example' ile sınırlı
        c.DisplayRequestDuration();// İstek süresinin gösterilmesi
        c.ConfigObject.AdditionalItems["syntaxHighlight"] = new Dictionary<string, object>
        {
            ["activated"] = false // Renklendirme kapalı
        };
    });

    await app.ConfigureExceptionHandling(opt =>
    {
        var detail = app.Configuration.GetValue<string>("DevPass") == "OCK";
        opt.UseExceptionDetails = detail;
        opt.UseLogger();
    });

    //app.UseMiddleware<LicenceMiddleware>();

    app.UseHealthChecks("/IsAlive", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            await context.Response.WriteAsync(report.Status.ToString());
        }
    });

    app.UseRequestId();

    app.UseAuthentication();

    app.UseAuthorization();

    StaticFiles(app);

    app.MapControllers();

    Console.WriteLine("API Started...");
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        foreach (var url in app.Urls) Console.WriteLine(url.Replace("[::]", "localhost"));
    });
}

static void SwaggerAndToken(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGenNewtonsoftSupport();

    builder.Services.AddSwaggerGen(opt =>
    {
        opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = """
            Example: Bearer 12345abcdef
            """,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        });

        opt.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme,Id = "Bearer"}
                },
                Array.Empty<string>()
            }
        });

        opt.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "HamsteraiWebApi",
            Version = "v1",
            Contact = new() { Name = "Ozan Can KÖSEMEZ", Email = "ozancank@gmail.com" },
        });

        //opt.SchemaFilter<EnumSchemaFilter>();
    });

    var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = tokenOptions!.Issuer,
                ValidateAudience = true,
                ValidAudience = tokenOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey ?? string.Empty),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            //Cookie üzerinden jwt taşımak için
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["bak-token"];
                    return Task.CompletedTask;
                }
            };
        });
}

static void StaticFiles(WebApplication app)
{
    var staticFilePaths = new Dictionary<string, string>
    {
        { "/ProfilePicture", Domain.Constants.AppOptions.ProfilePictureFolderPath },
        { "/QuestionPicture", Domain.Constants.AppOptions.QuestionPictureFolderPath },
        { "/QuestionSmallPicture", Domain.Constants.AppOptions.QuestionSmallPictureFolderPath },
        { "/QuestionThumbnail", Domain.Constants.AppOptions.QuestionThumbnailFolderPath },
        { "/AnswerPicture", Domain.Constants.AppOptions.AnswerPictureFolderPath },
        { "/SimilarQuestionPicture", Domain.Constants.AppOptions.SimilarQuestionPictureFolderPath },
        { "/SimilarAnswerPicture", Domain.Constants.AppOptions.SimilarAnswerPictureFolderPath },
        { "/QuizQuestionPicture", Domain.Constants.AppOptions.QuizQuestionPictureFolderPath },
        { "/QuizAnswerPicture", Domain.Constants.AppOptions.QuizAnswerPictureFolderPath },
        { "/PackagePicture", Domain.Constants.AppOptions.PackagePictureFolderPath },
        { "/Homework", Domain.Constants.AppOptions.HomeworkFolderPath },
        { "/HomeworkAnswer", Domain.Constants.AppOptions.HomeworkAnswerFolderPath }
    };

    foreach (var path in staticFilePaths)
    {
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(path.Value),
            RequestPath = path.Key,
            //OnPrepareResponse = context =>
            //{
            //    Console.WriteLine($"Dosya sunuluyor: {DateTime.Now} - {context.File.PhysicalPath}");
            //}
        });
    }


}

static void Delegates()
{
    ControlUserStatusAsync = ServiceTools.GetService<IUserService>().UserStatusAndLicense;
    UpdateQuestionOcrImage = ServiceTools.GetService<IQuestionService>().UpdateQuestion;
    AddSimilarAnswer = ServiceTools.GetService<IQuestionService>().AddSimilar;
}

/*
app.Use(async (context, next) =>
    {
        var pathMarked = "/Books/";
        var requestPath = context.Request.Path.Value;

        if (!requestPath.StartsWith(pathMarked, StringComparison.OrdinalIgnoreCase)) goto next;

        var pathArray = requestPath.Replace(pathMarked, string.Empty, StringComparison.OrdinalIgnoreCase).Split('/');
        if (pathArray.Length != 2) goto next;

        var isNumber = int.TryParse(pathArray[0], out var bookId);
        if (!isNumber) goto next;

        var commonService = ServiceTools.GetService<ICommonService>();
        var userTypes = commonService.HttpUserType;
        if (userTypes == UserTypes.Administator) goto file;
        if (!userTypes.IsIn(UserTypes.School, UserTypes.Teacher, UserTypes.Student)) goto next;

        var dbContext = ServiceTools.GetService<IDbContextFactory<HamsteraiDbContext>>().CreateDbContext();
        try
        {
            var bookInfo = await dbContext.Books.Where(x => x.Id == bookId).Select(x => new { x.CreateUser, x.SchoolId }).DefaultIfEmpty().FirstOrDefaultAsync();
            if (bookInfo == null) goto next;
            if (!await ControlUserStatusAsync(commonService.HttpUserId)) goto next;
            if (bookInfo.SchoolId != commonService.HttpSchoolId) goto next;
        }
        finally
        {
            dbContext.Dispose();
            commonService = null;
        }

    file:
        var fileName = pathArray[1];

        var fullPath = Path.Combine(Domain.Constants.AppOptions.BookFolderPath, $"{bookId}", fileName);

        if (!File.Exists(fullPath)) goto next;

        switch (Path.GetExtension(fileName)?.ToLowerInvariant())
        {
            case ".pdf":
                context.Response.ContentType = "application/pdf";
                break;

            case ".jpg":
            case ".jpeg":
                context.Response.ContentType = "image/jpeg";
                break;

            default:
                goto next;
        }

        await context.Response.SendFileAsync(fullPath);
        return;

    next:
        await next();
    });
 */