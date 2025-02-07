namespace DataAccess.EF.Configuration;

public class RBookClassRoomConfiguration : IEntityTypeConfiguration<RBookClassRoom>
{
    public void Configure(EntityTypeBuilder<RBookClassRoom> builder)
    {
        builder.ToTable("RBookClassRooms");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.BookId).HasColumnName("BookId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.ClassRoomId).HasColumnName("ClassRoomId").HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => new { e.BookId, e.ClassRoomId }).HasDatabaseName("IX_RBookClassRooms_1").IsUnique();

        builder.HasOne(d => d.ClassRoom).WithMany(p => p.BookClassRooms).HasForeignKey(d => d.ClassRoomId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Book).WithMany(p => p.BookClassRooms).HasForeignKey(d => d.BookId).HasPrincipalKey(x => x.Id);
    }
}