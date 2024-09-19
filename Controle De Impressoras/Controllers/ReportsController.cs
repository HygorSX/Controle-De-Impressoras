using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class ReportsController : Controller
    {
        private readonly PrintersContext _context = new PrintersContext();

        public ActionResult Index(DateTime? startDate, DateTime? endDate, int? printerId)
        {
            var query = _context.PrinterStatusLogs.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(r => r.DataHoraDeBusca >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.DataHoraDeBusca <= endDate.Value);
            }

            if (printerId.HasValue && printerId.Value > 0)
            {
                query = query.Where(r => r.PrinterId == printerId.Value);
            }

            var reports = query.ToList();

            var printerDetails = _context.Printers.FirstOrDefault(p => p.Id == printerId);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.PrinterId = printerId;
            ViewBag.PrinterDetails = printerDetails; 

            return View(reports);
        }

    }
}
