﻿using Domain.Entities.Core;

namespace Domain.Entities;

public class PackageUser : BaseEntity<Guid>
{
    public short PackageId { get; set; }
    public long UserId { get; set; }
    public int RenewCount { get; set; }
    public DateTime EndDate { get; set; }
    public int QuestionCredit { get; set; }

    public virtual Package? Package { get; set; }
    public virtual User? User { get; set; }

    public PackageUser() : base()
    { }

    public PackageUser(Guid id, bool isActive, long createUser, DateTime createDate, long updateUser, DateTime updateDate, short packageId, long userId, DateTime endDate, int questionCredit)
        : base(id, isActive, createUser, createDate, updateUser, updateDate)
    {
        PackageId = packageId;
        UserId = userId;
        EndDate = endDate;
        QuestionCredit = questionCredit;
    }
}