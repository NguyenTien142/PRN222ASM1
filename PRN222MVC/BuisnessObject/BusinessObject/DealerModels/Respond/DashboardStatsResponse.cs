using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.DealerModels.Respond
{
    public class DashboardStatsResponse
    {
        public int SuccessfulOrdersCount { get; set; }
        public int PendingOrdersCount { get; set; }
        public int StockQuantity { get; set; }
        public decimal TotalEarnings { get; set; }
        public int TotalSales { get; set; }
    }
}
