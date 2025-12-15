using Application.DTOs.Auth;
using Application.DTOs.Customer;
using Application.DTOs.PaymentMethod;
using Application.DTOs.Product;
using Application.DTOs.SystemSettings;
using AutoMapper;
using Domain.Entities;

namespace Application.Common.Settings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<PaymentMethod, PaymentMethodDto>();
            CreateMap<ProductDto, Product>();
            CreateMap<CreateProductDto, Product>();
            CreateMap<PaymentMethodDto, PaymentMethod>();
            CreateMap<PaymentMethod, PaymentMethodDto>();

            CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.IsLowStock,
                opt => opt.MapFrom(src =>
                    src.MinStock.HasValue && src.Stock <= src.MinStock.Value
                ));

            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.HasUserAccount,
                    opt => opt.MapFrom(src => src.UserId.HasValue))
                .ForMember(dest => dest.Username,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Username : null));

            CreateMap<SystemSetting, SystemSettingDto>()
                .ForMember(dest => dest.Key,
                    opt => opt.MapFrom(src => src.SettingKey))
                .ForMember(dest => dest.Value,
                    opt => opt.MapFrom(src => src.SettingValue));

            CreateMap<SystemSettingDto, SystemSetting>()
                .ForMember(dest => dest.SettingKey,
                    opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.SettingValue,
                    opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<User, UserDto>()
               .ForMember(dest => dest.Roles,
                   opt => opt.MapFrom(src =>
                       src.UserRoles != null
                           ? src.UserRoles.Select(ur => ur.Role.Name)
                           : new List<string>()));
        }
    }
}
