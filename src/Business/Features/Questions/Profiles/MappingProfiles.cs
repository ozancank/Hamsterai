using Business.Features.Questions.Models.Questions;
using Business.Features.Questions.Models.Similars;

namespace Business.Features.Questions.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        #region Question

        CreateMap<Question, GetQuestionModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson.Name))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain.IfNullEmptyString(x => x.Name)));
        CreateMap<IPaginate<GetQuestionModel>, PageableModel<GetQuestionModel>>();

        CreateMap<AddQuestionModel, Question>();

        #endregion Question

        #region SimilarQuestion

        CreateMap<SimilarQuestion, GetSimilarModel>()
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson.Name))
            .ForMember(dest => dest.GainName, opt => opt.MapFrom(src => src.Gain.IfNullEmptyString(x => x.Name)));
        CreateMap<IPaginate<GetSimilarModel>, PageableModel<GetSimilarModel>>();

        CreateMap<AddSimilarModel, SimilarQuestion>();

        #endregion SimilarQuestion
    }
}