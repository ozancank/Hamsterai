namespace DataAccess.EF.Concrete;

public class ClassRoomDal(HamsteraiDbContext context) : EfRepositoryBase<ClassRoom, HamsteraiDbContext>(context), IClassRoomDal
{
}
