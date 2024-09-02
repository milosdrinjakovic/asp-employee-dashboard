using System.Collections.Generic;

namespace employee_dashboard.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<dynamic> Employees { get; set; }
        public string PieChartPath { get; set; }
    }
}
