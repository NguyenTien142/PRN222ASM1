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
    }
}
