using Application.Features.Homeworks.Models;
using Application.Features.Homeworks.Rules;
using Application.Features.Lessons.Rules;
using Application.Features.Packages.Rules;
using Application.Features.Schools.Rules;
using Application.Features.Students.Rules;
using Application.Features.Users.Rules;
using Application.Services.CommonService;
using Application.Services.NotificationService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;

namespace Application.Features.Homeworks.Commands;

public class AddHomeworkCommand : IRequest<GetHomeworkModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public required AddHomeworkModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Administator, UserTypes.Teacher];
    public bool AllowByPass => false;
    public string[] HidePropertyNames { get; } = [];
}

public class AddHomeworkCommandHandler(IMapper mapper,
                                       ICommonService commonService,
                                       IHomeworkDal homeworkDal,
                                       IHomeworkStudentDal homeworkStudentDal,
                                       IStudentDal studentDal,
                                       IHomeworkUserDal homeworkUserDal,
                                       IPackageUserDal packageUserDal,
                                       INotificationService notificationService,
                                       LessonRules lessonRules,
                                       ClassRoomRules classRoomRules,
                                       StudentRules studentRules,
                                       UserRules userRules,
                                       PackageRules packageRules,
                                       HomeworkRules homeworkRules) : IRequestHandler<AddHomeworkCommand, GetHomeworkModel>
{
    public async Task<GetHomeworkModel> Handle(AddHomeworkCommand request, CancellationToken cancellationToken)
    {
        await HomeworkRules.HomeworkSendUserShouldBeTeacher(commonService.HttpUserType);
        await lessonRules.LessonShouldExistsAndActiveById(request.Model.LessonId);
        await HomeworkRules.OnlyOneShouldBeFilled(request.Model.ClassRoomId, request.Model.StudentIds, request.Model.UserIds, request.Model.PackageIds);

        List<int> studentIds = [];
        List<long> userIds = [];

        if (request.Model.ClassRoomId != null && request.Model.ClassRoomId > 0)
        {
            await classRoomRules.ClassRoomShouldExistsAndActiveById(request.Model.ClassRoomId.Value);
            studentIds = await studentDal.GetListAsync(
                enableTracking: false,
                predicate: x => x.ClassRoomId == request.Model.ClassRoomId.Value && x.IsActive,
                selector: x => x.Id,
                cancellationToken: cancellationToken);
        }
        else if (request.Model.StudentIds != null && request.Model.StudentIds.Count != 0)
        {
            await studentRules.StudentsShouldExistsAndActiveByIds(request.Model.StudentIds);
            studentIds = request.Model.StudentIds;
        }
        else if (request.Model.UserIds != null && request.Model.UserIds.Count > 0)
        {
            await userRules.UserShouldExistsAndActiveByIds(request.Model.UserIds);
            userIds = request.Model.UserIds;
        }
        else if (request.Model.PackageIds != null && request.Model.PackageIds.Count > 0)
        {
            await packageRules.PackageShouldExistsAndActiveByIds(request.Model.PackageIds);
            userIds = await packageUserDal.GetListAsync(
                enableTracking: false,
                predicate: x => request.Model.PackageIds.Contains(x.PackageId) && x.IsActive,
                selector: x => x.UserId,
                cancellationToken: cancellationToken);
        }

        var senderUserId = commonService.HttpUserId;
        var date = DateTime.Now;
        var idPrefix = $"HW-{senderUserId}-{request.Model.LessonId}-";
        var homeworkId = $"{idPrefix}{date:HHmmssfff}";

        await homeworkRules.HomeworkShouldNotExistsById(homeworkId);

        var fileName = $"{homeworkId}_{Guid.NewGuid()}{Path.GetExtension(request.Model.File!.FileName)}";
        var filePath = Path.Combine(AppOptions.HomeworkFolderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.Model.File.CopyToAsync(stream, cancellationToken);
        }

        var homework = new Homework()
        {
            Id = homeworkId,
            IsActive = true,
            CreateDate = date,
            CreateUser = senderUserId,
            UpdateDate = date,
            UpdateUser = senderUserId,
            LessonId = request.Model.LessonId,
            FilePath = fileName,
            ClassRoomId = request.Model.ClassRoomId,
            SchoolId = commonService.HttpSchoolId.IfNullEmpty(null),
            TeacherId = commonService.HttpConnectionId.IfNullEmpty(null),
        };

        if (studentIds.Count != 0)
        {
            List<HomeworkStudent> homeworkStudents = [];

            foreach (var studentId in studentIds)
            {
                var id = $"{homeworkId}-{studentId}";
                if (await homeworkStudentDal.IsExistsAsync(x => x.Id == id, cancellationToken: cancellationToken)) continue;

                homeworkStudents.Add(new()
                {
                    Id = id,
                    IsActive = true,
                    CreateDate = date,
                    CreateUser = senderUserId,
                    UpdateDate = date,
                    UpdateUser = senderUserId,
                    StudentId = studentId,
                    HomeworkId = homeworkId,
                    AnswerPath = null,
                    Status = HomeworkStatus.Send,
                });
            }

            await homeworkDal.ExecuteWithTransactionAsync(async () =>
            {
                await homeworkDal.AddAsync(homework, cancellationToken: cancellationToken);
                await homeworkStudentDal.AddRangeAsync(homeworkStudents, cancellationToken: cancellationToken);
            }, cancellationToken: cancellationToken);

            //_ = notificationService.PushNotificationByUserId(new("Öğretmen Testi", "Bir ödeviniz var.", NotificationTypes.HomeWork, userIds, homework.Id, commonService.HttpUserId));
        }
        else if (userIds.Count != 0)
        {
            List<HomeworkUser> homeworkUsers = [];
            foreach (var userId in userIds)
            {
                var id = $"{homeworkId}-{userId}";
                if (await homeworkStudentDal.IsExistsAsync(x => x.Id == id, cancellationToken: cancellationToken)) continue;
                homeworkUsers.Add(new()
                {
                    Id = id,
                    IsActive = true,
                    CreateDate = date,
                    CreateUser = senderUserId,
                    UpdateDate = date,
                    UpdateUser = senderUserId,
                    UserId = userId,
                    HomeworkId = homeworkId,
                    AnswerPath = null,
                    Status = HomeworkStatus.Send,
                });
            }

            await homeworkDal.ExecuteWithTransactionAsync(async () =>
            {
                await homeworkDal.AddAsync(homework, cancellationToken: cancellationToken);
                await homeworkUserDal.AddRangeAsync(homeworkUsers, cancellationToken: cancellationToken);
            }, cancellationToken: cancellationToken);

            _ = notificationService.PushNotificationByUserId(new("Deneme Sınavı", "Seduss sizin için cdeneme sınavı oluşturdu.", NotificationTypes.HomeWork, userIds, homework.Id, commonService.HttpUserId));
        }

        var result = await homeworkDal.GetAsyncAutoMapper<GetHomeworkModel>(
            enableTracking: false,
            predicate: x => x.Id == homeworkId,
            include: x => x.Include(u => u.User)
                           .Include(u => u.School)
                           .Include(u => u.Teacher)
                           .Include(h => h.Lesson)
                           .Include(h => h.ClassRoom)
                           .Include(h => h.HomeworkStudents)
                           .Include(h => h.HomeworkUsers),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        return result;
    }
}

public class AddHomeworkCommandValidator : AbstractValidator<AddHomeworkCommand>
{
    public AddHomeworkCommandValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model).NotNull().WithMessage(Strings.InvalidValue);

        RuleFor(x => x.Model.LessonId).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.Lesson]);

        RuleFor(x => x.Model.File).NotEmpty().WithMessage(Strings.DynamicNotEmpty, [Strings.File]);
    }
}