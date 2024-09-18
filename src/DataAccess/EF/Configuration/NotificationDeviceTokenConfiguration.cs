namespace DataAccess.EF.Configuration;

public class NotificationDeviceTokenConfiguration : IEntityTypeConfiguration<NotificationDeviceToken>
{
    public void Configure(EntityTypeBuilder<NotificationDeviceToken> builder)
    {
        builder.ToTable("NotificationDeviceTokens");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.DeviceToken).HasColumnName("DeviceToken").HasColumnOrder(4).IsRequired();

        builder.HasOne(e => e.User).WithMany(e => e.NotificationDeviceTokens).HasForeignKey(e => e.UserId).HasPrincipalKey(e => e.Id);
    }
}