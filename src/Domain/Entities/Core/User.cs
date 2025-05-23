﻿using Domain.Constants;
using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class User : Security.User
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? ProfileUrl { get; set; }
    public string? Email { get; set; }
    public UserTypes Type { get; set; }
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }
    public bool AutomaticPayment { get; set; }
    public string? TaxNumber { get; set; }
    public string? AIUrl { get; set; }
    public string? ExitPassword { get; set; }

    public virtual School? School { get; set; }
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; } = [];
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public virtual ICollection<PackageUser> PackageUsers { get; set; } = [];
    public virtual ICollection<Question> Questions { get; set; } = [];
    public virtual ICollection<PasswordToken> PasswordTokens { get; set; } = [];
    public virtual ICollection<Similar> SimilarQuestions { get; set; } = [];
    public virtual ICollection<NotificationDeviceToken> NotificationDeviceTokens { get; set; } = [];
    public virtual ICollection<Quiz> Quizzes { get; set; } = [];
    public virtual ICollection<Homework> Homeworks { get; set; } = [];
    public virtual ICollection<Notification> SendNotification { get; set; } = [];
    public virtual ICollection<Notification> ReceivedNotification { get; set; } = [];
    public virtual ICollection<Order> Orders { get; set; } = [];
    public virtual ICollection<Payment> Payments { get; set; } = [];
    public virtual ICollection<HomeworkUser> HomeworkUsers { get; set; } = [];
    public virtual ICollection<Postit> Postits { get; set; } = [];
    public virtual ICollection<BookQuizUser> BookQuizUsers { get; set; } = [];

    public User() : base()
    {
    }

    public User(long id, string userName, byte[] passwordSalt, byte[] passwordHash, bool mustPasswordChange, DateTime createDate, bool isActive)
        : base(id, userName, passwordSalt, passwordHash, mustPasswordChange, createDate, isActive)
    {
    }
}