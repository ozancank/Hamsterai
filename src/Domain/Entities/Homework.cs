using Domain.Entities.Core;

namespace Domain.Entities;

public class Homework : BaseEntity<string>
{
    public int? SchoolId { get; set; }
    public int? TeacherId { get; set; }
    public byte LessonId { get; set; }
    public string FilePath { get; set; }
    public int? ClassRoomId { get; set; }

    public virtual User User { get; set; }
    public virtual School School { get; set; }
    public virtual Teacher Teacher { get; set; }
    public virtual Lesson Lesson { get; set; }
    public virtual ClassRoom ClassRoom { get; set; }
    public virtual ICollection<HomeworkStudent> HomeworkStudents { get; set; }

    public Homework() : base()
    {
        HomeworkStudents = [];
    }
}