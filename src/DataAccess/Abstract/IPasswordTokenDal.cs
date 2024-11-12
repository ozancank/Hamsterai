namespace DataAccess.Abstract;

public interface IPasswordTokenDal : ISyncRepository<PasswordToken>, IAsyncRepository<PasswordToken>
{
}