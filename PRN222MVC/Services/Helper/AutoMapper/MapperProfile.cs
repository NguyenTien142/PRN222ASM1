using AutoMapper;
using BusinessObject.BusinessObject;
using BusinessObject.BusinessObject.VehicleModels.Respond;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Vehicle
            CreateMap<Vehicle, BusinessObject.BusinessObject.VehicleModels.Respond.GetDetailVehicleRespond>()
                .ForMember(dest => dest.Category,
                           opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.VehicleInventories != null
                                                     ? src.VehicleInventories.Sum(vi => vi.Quantity)
                                                     : 0));

            CreateMap<Vehicle, BusinessObject.BusinessObject.VehicleModels.Respond.GetVehicleRespond>();
            CreateMap<BusinessObject.BusinessObject.VehicleModels.Request.CreateVehicleRequest, Vehicle>();
            CreateMap<BusinessObject.BusinessObject.VehicleModels.Request.UpdateVehicleRequest, Vehicle>();

            CreateMap<Vehicle, GetVehicleByDealerRespond>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Quantity,
                opt => opt.MapFrom(src => src.VehicleInventories
                    .Sum(vi => vi.Quantity)))
            .ForMember(dest => dest.DealerId,
                opt => opt.MapFrom(src => src.VehicleInventories
                    .Select(vi => vi.Inventory.DealerId)
                    .FirstOrDefault()))
            .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => src.VehicleInventories
                    .Select(vi => vi.Inventory.Dealer.Address)
                    .FirstOrDefault()))
            .ForMember(dest => dest.DealerType,
                opt => opt.MapFrom(src => src.VehicleInventories
                    .Select(vi => vi.Inventory.Dealer.DealerType.TypeName)
                    .FirstOrDefault()))
            .ForMember(dest => dest.ManufactureDate,
                opt => opt.MapFrom(src => src.ManufactureDate.ToDateTime(TimeOnly.MinValue)));

            // Category
            CreateMap<VehicleCategory, BusinessObject.BusinessObject.CategoryModels.Respond.GetCategoryRespond>();
            CreateMap<BusinessObject.BusinessObject.CategoryModels.Request.AddCategoryResquest, VehicleCategory>();
            CreateMap<BusinessObject.BusinessObject.CategoryModels.Request.UpdateCategoryRequest, VehicleCategory>();

            // User mappings
            CreateMap<User, BusinessObject.BusinessObject.UserModels.Respond.GetUserRespond>()
                .ForMember(dest => dest.DealerTypeName,
                           opt => opt.MapFrom(src => src.Dealer != null && src.Dealer.DealerType != null 
                                                     ? src.Dealer.DealerType.TypeName 
                                                     : string.Empty))
                .ForMember(dest => dest.DealerAddress,
                           opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Address : string.Empty));

            CreateMap<User, BusinessObject.BusinessObject.UserModels.Respond.GetDetailUserRespond>()
                .ForMember(dest => dest.DealerTypeName,
                           opt => opt.MapFrom(src => src.Dealer != null && src.Dealer.DealerType != null 
                                                     ? src.Dealer.DealerType.TypeName 
                                                     : string.Empty))
                .ForMember(dest => dest.DealerAddress,
                           opt => opt.MapFrom(src => src.Dealer != null ? src.Dealer.Address : string.Empty))
                .ForMember(dest => dest.DealerId,
                           opt => opt.MapFrom(src => src.DealerId));

            CreateMap<BusinessObject.BusinessObject.UserModels.Request.RegisterRequest, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

            CreateMap<BusinessObject.BusinessObject.UserModels.Request.UpdateUserRequest, User>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            // Dealer mappings
            CreateMap<Dealer, BusinessObject.BusinessObject.DealerModels.Respond.GetDealerRespond>()
                .ForMember(dest => dest.TypeName,
                           opt => opt.MapFrom(src => src.DealerType != null 
                                                     ? src.DealerType.TypeName 
                                                     : string.Empty));

        }
    }
}
