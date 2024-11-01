namespace DataAccess.EF.Configuration;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.OrderId).HasColumnName("OrderId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.PackageId).HasColumnName("PackageId").HasColumnOrder(7).IsRequired();
        builder.Property(e => e.QuestionCredit).HasColumnName("QuestionCredit").HasDefaultValue(0).HasColumnOrder(8).IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("Quantity").HasDefaultValue(0).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnName("UnitPrice").HasDefaultValue(0).HasColumnOrder(10).IsRequired();
        builder.Property(e => e.DiscountRatio).HasColumnName("DiscountRatio").HasDefaultValue(0).HasColumnOrder(11).IsRequired();
        builder.Property(e => e.DiscountAmount).HasColumnName("DiscountAmount").HasDefaultValue(0).HasColumnOrder(12).IsRequired();
        builder.Property(e => e.TaxBase).HasColumnName("TaxBase").HasDefaultValue(0).HasColumnOrder(13).IsRequired();
        builder.Property(e => e.TaxRatio).HasColumnName("TaxRatio").HasDefaultValue(0).HasColumnOrder(14).IsRequired();
        builder.Property(e => e.TaxAmount).HasColumnName("TaxAmount").HasDefaultValue(0).HasColumnOrder(15).IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasDefaultValue(0).HasColumnOrder(16).IsRequired();

        builder.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasForeignKey(d => d.OrderId).HasPrincipalKey(x => x.Id);
        builder.HasOne(d => d.Package).WithMany(p => p.OrderDetails).HasForeignKey(d => d.PackageId).HasPrincipalKey(x => x.Id);
    }
}