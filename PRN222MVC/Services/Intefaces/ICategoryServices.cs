using BusinessObject.BusinessObject.CategoryModels.Request;
using BusinessObject.BusinessObject.CategoryModels.Respond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Intefaces
{
    public interface ICategoryServices
    {
        Task<List<GetCategoryRespond>> GetAllCategory();
        Task<GetCategoryRespond?> GetCategoryById(int categoryId);
        Task<bool> AddCategory(AddCategoryResquest category);
        Task<bool> UpdateCategory(int categoryId, AddCategoryResquest category);
        Task<bool> DeleteCategory(int categoryId);
    }
}
