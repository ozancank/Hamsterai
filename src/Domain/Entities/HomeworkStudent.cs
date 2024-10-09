using Domain.Constants;
using Domain.Entities.Core;

namespace Domain.Entities;

public class HomeworkStudent : BaseEntity<string>
{
    public int StudentId { get; set; }
    public string HomeworkId { get; set; }
    public string AnswerPath { get; set; }
    public HomeworkStatus Status { get; set; }

    public virtual Student Student { get; set; }
    public virtual Homework Homework { get; set; }

    public HomeworkStudent() : base()
    {
    }
}