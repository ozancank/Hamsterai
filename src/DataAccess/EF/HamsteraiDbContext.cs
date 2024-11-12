using Domain.Entities.Core;
using Microsoft.Extensions.Configuration;
using OCK.Core.Utilities;
using System.Reflection;

namespace DataAccess.EF;

public class HamsteraiDbContext : DbContext
{
    private static bool IsMigration = false;
    protected IConfiguration Configuration { get; set; }

    #region Core

    public required DbSet<User> Users { get; set; }
    public required DbSet<OperationClaim> OperationClaims { get; set; }
    public required DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public required DbSet<RefreshToken> RefreshTokens { get; set; }

    #endregion Core

    public required DbSet<ClassRoom> ClassRooms { get; set; }
    public required DbSet<Gain> Gains { get; set; }
    public required DbSet<Homework> Homeworks { get; set; }
    public required DbSet<HomeworkStudent> HomeworkStudents { get; set; }
    public required DbSet<Lesson> Lessons { get; set; }
    public required DbSet<Notification> Notifications { get; set; }
    public required DbSet<NotificationDeviceToken> NotificationDeviceTokens { get; set; }
    public required DbSet<Order> Orders { get; set; }
    public required DbSet<OrderDetail> OrderDetails { get; set; }
    public required DbSet<PackageCategory> PackageCategories { get; set; }
    public required DbSet<Package> Package { get; set; }
    public required DbSet<PackageUser> PackageUser { get; set; }
    public required DbSet<PasswordToken> PasswordTokens { get; set; }
    public required DbSet<Payment> Payments { get; set; }
    public required DbSet<Question> Questions { get; set; }
    public required DbSet<Quiz> Quizzes { get; set; }
    public required DbSet<QuizQuestion> QuizQuestions { get; set; }
    public required DbSet<RPackageLesson> RPackageLesson { get; set; }
    public required DbSet<RPackageSchool> RPackageSchools { get; set; }
    public required DbSet<RTeacherClassRoom> RTeacherClassRooms { get; set; }
    public required DbSet<RTeacherLesson> RTeacherLessons { get; set; }
    public required DbSet<School> Schools { get; set; }
    public required DbSet<Similar> Similars { get; set; }
    public required DbSet<Student> Students { get; set; }
    public required DbSet<Teacher> Teachers { get; set; }

    public HamsteraiDbContext(DbContextOptions<HamsteraiDbContext> options)
        : base(options)
    {
        Configuration = ServiceTools.GetService<IConfiguration>();
        if (!IsMigration)
        {
            Database.Migrate();
            IsMigration = true;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.RegisterDbFunctions<PostgresqlFunctions>();
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        foreach (var item in modelBuilder.Model.GetEntityTypes())
        {
            item.GetForeignKeys().Where(x => x.DeleteBehavior == DeleteBehavior.Cascade).ToList().ForEach(x => x.DeleteBehavior = DeleteBehavior.NoAction);
        }

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetColumnType("citext");
                }
            }
        }

        //modelBuilder.HasDbFunction(() => Microsoft.EntityFrameworkCore.EF.Functions.TrLower(default))
        //            .HasTranslation(args => new SqlFunctionExpression("tr_lower", true, typeof(string), null));

        //modelBuilder.HasDbFunction(() => Microsoft.EntityFrameworkCore.EF.Functions.TrUpper(default))
        //    .HasTranslation(args => new SqlFunctionExpression("tr_upper", true, typeof(string), null));
    }
}

//protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
//{
//    configurationBuilder.Properties<DateTime>()
//        .HaveConversion(typeof(DateTimeToDateTimeUtc));
//}

//private class DateTimeToDateTimeUtc : ValueConverter<DateTime, DateTime>
//{
//    public DateTimeToDateTimeUtc() : base(c => DateTime.SpecifyKind(c, DateTimeKind.Utc), c => c)
//    {
//    }
//}

//protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//{
//    optionsBuilder.ReplaceService<IMigrationsSqlGenerator, CustomNpgsqlMigrationsSqlGenerator>();
//}

//private class CustomSqlServerMigrationsSqlGenerator
//    (MigrationsSqlGeneratorDependencies dependencies, ICommandBatchPreparer commandBatchPreparer)
//    : SqlServerMigrationsSqlGenerator(dependencies, commandBatchPreparer)
//{
//    protected override void Generate(SqlServerCreateDatabaseOperation operation, Microsoft.EntityFrameworkCore.Metadata.IModel model, MigrationCommandListBuilder builder)
//    {
//        base.Generate(operation, model, builder);

//        builder
//            .Append("ALTER DATABASE ")
//            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
//            .Append(" COLLATE ")
//            .Append("Turkish_CI_AS")
//            .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
//            .EndCommand(suppressTransaction: true);
//    }
//}

//private class CustomNpgsqlMigrationsSqlGenerator : NpgsqlMigrationsSqlGenerator
//{
//    public CustomNpgsqlMigrationsSqlGenerator(
//        MigrationsSqlGeneratorDependencies dependencies,
//        INpgsqlSingletonOptions npgsqlSingletonOptions)
//        : base(dependencies, npgsqlSingletonOptions)
//    {
//    }

//    protected override void Generate(NpgsqlCreateDatabaseOperation operation, IModel model, MigrationCommandListBuilder builder)
//    {
//        base.Generate(operation, model, builder);

//        builder
//            .Append("ALTER DATABASE ")
//            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
//            .Append(" SET LC_COLLATE TO 'tr_TR.UTF-8'")
//            .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
//            .EndCommand(suppressTransaction: true);

//        builder
//            .Append("ALTER DATABASE ")
//            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
//            .Append(" SET LC_CTYPE TO 'tr_TR.UTF-8'")
//            .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator)
//            .EndCommand(suppressTransaction: true);
//    }

//}

/*
 *
    if (IsMigration())
        {
            SeedUserOperationClaims(modelBuilder);
}

private static bool IsMigration()
    {
        var isMigration = false;
        var stackTrace = new System.Diagnostics.StackTrace();
        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod();
            if (method == null || method.Name != "OnModelCreating") continue;

            var declaringType = method.DeclaringType;
            if (declaringType == null || declaringType.Name != "Migration") continue;

            isMigration = true;
            break;
        }
        return isMigration;
    }

    private void SeedUserOperationClaims(ModelBuilder modelBuilder)
    {
        var userOperationClaimSeed = new List<UserOperationClaim>
        {
            new() { UserId = 1, OperationClaimId = 1 }
        };

        foreach (var operationClaim in OperationClaimContainer.OperationClaimList)
        {
            userOperationClaimSeed.Add(new() {UserId = 2, OperationClaimId = operationClaim.Id });
        }

        foreach (var claim in userOperationClaimSeed)
        {
            if (!UserOperationClaims.Any(uoc => uoc.UserId == claim.UserId && uoc.OperationClaimId == claim.OperationClaimId))
            {
                modelBuilder.Entity<UserOperationClaim>().HasData(claim);
            }
        }
    }
 */