using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.BusinessObject.ReportModels
{
    public class DealerSalesReportResponse
    {
        public string DealerName { get; set; } = null!;
        public string DealerType { get; set; } = null!;
        public string DealerAddress { get; set; } = null!;
        public int TotalOrders { get; set; }
        public int SuccessfulOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalEarnings { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public int TotalVehiclesSold { get; set; }
    }

    public class SalesReportSummary
    {
        public int TotalDealers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalEarnings { get; set; }
        public DateTime ReportGeneratedDate { get; set; }
    }
}
