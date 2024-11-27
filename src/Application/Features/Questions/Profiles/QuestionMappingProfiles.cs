using Application.Features.Questions.Models.Questions;

namespace Application.Features.Questions.Profiles;

public class QuestionMappingProfiles : Profile
{
    public QuestionMappingProfiles()
    {
        var httpContextAccessor = ServiceTools.GetService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;
        var baseUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}";

        CreateMap<Question, GetQuestionModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : default))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain != null ? src.Gain.Name : default));
        CreateMap<IPaginate<GetQuestionModel>, PageableModel<GetQuestionModel>>();

        CreateMap<Question, GetQuestionForAdminModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : default))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain != null ? src.Gain.Name : default))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : default))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.QuestionPicture, opt => opt.MapFrom(src => $"{baseUrl}/QuestionPicture/{src.QuestionPictureFileName}"));
        CreateMap<IPaginate<GetQuestionForAdminModel>, PageableModel<GetQuestionForAdminModel>>();

        CreateMap<AddQuestionModel, Question>();
        CreateMap<IEnumerable<Question>, List<GetQuestionModel>>();
    }
}