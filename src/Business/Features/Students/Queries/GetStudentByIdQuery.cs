using Business.Features.Students.Models;
using Business.Features.Students.Rules;
using Business.Services.CommonService;
using MediatR;
using OCK.Core.Pipelines.Authorization;

namespace Business.Features.Students.Queries;

public class GetStudentByIdQuery : IRequest<GetStudentModel>, ISecuredRequest<UserTypes>
{
    public byte Id { get; set; }
    public bool ThrowException { get; set; } = true;
    public bool Tracking { get; set; } = false;

    public UserTypes[] Roles { get; } = [UserTypes.School];
}

public class GetStudentByIdQueryHandler(IMapper mapper,
                                        ICommonService commonService,
                                        IStudentDal studentDal) : IRequestHandler<GetStudentByIdQuery, GetStudentModel>
{
    public async Task<GetStudentModel> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var student = await studentDal.GetAsyncAutoMapper<GetStudentModel>(
            enableTracking: request.Tracking,
            predicate: x => x.Id == request.Id && x.ClassRoom.SchoolId == commonService.HttpSchoolId,
            include: x => x.Include(u => u.ClassRoom).Include(u => u.Teachers),
            configurationProvider: mapper.ConfigurationProvider,
            cancellationToken: cancellationToken);

        if (request.ThrowException) await StudentRules.StudentShouldExists(student);
        return student;
    }
}