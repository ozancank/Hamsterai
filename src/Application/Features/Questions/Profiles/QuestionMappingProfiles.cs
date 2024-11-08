using Application.Features.Questions.Models.Questions;

namespace Application.Features.Questions.Profiles;

public class QuestionMappingProfiles : Profile
{
    public QuestionMappingProfiles()
    {
        CreateMap<Question, GetQuestionModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson!.Name))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain!.Name));
        CreateMap<IPaginate<GetQuestionModel>, PageableModel<GetQuestionModel>>();

        CreateMap<AddQuestionModel, Question>();
    }
}