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
            CreateMap<Vehicle, GetDetailVehicleRespond>()
                .ForMember(dest => dest.Category,
                           opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AvailableQuantity,
                           opt => opt.MapFrom(src => src.VehicleInventories != null
                                                     ? src.VehicleInventories.Sum(vi => vi.Quantity)
                                                     : 0));

            CreateMap<Vehicle, BusinessObject.BusinessObject.VehicleModels.Respond.GetVehicleRespond>();

            CreateMap<BusinessObject.BusinessObject.VehicleModels.Request.UpdateVehicleRequest, Vehicle>();
        }
    }
}
