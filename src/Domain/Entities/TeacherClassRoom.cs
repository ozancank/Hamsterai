using Domain.Entities.Core;

namespace Domain.Entities;

public class TeacherClassRoom : BaseEntity<Guid>
{
    public int TeacherId { get; set; }
    public int ClassRoomId { get; set; }

    public virtual Teacher Teacher { get; set; }
    public virtual ClassRoom ClassRoom { get; set; }

    public TeacherClassRoom() : base()
    {
    }

    public TeacherClassRoom(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, int teacherId, int classRoomId)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        TeacherId = teacherId;
        ClassRoomId = classRoomId;
    }
}