using Domain.Entities.Core;

namespace Domain.Entities;

public class Lesson : BaseEntity<byte>
{
    public string Name { get; set; }
    public byte SortNo { get; set; }

    public virtual ICollection<TeacherLesson> TeacherLessons { get; set; }
    public virtual ICollection<LessonGroup> LessonGroups { get; set; }
    public virtual ICollection<Question> Questions { get; set; }
    public virtual ICollection<Similar> SimilarQuestions { get; set; }
    public virtual ICollection<Gain> Gains { get; set; }

    public Lesson() : base()
    {
        TeacherLessons = [];
        LessonGroups = [];
        Questions = [];
        SimilarQuestions = [];
        Gains = [];
    }

    public Lesson(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name, byte sortNo)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
        SortNo = sortNo;
        TeacherLessons = [];
        LessonGroups = [];
        Questions = [];
        SimilarQuestions = [];
        Gains = [];
    }
}