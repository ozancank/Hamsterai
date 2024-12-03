using Application;
using Application.Services.QuestionService;
using Application.Services.UserService;
using Domain.Constants;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OCK.Core.Caching;
using OCK.Core.Caching.Microsoft;
using OCK.Core.Exceptions;
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
    Domain.Constants.AppOptions.CreateFolder();
    if (Environment.OSVersion.Platform == PlatformID.Unix) RunLinuxCommands();
}

static void Services(WebApplicationBuilder builder)
{
    Console.WriteLine("API Starting...");
    Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
    Console.WriteLine(Directory.GetCurrentDirectory());

    builder.Services.AddSingleton(FirebaseApp.Create(new FirebaseAdmin.AppOptions()
    {
        Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hamster-private-key.json")),
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
        options.MultipartBodyLengthLimit = 524288000;
        options.ValueLengthLimit = 1024 * 1024 * 1024;
        options.MemoryBufferThreshold = 1024 * 1024 * 1024;
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

    //app.UseForwardedHeaders(new ForwardedHeadersOptions
    //{
    //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    //});

    if (!app.Environment.IsDevelopment())
        app.UseMiddleware<HeaderAuthMiddleware>([Strings.XApiKey, Strings.SwaggerPath, new string[] { Strings.Domain, Strings.Domain2 }]);

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

    StaticFiles(app);

    app.UseRequestId();

    app.UseAuthentication();

    app.UseAuthorization();

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

static void RunLinuxCommands()
{
    string[] commands =
    [
            $"sudo chown -R www-data:www-data {Directory.GetParent(Domain.Constants.AppOptions.ProfilePictureFolderPath).FullName}",
            $"sudo chown -R www-data:www-data {Domain.Constants.AppOptions.HomeworkFolderPath}",
            $"sudo chown -R www-data:www-data {Domain.Constants.AppOptions.HomeworkAnswerFolderPath}",
            $"sudo chmod -R 755 {Directory.GetParent(Domain.Constants.AppOptions.ProfilePictureFolderPath).FullName}",
            $"sudo chmod -R 755 {Domain.Constants.AppOptions.HomeworkFolderPath}",
            $"sudo chmod -R 755 {Domain.Constants.AppOptions.HomeworkAnswerFolderPath}",
    ];

    foreach (var command in commands)
    {
        ExecuteBashCommand(command);
    }
}

static void ExecuteBashCommand(string command)
{
    var processInfo = new ProcessStartInfo("bash", $"-c \"{command}\"")
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var process = new Process { StartInfo = processInfo };
    process.Start();
    process.WaitForExit();

    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    if (!string.IsNullOrEmpty(error))
    {
        Console.WriteLine($"Error: {error}");
    }
}