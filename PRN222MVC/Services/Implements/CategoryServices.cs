using AutoMapper;
using BusinessObject.BusinessObject.CategoryModels.Respond;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using Services.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class CategoryServices : ICategoryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<GetCategoryRespond>> GetAllCategory()
        {
            var categoryRepo = _unitOfWork.GetRepository<VehicleCategory>();
            var categories = await categoryRepo.GetAllAsync();
            var categoryResponds = _mapper.Map<List<GetCategoryRespond>>(categories.ToList());
            return categoryResponds;
        }
    }
}
