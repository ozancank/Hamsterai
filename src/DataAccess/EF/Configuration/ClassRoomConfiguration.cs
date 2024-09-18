namespace DataAccess.EF.Configuration;

public class ClassRoomConfiguration : IEntityTypeConfiguration<ClassRoom>
{
    public void Configure(EntityTypeBuilder<ClassRoom> builder)
    {
        builder.ToTable("ClassRooms");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.No).HasColumnName("No").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Branch).HasColumnName("Branch").HasColumnType("citext").HasMaxLength(10).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.SchoolId).HasColumnName("SchoolId").HasColumnOrder(8).IsRequired();
        builder.HasIndex(e => new { e.No, e.Branch, e.SchoolId }).HasDatabaseName("IX_ClassRooms_1").IsUnique();

        builder.HasOne(d => d.School).WithMany(p => p.ClassRooms).HasForeignKey(d => d.SchoolId).HasPrincipalKey(x => x.Id);

        builder.HasData(
            new ClassRoom(1, true, 2, new DateTime(2000, 1, 1), 2, new(2000, 1, 1))
            {
                No = 1,
                Branch = "A",
                SchoolId = 1
            },
            new ClassRoom(2, true, 2, new DateTime(2000, 1, 1), 2, new(2000, 1, 1))
            {
                No = 1,
                Branch = "B",
                SchoolId = 1
            },
            new ClassRoom(3, true, 2, new DateTime(2000, 1, 1), 2, new(2000, 1, 1))
            {
                No = 1,
                Branch = "C",
                SchoolId = 1
            }
        );
    }
}