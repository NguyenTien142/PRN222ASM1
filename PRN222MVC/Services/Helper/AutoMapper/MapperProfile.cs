using AutoMapper;
using Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.BusinessObject;

namespace Services.Helper.AutoMapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Vehicle
            CreateMap<Vehicle, BusinessObject.BusinessObject.VehicleModels.Respond.GetDetailVehicleRespond>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.VehicleInventories != null && src.VehicleInventories.Any(vi => vi.Quantity > 0)));

            CreateMap<Vehicle, BusinessObject.BusinessObject.VehicleModels.Respond.GetVehicleRespond>();

            CreateMap<BusinessObject.BusinessObject.VehicleModels.Request.UpdateVehicleRequest, Vehicle>();
        }
    }
}
