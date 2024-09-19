using Business;
using Business.Services.QuestionService;
using Business.Services.UserService;
using Domain.Constants;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OCK.Core.Caching;
using OCK.Core.Caching.Microsoft;
using OCK.Core.Exceptions;
using OCK.Core.Logging.Serilog;
using OCK.Core.Security.Encryption;
using OCK.Core.Security.JWT;
using OCK.Core.Utilities;
using OCK.Core.Versioning;
using System.Diagnostics;
using static Infrastructure.Constants.InfrastructureDelegates;
using static OCK.Core.Constants.Delegates;

//var isService = !(Debugger.IsAttached || args.Contains("--console"));

//ServiceInstall();

var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
//if (isService && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) builder.Host.UseSystemd();
//else if (isService && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) builder.Host.UseWindowsService();

builder.Configuration.GetSection("AppOptions").Get<Domain.Constants.AppOptions>();
Domain.Constants.AppOptions.CreateFolder();

builder.Services.AddSingleton(FirebaseApp.Create(new FirebaseAdmin.AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hamster-private-key.json")),
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<Stopwatch>();
builder.Services.AddSingleton<ITokenHelper, JwtHelper>();
builder.Services.AddSingleton<ICacheManager, MemoryCacheManager>();
builder.Services.AddCustomApiVersioning(x =>
{
    x.AddUrlSegmentApiVersionReader();
    x.EnableVersionedApiExplorer = true;
});

SwaggerAndToken(builder);

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowEveryThing", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

ServiceTools.Create(builder);
builder.Services.AddBusinessServices();
ServiceTools.Create(builder);

ControlUserStatus = ServiceTools.GetService<IUserService>().UserStatus;
UpdateQuestion = ServiceTools.GetService<IQuestionService>().UpdateAnswer;
UpdateQuestionOcr = ServiceTools.GetService<IQuestionService>().UpdateAnswer;
UpdateQuestionOcrImage = ServiceTools.GetService<IQuestionService>().UpdateAnswer;
UpdateSimilarQuestionAnswer = ServiceTools.GetService<IQuestionService>().UpdateSimilarAnswer;

#region Middleware
var app = builder.Build();

app.UseCors("AllowEveryThing");

if (!app.Environment.IsDevelopment())
    app.UseMiddleware<HeaderAuthMiddleware>([Strings.XApiKey, Strings.SwaggerPath, Strings.Domain]);

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

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Domain.Constants.AppOptions.ProfilePictureFolderPath),
    RequestPath = "/ProfilePicture"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Domain.Constants.AppOptions.QuestionPictureFolderPath),
    RequestPath = "/QuestionPicture"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Domain.Constants.AppOptions.AnswerPictureFolderPath),
    RequestPath = "/AnswerPicture"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Domain.Constants.AppOptions.SimilarQuestionPictureFolderPath),
    RequestPath = "/SimilarQuestionPicture"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Domain.Constants.AppOptions.SimilarAnswerPictureFolderPath),
    RequestPath = "/SimilarAnswerPicture"
});

app.UseRequestId();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("API çalışıyor...");
Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
Console.WriteLine(Directory.GetCurrentDirectory());

app.Run();
#endregion Middleware

#region Swagger

static void SwaggerAndToken(WebApplicationBuilder builder)
{
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

#endregion Swagger

#region Service
/*
void ServiceInstall()
{
    if (!isService) return;
    if (args.Length == 0) return;
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

    string[] commands = ["--install", "-i", "--uninstall", "-u", "--stop", "-s", "--restart", "-r"];
    if (!args.Any(x => commands.Contains(x, StringComparer.OrdinalIgnoreCase))) return;

    var batFilePath = (args[0].ToLowerInvariant()) switch
    {
        "--install" or "-i" => "Install.bat",
        "--stop" or "-s" => "Stop.bat",
        "--restart" or "-r" => "Restart.bat",
        "--uninstall" or "-u" => "Uninstall.bat",
        _ => string.Empty
    };

    var directory = AppDomain.CurrentDomain.BaseDirectory;
    Console.WriteLine(directory);
    if (string.IsNullOrWhiteSpace(batFilePath)) Environment.Exit(0);
    var tempBatFilePath = Path.GetTempFileName() + ".bat";

    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Web.API.Batchs.{batFilePath}"))
    {
        if (stream == null) Environment.Exit(0);
        using var reader = new StreamReader(stream);
        File.WriteAllText(tempBatFilePath, reader.ReadToEnd().Replace("#ServicePath#", directory));
    }

    try
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"cd /d {directory} && {tempBatFilePath}\"",
                Verb = "runas",
                UseShellExecute = true,
                //CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        if (File.Exists(tempBatFilePath)) File.Delete(tempBatFilePath);
        Environment.Exit(0);
    }
}
*/
#endregion