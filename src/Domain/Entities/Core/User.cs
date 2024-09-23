using Domain.Constants;
using Security = OCK.Core.Security.Entities;

namespace Domain.Entities.Core;

public class User : Security.User
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string ProfileUrl { get; set; }
    public string Email { get; set; }
    public UserTypes Type { get; set; }
    public int? ConnectionId { get; set; }
    public int? SchoolId { get; set; }

    public virtual School School { get; set; }
    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    public virtual ICollection<Question> Questions { get; set; }
    public virtual ICollection<PasswordToken> PasswordTokens { get; set; }
    public virtual ICollection<Similar> SimilarQuestions { get; set; }
    public virtual ICollection<NotificationDeviceToken> NotificationDeviceTokens { get; set; }

    public User() : base()
    {
        UserOperationClaims = [];
        RefreshTokens = [];
        Questions = [];
        PasswordTokens = [];
        SimilarQuestions = [];
        NotificationDeviceTokens = [];
    }

    public User(long id, string userName, byte[] passwordSalt, byte[] passwordHash, bool mustPasswordChange, DateTime createDate, bool isActive)
        : base(id, userName, passwordSalt, passwordHash, mustPasswordChange, createDate, isActive)
    {
        UserOperationClaims = [];
        RefreshTokens = [];
        Questions = [];
        PasswordTokens = [];
        SimilarQuestions = [];
        NotificationDeviceTokens = [];
    }
}