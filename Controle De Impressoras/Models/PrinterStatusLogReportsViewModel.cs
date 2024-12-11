using System.Collections.Generic;

namespace Controle_De_Impressoras.Models
{
    public class PrinterStatusLogModelView
    {
        public List<PrinterStatusLogModel> Reports { get; set; }
        public List<PrinterStatusLogMonthlyReportModel> MonthlyReports { get; set; }
    }
}
