namespace DataAccess.EF.Configuration;

public class BookQuizUserConfiguration : IEntityTypeConfiguration<BookQuizUser>
{
    public void Configure(EntityTypeBuilder<BookQuizUser> builder)
    {
        builder.ToTable("BookQuizUsers");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.BookQuizId).HasColumnName("BookQuizId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.Answers).HasColumnName("Answers").HasColumnType("citext").HasColumnOrder(8).IsRequired();

        builder.HasOne(d => d.BookQuiz).WithMany(p => p.BookQuizUsers).HasForeignKey(d => d.BookQuizId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.User).WithMany(p => p.BookQuizUsers).HasForeignKey(d => d.UserId).HasPrincipalKey(x => x.Id);
    }
}