namespace DataAccess.EF.Concrete;

public class PasswordTokenDal(HamsteraiDbContext context) : EfRepositoryBase<PasswordToken, HamsteraiDbContext>(context), IPasswordTokenDal
{
}