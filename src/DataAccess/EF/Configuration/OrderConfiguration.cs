namespace DataAccess.EF.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.OrderNo).HasColumnName("OrderNo").HasMaxLength(50).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.QuestionCredit).HasColumnName("QuestionCredit").HasDefaultValue(0).HasColumnOrder(8).IsRequired();
        builder.Property(e => e.SubTotal).HasColumnName("SubTotal").HasDefaultValue(0).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasDefaultValue(0).HasColumnOrder(10).IsRequired();
        builder.Property(e => e.TaxBase).HasColumnName("TaxBase").HasDefaultValue(0).HasColumnOrder(11).IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasDefaultValue(0).HasColumnOrder(12).IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasDefaultValue(0).HasColumnOrder(13).IsRequired();

        builder.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId).HasPrincipalKey(x => x.Id);
    }
}