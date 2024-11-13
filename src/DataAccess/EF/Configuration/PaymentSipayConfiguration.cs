namespace DataAccess.EF.Configuration;

public class PaymentSipayConfiguration : IEntityTypeConfiguration<PaymentSipay>
{
    public void Configure(EntityTypeBuilder<PaymentSipay> builder)
    {
        builder.ToTable("PaymentSipays");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.IsActive).HasColumnName("IsActive").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.CreateUser).HasColumnName("CreateUser").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.UpdateUser).HasColumnName("UpdateUser").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.UpdateDate).HasColumnName("UpdateDate").HasColumnOrder(5).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(6).IsRequired();
        builder.Property(e => e.Status).HasColumnName("Status").HasColumnOrder(7);
        builder.Property(e => e.OrderNo).HasColumnName("OrderNo").HasColumnOrder(8);
        builder.Property(e => e.OrderId).HasColumnName("OrderId").HasColumnOrder(9);
        builder.Property(e => e.InvoiceId).HasColumnName("InvoiceId").HasColumnOrder(10);
        builder.Property(e => e.StatusCode).HasColumnName("StatusCode").HasColumnOrder(11);
        builder.Property(e => e.StatusDescription).HasColumnName("StatusDescription").HasColumnOrder(12);
        builder.Property(e => e.PaymentMethod).HasColumnName("PaymentMethod").HasColumnOrder(13);
        builder.Property(e => e.CreditCardNo).HasColumnName("CreditCardNo").HasColumnOrder(14);
        builder.Property(e => e.TransactionType).HasColumnName("TransactionType").HasColumnOrder(15);
        builder.Property(e => e.PaymentStatus).HasColumnName("PaymentStatus").HasColumnOrder(16);
        builder.Property(e => e.PaymentMethodCode).HasColumnName("PaymentMethodCode").HasColumnOrder(17);
        builder.Property(e => e.ErrorCode).HasColumnName("ErrorCode").HasColumnOrder(18);
        builder.Property(e => e.Error).HasColumnName("Error").HasColumnOrder(19);
        builder.Property(e => e.AuthCode).HasColumnName("AuthCode").HasColumnOrder(20);
        builder.Property(e => e.MerchantCommission).HasColumnName("MerchantCommission").HasColumnOrder(21);
        builder.Property(e => e.UserCommission).HasColumnName("UserCommission").HasColumnOrder(22);
        builder.Property(e => e.MerchantCommissionPercentage).HasColumnName("MerchantCommissionPercentage").HasColumnOrder(23);
        builder.Property(e => e.MerchantCommissionFixed).HasColumnName("MerchantCommissionFixed").HasColumnOrder(24);
        builder.Property(e => e.Installment).HasColumnName("Installment").HasColumnOrder(25);
        builder.Property(e => e.Amount).HasColumnName("Amount").HasColumnOrder(26);
        builder.Property(e => e.PaymentReasonCode).HasColumnName("PaymentReasonCode").HasColumnOrder(27);
        builder.Property(e => e.PaymentReasonCodeDetail).HasColumnName("PaymentReasonCodeDetail").HasColumnOrder(28);
        builder.Property(e => e.HashKey).HasColumnName("HashKey").HasColumnOrder(29);
        builder.Property(e => e.MdStatus).HasColumnName("MdStatus").HasColumnOrder(30);
        builder.Property(e => e.OriginalBankErrorCode).HasColumnName("OriginalBankErrorCode").HasColumnOrder(31);
        builder.Property(e => e.OriginalBankErrorDescription).HasColumnName("OriginalBankErrorDescription").HasColumnOrder(32);
    }
}