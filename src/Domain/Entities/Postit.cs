using Domain.Entities.Core;

namespace Domain.Entities;

public class Postit : BaseEntity<Guid>
{
    public short LessonId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public short SortNo { get; set; }
    public string? PictureFileName { get; set; }

    public virtual User? User { get; set; }
    public virtual Lesson? Lesson { get; set; }

    public Postit() : base()
    { }

    public Postit(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    { }
}