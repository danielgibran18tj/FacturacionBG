using Application.DTOs.Customer;
using Application.DTOs.PaymentMethod;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Settings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<PaymentMethod, PaymentMethodDto>();

            CreateMap<Customer, CustomerDto>()
              .ForMember(dest => dest.HasUserAccount,
                  opt => opt.MapFrom(src => src.UserId.HasValue))
              .ForMember(dest => dest.Username,
                  opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));
        }
    }
}
