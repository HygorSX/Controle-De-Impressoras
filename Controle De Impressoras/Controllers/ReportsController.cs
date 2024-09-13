using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web.Mvc;
using System.Drawing;

namespace Controle_De_Impressoras.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports/GenerateExcel
        public ActionResult GenerateExcel(int? printerId, DateTime? startDate, DateTime? endDate)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var context = new PrintersContext())
            {
                // Recupera os relatórios com base no filtro de datas e printerId
                var reports = context.DailyReports
                    .Where(r => (!printerId.HasValue || r.PrinterId == printerId) &&
                                (!startDate.HasValue || r.ReportDate >= startDate.Value) &&
                                (!endDate.HasValue || r.ReportDate <= endDate.Value))
                    .ToList();

                if (reports == null || !reports.Any())
                {
                    return HttpNotFound();
                }

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Relatórios");

                    // Configura o cabeçalho
                    worksheet.Cells["A1"].Value = "Relatórios de Impressoras";
                    worksheet.Cells["A1:R1"].Merge = true;
                    worksheet.Cells["A1"].Style.Font.Size = 16;
                    worksheet.Cells["A1"].Style.Font.Bold = true;
                    worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells["A2"].Value = $"Data: {DateTime.Now:dd/MM/yyyy}";
                    worksheet.Cells["A2:R2"].Merge = true;
                    worksheet.Cells["A2"].Style.Font.Size = 12;
                    worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Configura os cabeçalhos da tabela
                    var headers = new[]
                    {
                        "ID", "Printer ID", "Data do Relatório", "Fabricante", "Modelo do Dispositivo",
                        "Modelo da Impressora", "Número de Série da Impressora", "Número de Série da Unidade de Imagem",
                        "Quantidade Total de Impressões", "Porcentagem Preto", "Porcentagem Ciano", "Porcentagem Amarelo",
                        "Porcentagem Magenta", "Porcentagem Fusor", "Porcentagem Correia", "Porcentagem Kit de Manutenção",
                        "Status da Impressora", "Tipo", "Patrimônio"
                    };

                    // Inserir os cabeçalhos na planilha
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[4, i + 1].Value = headers[i];
                        worksheet.Cells[4, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[4, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[4, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }

                    // Preenche a tabela com os dados dos relatórios
                    int row = 5;
                    foreach (var report in reports)
                    {
                        worksheet.Cells[row, 1].Value = report.ReportId;
                        worksheet.Cells[row, 2].Value = report.PrinterId;
                        worksheet.Cells[row, 3].Value = report.ReportDate.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 4].Value = report.DeviceManufacturer;
                        worksheet.Cells[row, 5].Value = report.DeviceModel;
                        worksheet.Cells[row, 6].Value = report.PrinterModel;
                        worksheet.Cells[row, 7].Value = report.SerialImpressora;
                        worksheet.Cells[row, 8].Value = report.SerialUniImage;
                        worksheet.Cells[row, 9].Value = report.QuantidadeImpressaoTotal;
                        worksheet.Cells[row, 10].Value = report.PorcentagemBlack;
                        worksheet.Cells[row, 11].Value = report.PorcentagemCyan;
                        worksheet.Cells[row, 12].Value = report.PorcentagemYellow;
                        worksheet.Cells[row, 13].Value = report.PorcentagemMagenta;
                        worksheet.Cells[row, 14].Value = report.PorcentagemFusor;
                        worksheet.Cells[row, 15].Value = report.PorcentagemBelt;
                        worksheet.Cells[row, 16].Value = report.PorcentagemKitManutencao;
                        worksheet.Cells[row, 17].Value = report.PrinterStatus;
                        worksheet.Cells[row, 18].Value = report.Tipo;
                        worksheet.Cells[row, 19].Value = report.Patrimonio;

                        row++;
                    }

                    // Ajusta a largura das colunas automaticamente
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Cria o arquivo Excel
                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0; // Reseta a posição do stream para o início

                    // Retorna o arquivo Excel como um download
                    return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = "Relatorios_Impressora.xlsx"
                    };
                }
            }
        }

        // GET: Reports
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int printerId = 0)
        {
            using (var context = new PrintersContext())
            {
                // Recupera relatórios com base nos filtros de datas e printerId
                var reportsQuery = context.DailyReports
                    .Where(r => (printerId == 0 || r.PrinterId == printerId) &&
                                (!startDate.HasValue || r.ReportDate >= startDate.Value) &&
                                (!endDate.HasValue || r.ReportDate <= endDate.Value));

                // Adiciona ordenação condicional
                if (printerId == 0)
                {
                    reportsQuery = reportsQuery.OrderBy(r => r.PrinterId);
                }

                var reports = reportsQuery.ToList();

                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.PrinterId = printerId;

                return View(reports);
            }
        }


        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                if (report == null)
                {
                    return HttpNotFound();
                }
                return View(report);
            }
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportModel report)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintersContext())
                {
                    context.DailyReports.Add(report);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(report);
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                if (report == null)
                {
                    return HttpNotFound();
                }
                return View(report);
            }
        }

        // POST: Reports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReportModel report)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintersContext())
                {
                    context.Entry(report).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(report);
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                if (report == null)
                {
                    return HttpNotFound();
                }
                return View(report);
            }
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                context.DailyReports.Remove(report);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
