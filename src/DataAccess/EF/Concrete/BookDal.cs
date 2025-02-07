namespace DataAccess.EF.Concrete;

public class BookDal(HamsteraiDbContext context) : EfRepositoryBase<Book, HamsteraiDbContext>(context), IBookDal
{
}