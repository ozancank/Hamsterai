namespace DataAccess.EF.Concrete;

public class RBookClassRoomDal(HamsteraiDbContext context) : EfRepositoryBase<RBookClassRoom, HamsteraiDbContext>(context), IRBookClassRoomDal
{
}