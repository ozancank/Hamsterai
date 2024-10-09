using DataAccess.Abstract.Core;
using DataAccess.EF.Concrete;
using DataAccess.EF.Concrete.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OCK.Core;
using OCK.Core.Utilities;
using System.Diagnostics;

namespace DataAccess;

public static class DALServiceRegistration
{
    public static IServiceCollection AddDALServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<HamsteraiDbContext>(options =>
        {
            //options.UseSqlServer(ServiceTools.Configuration.GetConnectionString("HamsteraiConnectionString"),
            //    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            //);
            options.UseNpgsql(ServiceTools.Configuration.GetConnectionString("HamsteraiConnectionString"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        );
            if (Debugger.IsAttached) options.EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);

        services.AddScoped<IOperationClaimDal, OperationClaimDal>();
        services.AddScoped<IRefreshTokenDal, RefreshTokenDal>();
        services.AddScoped<IUserDal, UserDal>();
        services.AddScoped<IUserOperationClaimDal, UserOperationClaimDal>();

        services.AddScoped<IClassRoomDal, ClassRoomDal>();
        services.AddScoped<IGainDal, GainDal>();
        services.AddScoped<IGroupDal, GroupDal>();
        services.AddScoped<IHomeworkDal, HomeworkDal>();
        services.AddScoped<IHomeworkStudentDal, HomeworkStudentDal>();
        services.AddScoped<ILessonDal, LessonDal>();
        services.AddScoped<ILessonGroupDal, LessonGroupDal>();
        services.AddScoped<INotificationDeviceTokenDal, NotificationDeviceTokenDal>();
        services.AddScoped<IPasswordTokenDal, PasswordTokenDal>();
        services.AddScoped<IQuestionDal, QuestionDal>();
        services.AddScoped<IQuizDal, QuizDal>();
        services.AddScoped<IQuizQuestionDal, QuizQuestionDal>();
        services.AddScoped<ISchoolDal, SchoolDal>();
        services.AddScoped<ISimilarQuestionDal, SimilarQuestionDal>();
        services.AddScoped<IStudentDal, StudentDal>();
        services.AddScoped<ITeacherClassRoomDal, TeacherClassRoomDal>();
        services.AddScoped<ITeacherDal, TeacherDal>();
        services.AddScoped<ITeacherLessonDal, TeacherLessonDal>();

        services.AddCoreServices();
        return services;
    }
}