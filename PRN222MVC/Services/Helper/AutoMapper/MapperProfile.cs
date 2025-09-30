using AutoMapper;
using BusinessObject.BusinessObject;
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

            // Category
            CreateMap<VehicleCategory, BusinessObject.BusinessObject.CategoryModels.Respond.GetCategoryRespond>();
            CreateMap<BusinessObject.BusinessObject.CategoryModels.Request.AddCategoryResquest, VehicleCategory>();
            CreateMap<BusinessObject.BusinessObject.CategoryModels.Request.UpdateCategoryRequest, VehicleCategory>();
        }
    }
}
