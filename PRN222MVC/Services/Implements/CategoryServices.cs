using AutoMapper;
using BusinessObject.BusinessObject.CategoryModels.Request;
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

        public async Task<GetCategoryRespond?> GetCategoryById(int categoryId)
        {
            var categoryRepo = _unitOfWork.GetRepository<VehicleCategory>();
            var category = await categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
            {
                return null;
            }
            var categoryRespond = _mapper.Map<GetCategoryRespond>(category);
            return categoryRespond;
        }

        public async Task<bool> AddCategory(AddCategoryResquest category)
        {
            var categoryRepo = _unitOfWork.GetRepository<VehicleCategory>();
            var newCategory = _mapper.Map<VehicleCategory>(category);
            await categoryRepo.AddAsync(newCategory);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateCategory(int categoryId, AddCategoryResquest category)
        {
            var categoryRepo = _unitOfWork.GetRepository<VehicleCategory>();
            var existingCategory = await categoryRepo.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(category.Name))
                return false;
            existingCategory.Name = category.Name;
            categoryRepo.Update(existingCategory);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            var categoryRepo = _unitOfWork.GetRepository<VehicleCategory>();
            var existingCategory = await categoryRepo.GetByIdAsync(categoryId);
            if (existingCategory == null)
            {
                return false;
            }
            categoryRepo.DeleteAsync(existingCategory);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
