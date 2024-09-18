namespace DataAccess.EF.Configuration;

public class TeacherClassRoomConfiguration : IEntityTypeConfiguration<TeacherClassRoom>
{
    public void Configure(EntityTypeBuilder<TeacherClassRoom> builder)
    {
        builder.ToTable("TeacherClassRooms");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.TeacherId).HasColumnName("TeacherId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.ClassRoomId).HasColumnName("ClassRoomId").HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => new { e.TeacherId, e.ClassRoomId }).HasDatabaseName("IX_TeacherClassRooms_1").IsUnique();

        builder.HasOne(d => d.ClassRoom).WithMany(p => p.TeacherClassRooms).HasForeignKey(d => d.ClassRoomId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Teacher).WithMany(p => p.TeacherClassRooms).HasForeignKey(d => d.TeacherId).HasPrincipalKey(x => x.Id);

        builder.HasData([
            new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,1),
            new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec1"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,2),
            new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec2"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),1,3),
            new(Guid.Parse("a1a84a26-a7e4-4671-a979-d65fbbbedec3"),true,2,new DateTime(2000, 01, 01), 2, new DateTime(2000, 01, 01),2,3),
        ]);
    }
}