using Business.Features.Orders.Models;
using Business.Features.Students.Models;

namespace Business.Features.Orders.Profiles;

public class OrderMappingProfiles : Profile
{
    public OrderMappingProfiles()
    {
        CreateMap<Order, GetOrderModel>()
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.OrderDetails));
        CreateMap<IPaginate<GetOrderModel>, PageableModel<GetOrderModel>>();

        CreateMap<OrderDetail, GetOrderDetailModel>()
            .ForMember(dest => dest.Package, opt => opt.MapFrom(src => src.Package));
        CreateMap<IPaginate<GetOrderDetailModel>, PageableModel<GetOrderDetailModel>>();
    }
}