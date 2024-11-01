namespace Domain.Constants;

public enum PaymentReason : byte
{
    None = 0,
    FirstPayment = 1,
    RenewalPayment = 2,
    UpgradePayment = 3,
    Refund = 4,
    Other = 5
}