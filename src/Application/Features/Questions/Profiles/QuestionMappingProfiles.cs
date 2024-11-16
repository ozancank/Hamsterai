using Application.Features.Questions.Models.Questions;

namespace Application.Features.Questions.Profiles;

public class QuestionMappingProfiles : Profile
{
    public QuestionMappingProfiles()
    {
        CreateMap<Question, GetQuestionModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson!=null? src.Lesson.Name:default))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain != null ? src.Gain.Name : default));
        CreateMap<IPaginate<GetQuestionModel>, PageableModel<GetQuestionModel>>();

        CreateMap<AddQuestionModel, Question>();
        CreateMap<IEnumerable<Question>, List<GetQuestionModel>>();
    }
}