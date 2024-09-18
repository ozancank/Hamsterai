namespace DataAccess.EF.Concrete;

public class TeacherClassRoomDal(HamsteraiDbContext context) : EfRepositoryBase<TeacherClassRoom, HamsteraiDbContext>(context), ITeacherClassRoomDal
{
}
