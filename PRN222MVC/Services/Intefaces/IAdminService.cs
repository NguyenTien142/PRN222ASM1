using BusinessObject.BusinessObject.UserModels.Respond;
using BusinessObject.BusinessObject.UserModels.Request;
using BusinessObject.BusinessObject.ReportModels;

namespace Services.Intefaces
{
    public interface IAdminService
    {
        Task<IEnumerable<GetUserRespond>> GetAllUsersAsync(string searchUsername = "");
        Task<GetDetailUserRespond?> GetUserByIdAsync(int id);
        Task<bool> EditUserAsync(UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<DealerSalesReportResponse>> GetDealerSalesReportAsync();
        Task<SalesReportSummary> GetSalesReportSummaryAsync();
    }
}