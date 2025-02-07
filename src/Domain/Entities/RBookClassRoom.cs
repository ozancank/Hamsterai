using Domain.Entities.Core;

namespace Domain.Entities;

public class RBookClassRoom : BaseEntity<Guid>
{
    public int BookId { get; set; }
    public int ClassRoomId { get; set; }

    public virtual Book? Book { get; set; }
    public virtual ClassRoom? ClassRoom { get; set; }

    public RBookClassRoom() : base()
    { }

    public RBookClassRoom(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, int bookId, int classRoomId)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        BookId = bookId;
        ClassRoomId = classRoomId;
    }
}