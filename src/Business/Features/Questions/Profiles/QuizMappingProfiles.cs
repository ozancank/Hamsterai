using Business.Features.Questions.Models.Quizzes;

namespace Business.Features.Questions.Profiles;

public class QuizMappingProfiles : Profile
{
    public QuizMappingProfiles()
    {
        CreateMap<Quiz, GetQuizModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson.Name))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => $"{src.User.Name} {src.User.Surname}"))
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.User.School.Name))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.QuizQuestions))
            .ForMember(dest => dest.RightOptions, opt => opt.MapFrom(src => src.QuizQuestions.Select(x => new KeyValuePair<byte, char>(x.SortNo, x.RightOption))))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.QuizQuestions.Select(x => new KeyValuePair<byte, char?>(x.SortNo, x.AnswerOption))))
            .ForMember(dest => dest.GainNames, opt => opt.MapFrom(src => src.QuizQuestions.OrderBy(x => x.Gain.Name).Select(x => x.Gain.Name).Distinct()));
        CreateMap<IPaginate<GetQuizModel>, PageableModel<GetQuizModel>>();

        CreateMap<Quiz, GetQuizListModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson.Name))
            .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => $"{src.User.Name} {src.User.Surname}"))
            .ForMember(dest => dest.SchoolName, opt => opt.MapFrom(src => src.User.School.Name))
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => (byte)src.QuizQuestions.Count()))
            .ForMember(dest => dest.GainNames, opt => opt.MapFrom(src => src.QuizQuestions.OrderBy(x => x.Gain.Name).Select(x => x.Gain.Name).Distinct()));
        CreateMap<IPaginate<GetQuizListModel>, PageableModel<GetQuizListModel>>();

        CreateMap<QuizQuestion, GetQuizQuestionModel>()
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain.Name));
        CreateMap<IPaginate<GetQuizQuestionModel>, PageableModel<GetQuizQuestionModel>>();
    }
}