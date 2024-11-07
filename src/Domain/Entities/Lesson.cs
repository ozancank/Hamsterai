using Domain.Entities.Core;

namespace Domain.Entities;

public class Lesson : BaseEntity<short>
{
    public string? Name { get; set; }
    public short SortNo { get; set; }

    public virtual ICollection<RTeacherLesson> TeacherLessons { get; set; } = [];
    public virtual ICollection<RPackageLesson> RPackageLessons { get; set; } = [];
    public virtual ICollection<Question> Questions { get; set; } = [];
    public virtual ICollection<Similar> SimilarQuestions { get; set; } = [];
    public virtual ICollection<Gain> Gains { get; set; } = [];
    public virtual ICollection<Quiz> Quizzes { get; set; } = [];
    public virtual ICollection<Homework> Homeworks { get; set; } = [];

    public Lesson() : base()
    { }

    public Lesson(byte id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name, byte sortNo)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
        SortNo = sortNo;
    }
}