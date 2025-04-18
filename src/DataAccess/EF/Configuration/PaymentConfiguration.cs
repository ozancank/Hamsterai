﻿namespace DataAccess.EF.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Amount).HasColumnName("Amount").HasDefaultValue(0).HasColumnOrder(7).IsRequired();
        builder.Property(e => e.PaymentDate).HasColumnName("PaymentDate").HasColumnOrder(8).IsRequired();
        builder.Property(e => e.PaymentReason).HasColumnName("PaymentReason").HasDefaultValue(PaymentReason.None).HasColumnOrder(9).IsRequired();
        builder.Property(e => e.ReasonId).HasColumnName("ReasonId").HasMaxLength(50).HasColumnOrder(10);
        builder.Property(e => e.PaymentSipayId).HasColumnName("PaymentSipayId").HasColumnOrder(11);

        builder.HasOne(e => e.User).WithMany(e => e.Payments).HasForeignKey(e => e.UserId).HasPrincipalKey(e => e.Id);
        builder.HasOne(e => e.PaymentSipay).WithMany(e => e.Payments).HasForeignKey(e => e.PaymentSipayId).HasPrincipalKey(e => e.Id);
    }
}