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
            if (!options.IsConfigured)
            {
                //options.UseSqlServer(ServiceTools.Configuration.GetConnectionString("HamsteraiConnectionString"),
                //    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                //);
                options.UseNpgsql(ServiceTools.Configuration.GetConnectionString("HamsteraiConnectionString"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            );
            }
            if (Debugger.IsAttached) options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IOperationClaimDal, OperationClaimDal>();
        services.AddScoped<IRefreshTokenDal, RefreshTokenDal>();
        services.AddScoped<IUserDal, UserDal>();
        services.AddScoped<IUserOperationClaimDal, UserOperationClaimDal>();

        services.AddScoped<IClassRoomDal, ClassRoomDal>();
        services.AddScoped<IGainDal, GainDal>();
        services.AddScoped<IHomeworkDal, HomeworkDal>();
        services.AddScoped<IHomeworkStudentDal, HomeworkStudentDal>();
        services.AddScoped<ILessonDal, LessonDal>();
        services.AddScoped<INotificationDal, NotificationDal>();
        services.AddScoped<INotificationDeviceTokenDal, NotificationDeviceTokenDal>();
        services.AddScoped<IOrderDal, OrderDal>();
        services.AddScoped<IPackageCategoryDal, PackageCategoryDal>();
        services.AddScoped<IPackageDal, PackageDal>();
        services.AddScoped<IPackageUserDal, PackageUserDal>();
        services.AddScoped<IPasswordTokenDal, PasswordTokenDal>();
        services.AddScoped<IPaymentDal, PaymentDal>();
        services.AddScoped<IQuestionDal, QuestionDal>();
        services.AddScoped<IQuizDal, QuizDal>();
        services.AddScoped<IQuizQuestionDal, QuizQuestionDal>();
        services.AddScoped<IRPackageLessonDal, RPackageLessonDal>();
        services.AddScoped<IRPackageSchoolDal, RPackageSchoolDal>();
        services.AddScoped<IRTeacherClassRoomDal, RTeacherClassRoomDal>();
        services.AddScoped<IRTeacherLessonDal, RTeacherLessonDal>();
        services.AddScoped<ISchoolDal, SchoolDal>();
        services.AddScoped<ISimilarDal, SimilarDal>();
        services.AddScoped<IStudentDal, StudentDal>();
        services.AddScoped<ITeacherDal, TeacherDal>();

        services.AddCoreServices();
        return services;
    }
}