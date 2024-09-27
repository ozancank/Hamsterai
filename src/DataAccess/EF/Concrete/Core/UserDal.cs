using DataAccess.Abstract.Core;
using Domain.Entities.Core;

namespace DataAccess.EF.Concrete.Core;

public class UserDal(HamsteraiDbContext context) : EfRepositoryBase<User, HamsteraiDbContext>(context), IUserDal
{
}