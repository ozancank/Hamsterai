namespace Domain.Constants;

public enum PaymentRenewalPeriod : byte
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    SemiAnnually = 5,
    Annually = 6
}