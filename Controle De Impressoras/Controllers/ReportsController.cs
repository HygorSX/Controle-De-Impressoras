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

        public ActionResult PrinterDetails(int printerId)
        {
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            Console.WriteLine($"First Day: {firstDayOfMonth}, Last Day: {lastDayOfMonth}");

            var printerDetails = _context.PrinterStatusLogs
                .Where(p => p.PrinterId == printerId && p.DataHoraDeBusca >= firstDayOfMonth && p.DataHoraDeBusca <= lastDayOfMonth)
                .OrderBy(p => p.DataHoraDeBusca)
                .ToList();

            Console.WriteLine($"Número de registros encontrados: {printerDetails.Count}");

            if (printerDetails.Any())
            {
                var firstRecord = printerDetails.First();
                var lastRecord = printerDetails.Last();

                Console.WriteLine($"Primeiro Registro: {firstRecord.QuantidadeImpressaoTotal}, Último Registro: {lastRecord.QuantidadeImpressaoTotal}");

                int? totalPrints = lastRecord.QuantidadeImpressaoTotal - firstRecord.QuantidadeImpressaoTotal;

                Console.WriteLine($"Total de Impressões: {totalPrints}");

                ViewBag.ImpressaoTotal = totalPrints; 
            }
            else
            {
                ViewBag.ImpressaoTotal = 0; 
                Console.WriteLine("Nenhum registro encontrado para este mês.");
            }

            ViewBag.PrinterDetails = _context.Printers.Find(printerId);

            return View();
        }
    }
}
