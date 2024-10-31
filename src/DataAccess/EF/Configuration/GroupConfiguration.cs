namespace DataAccess.EF.Configuration;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Name).HasColumnName("Name").HasMaxLength(50).HasColumnOrder(6).IsRequired();
        builder.HasAlternateKey(e => e.Name).HasName("UK_Groups_Name");
        builder.Property(e => e.SortNo).HasColumnName("SortNo").HasDefaultValue(0).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.IsWebVisible).HasColumnName("IsWebVisible").HasDefaultValue(false).HasColumnOrder(8).IsRequired();

        builder.HasData([
                new(1, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Sözel"),
                new(2, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Sayısal"),
                new(3, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Eşit Ağırlık"),
                new(4, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Yabancı Dil"),
                new(5, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Hepsi"),
                new(6, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Orta Okul"),
                new(7, true, 1, new DateTime(2000, 01, 01), 1, new DateTime(2000, 01, 01), "Lise"),
            ]);
    }
}