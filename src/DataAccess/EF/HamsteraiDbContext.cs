using Domain.Entities.Core;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace DataAccess.EF;

public class HamsteraiDbContext : DbContext
{
    private static bool IsMigration = false;
    protected IConfiguration Configuration { get; set; }

    #region Core

    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    #endregion Core

    public DbSet<ClassRoom> ClassRooms { get; set; }
    public DbSet<Gain> Gains { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Homework> Homeworks { get; set; }
    public DbSet<HomeworkStudent> HomeworkStudents { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<LessonGroup> LessonGroups { get; set; }
    public DbSet<NotificationDeviceToken> NotificationDeviceTokens { get; set; }
    public DbSet<PasswordToken> PasswordTokens { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<School> Schools { get; set; }
    public DbSet<Similar> SimilarQuestions { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<TeacherClassRoom> TeacherClassRooms { get; set; }
    public DbSet<TeacherLesson> TeacherLessons { get; set; }

    public HamsteraiDbContext(DbContextOptions<HamsteraiDbContext> options, IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
        if (!IsMigration)
        {
            Database.Migrate();
            IsMigration = true;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("citext");
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
}

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