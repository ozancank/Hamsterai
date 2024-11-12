using Application.Features.Payments.Models;

namespace Application.Features.Payments.Profiles;

public class PaymentMappingProfiles : Profile
{
    public PaymentMappingProfiles()
    {
        CreateMap<Payment, GetPaymentModel>();
        CreateMap<IPaginate<GetPaymentModel>, PageableModel<GetPaymentModel>>();
    }
}