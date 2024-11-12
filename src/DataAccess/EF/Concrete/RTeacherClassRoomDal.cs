namespace DataAccess.EF.Concrete;

public class RTeacherClassRoomDal(HamsteraiDbContext context) : EfRepositoryBase<RTeacherClassRoom, HamsteraiDbContext>(context), IRTeacherClassRoomDal
{
}