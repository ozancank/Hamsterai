namespace DataAccess.EF.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.ReceiveredUserId).HasColumnName("ReceiveredUserId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.IsRead).HasColumnName("IsRead").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.ReadDate).HasColumnName("ReadDate").HasColumnOrder(8);
        builder.Property(e => e.Title).HasColumnName("Title").HasColumnOrder(9).IsRequired();
        builder.Property(e => e.Body).HasColumnName("Body").HasColumnOrder(10).IsRequired();
        builder.Property(e => e.Type).HasColumnName("Type").HasColumnOrder(11).IsRequired();
        builder.Property(e => e.ReasonId).HasColumnName("ReasonId").HasColumnOrder(12);

        builder.HasOne(e => e.SenderUser).WithMany(e => e.SendNotification).HasForeignKey(e => e.CreateUser).HasPrincipalKey(e => e.Id);
        builder.HasOne(e => e.ReceiveredUser).WithMany(e => e.ReceivedNotification).HasForeignKey(e => e.ReceiveredUserId).HasPrincipalKey(e => e.Id);
    }
}