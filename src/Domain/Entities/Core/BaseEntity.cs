using OCK.Core.Interfaces;

namespace Domain.Entities.Core;

public abstract class BaseEntity<T> : IEntity
{
    public T? Id { get; set; }
    public bool IsActive { get; set; }
    public long CreateUser { get; set; }
    public DateTime CreateDate { get; set; }
    public long UpdateUser { get; set; }
    public DateTime UpdateDate { get; set; }

    protected BaseEntity()
    { }

    protected BaseEntity(T id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate) : this()
    {
        Id = id;
        IsActive = isActive;
        CreateUser = createUser;
        CreateDate = createDate;
        UpdateUser = updateUser;
        UpdateDate = updateDate;
    }
}