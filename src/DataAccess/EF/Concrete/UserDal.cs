using Domain.Entities.Core;

namespace DataAccess.EF.Concrete;

public class UserDal(HamsteraiDbContext context) : EfRepositoryBase<User, HamsteraiDbContext>(context), IUserDal
{
}