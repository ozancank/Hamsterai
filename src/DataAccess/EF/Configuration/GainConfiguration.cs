namespace DataAccess.EF.Configuration;

public class GainConfiguration : IEntityTypeConfiguration<Gain>
{
    public void Configure(EntityTypeBuilder<Gain> builder)
    {
        builder.ToTable("Gains");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50).HasColumnOrder(6).IsRequired();
        builder.Property(e => e.LessonId).HasColumnName("LessonId").HasColumnOrder(7).IsRequired();
        builder.HasIndex(e => new { e.Name, e.LessonId }).HasDatabaseName("IX_Gains_1").IsUnique();

        builder.HasOne(d => d.Lesson).WithMany(p => p.Gains).HasForeignKey(d => d.LessonId).HasPrincipalKey(x => x.Id);
    }
}