using Business.Features.Homeworks.Models;
using Business.Features.Homeworks.Rules;
using Business.Features.Lessons.Rules;
using Business.Features.Schools.Rules;
using Business.Features.Students.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;
using OCK.Core.Pipelines.Logging;
using OCK.Core.Security.HashingHelper;

namespace Business.Features.Homeworks.Commands;

public class AddHomeworkCommand : IRequest<GetHomeworkModel>, ISecuredRequest<UserTypes>, ILoggableRequest
{
    public AddHomeworkModel Model { get; set; }
    public UserTypes[] Roles { get; } = [UserTypes.Teacher];
    public string[] HidePropertyNames { get; } = [];
}

public class AddHomeworkCommandHandler(IMapper mapper,
                                       ICommonService commonService,
                                       IHomeworkDal homeworkDal,
                                       IHomeworkStudentDal homeworkStudentDal,
                                       IStudentDal studentDal,
                                       LessonRules lessonRules,
                                       ClassRoomRules classRoomRules,
                                       StudentRules studentRules) : IRequestHandler<AddHomeworkCommand, GetHomeworkModel>
{
    public async Task<GetHomeworkModel> Handle(AddHomeworkCommand request, CancellationToken cancellationToken)
    {
        await lessonRules.LessonShouldExistsAndActiveById(request.Model.LessonId);
        await HomeworkRules.OnlyOneShouldBeFilled(request.Model.ClassRoomId, request.Model.StudentIds);

        List<int> studentIds = [];

        if (request.Model.ClassRoomId != null)
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

        var userId = commonService.HttpUserId;
        var date = DateTime.Now;
        var idPrefix = $"HW-{userId}-{request.Model.LessonId}-";
        var maxId = await homeworkDal.Query().AsNoTracking().Where(x => x.Id.StartsWith(idPrefix)).OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        var nextId = Convert.ToInt32(maxId?[idPrefix.Length..] ?? "0") + 1;
        var homeworkId = $"{idPrefix}{nextId}";

        var fileName = $"{homeworkId}_{Guid.NewGuid()}{Path.GetExtension(request.Model.File.FileName)}";
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
            CreateUser = userId,
            UpdateDate = date,
            UpdateUser = userId,
            LessonId = request.Model.LessonId,
            FilePath = fileName,
            ClassRoomId = request.Model.ClassRoomId,
            SchoolId = commonService.HttpSchoolId.IfNullEmpty(null),
            TeacherId = commonService.HttpConnectionId.IfNullEmpty(null),
        };

        HashingHelper.CreatePasswordHash(AppOptions.DefaultPassword, out byte[] passwordHash, out byte[] passwordSalt);

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
                CreateUser = userId,
                UpdateDate = date,
                UpdateUser = userId,
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

        var result = await homeworkDal.GetAsyncAutoMapper<GetHomeworkModel>(
            enableTracking: false,
            predicate: x => x.Id == homeworkId,
            include: x => x.Include(u => u.User)
                           .Include(u => u.School)
                           .Include(u => u.Teacher)
                           .Include(h => h.Lesson)
                           .Include(h => h.ClassRoom)
                           .Include(h => h.HomeworkStudents),
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