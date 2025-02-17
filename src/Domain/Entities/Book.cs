using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class Book : BaseEntity<int>
{
    public int SchoolId { get; set; }
    public short LessonId { get; set; }
    public short PublisherId { get; set; }
    public string? Name { get; set; }
    public short PageCount { get; set; }
    public short? Year { get; set; }
    public string? ThumbBase64 { get; set; }
    public byte TryPrepareCount { get; set; }
    public BookStatus Status { get; set; }

    public virtual School? School { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual Publisher? Publisher { get; set; }
    public virtual ICollection<RBookClassRoom> BookClassRooms { get; set; } = [];
    public virtual ICollection<BookQuiz> BookQuizzes { get; set; } = [];

    public Book() : base()
    {
    }

    public Book(int id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
    }
}