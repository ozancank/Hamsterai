using Domain.Entities.Core;

namespace Domain.Entities;

public class Publisher : BaseEntity<short>
{
    public string? Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = [];

    public Publisher() : base()
    { }

    public Publisher(short id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, string name)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        Name = name;
    }
}