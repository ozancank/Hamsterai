using Application.Features.Books.Models.BookQuizzes;

namespace Application.Features.Books.Profiles;

public class BookQuizMappingProfiles : Profile
{
    public BookQuizMappingProfiles()
    {
        CreateMap<BookQuiz, GetBookQuizModel>()
            .ForMember(dest => dest.BookName, opt => opt.MapFrom(src => src.Book != null ? src.Book.Name : string.Empty))
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.Lesson != null ? src.Lesson.Name : string.Empty));
        CreateMap<IPaginate<GetBookQuizModel>, PageableModel<GetBookQuizModel>>();
        CreateMap<IPaginate<BookQuiz>, PageableModel<GetBookQuizModel>>();

        CreateMap<AddBookQuizModel, BookQuiz>();
        CreateMap<UpdateBookQuizModel, BookQuiz>();

        CreateMap<BookQuizUser, GetBookQuizUserModel>()
            .ForMember(dest => dest.BookName, opt => opt.MapFrom(src => src.BookQuiz != null && src.BookQuiz.Book != null ? src.BookQuiz.Book.Name : string.Empty))
            .ForMember(dest => dest.LessonName, opt => opt.MapFrom(src => src.BookQuiz != null && src.BookQuiz.Lesson != null ? src.BookQuiz.Lesson.Name : string.Empty))
            .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.BookQuiz != null ? src.BookQuiz.Name : string.Empty))
            .ForMember(dest => dest.OptionCount, opt => opt.MapFrom(src => src.BookQuiz != null ? src.BookQuiz.OptionCount : 0))
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.BookQuiz != null ? src.BookQuiz.QuestionCount : 0));
    }
}