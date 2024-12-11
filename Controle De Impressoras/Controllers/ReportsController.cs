using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;

namespace Controle_De_Impressoras.Controllers
{
    public class ReportsController : Controller
    {
        private readonly PrintersContext _context = new PrintersContext();

        public ActionResult Index(DateTime? startDate, DateTime? endDate, int? printerId)
        {
            // Inicializa a consulta base
            var query = _context.PrinterStatusLogs.AsQueryable();

            // Filtrando por data
            if (startDate.HasValue)
            {
                query = query.Where(r => r.DataHoraDeBusca >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.DataHoraDeBusca <= endDate.Value);
            }

            // Filtrando por impressora
            if (printerId.HasValue && printerId.Value > 0)
            {
                query = query.Where(r => r.PrinterId == printerId.Value);
            }

            // Pegando todos os logs filtrados e ordenando por DataHoraDeBusca de forma reversa (do mais recente para o mais antigo)
            var reports = query.OrderByDescending(r => r.DataHoraDeBusca).ToList();

            // Calculando as impressões diárias (diferente do último log)
            foreach (var log in reports)
            {
                // Encontrar o log anterior da mesma impressora
                var previousLog = _context.PrinterStatusLogs
                    .Where(psl => psl.PrinterId == log.PrinterId)
                    .Where(psl => psl.DataHoraDeBusca < log.DataHoraDeBusca)
                    .OrderByDescending(psl => psl.DataHoraDeBusca)
                    .FirstOrDefault();

                // Se houver um log anterior, calculamos a impressão diária
                if (previousLog != null)
                {
                    log.QuantidadeImpressaoTotal = log.QuantidadeImpressaoTotal ?? 0;
                    previousLog.QuantidadeImpressaoTotal = previousLog.QuantidadeImpressaoTotal ?? 0;

                    log.QuantidadeImpressaoDiaria = log.QuantidadeImpressaoTotal - previousLog.QuantidadeImpressaoTotal;
                }
                else
                {
                    // Se não houver log anterior (primeiro log), ignoramos a impressão diária
                    log.QuantidadeImpressaoDiaria = 0; // Não contar o primeiro log
                }
            }

            // Calculando as impressões mensais a partir das impressões diárias
            var monthlyReportsQuery = reports
                .GroupBy(r => new { r.DataHoraDeBusca.Year, r.DataHoraDeBusca.Month })  // Agrupa por ano e mês
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalImpressaoMensal = g.Sum(r => r.QuantidadeImpressaoDiaria) // Soma as impressões diárias para o mês
                })
                .OrderByDescending(g => g.Year) // Ordenando de forma reversa por ano
                .ThenByDescending(g => g.Month) // Ordenando de forma reversa por mês
                .ToList();

            // Garantir que monthlyReports não seja nulo
            var monthlyReports = monthlyReportsQuery.Select(mr => new PrinterStatusLogMonthlyReportModel
            {
                Year = mr.Year,
                Month = mr.Month,
                TotalImpressaoMensal = mr.TotalImpressaoMensal ?? 0
            }).ToList();

            // Obtendo os detalhes da impressora, se necessário
            var printerDetails = _context.Printers.FirstOrDefault(p => p.Id == printerId);

            // Criando o ViewModel para passar para a View
            var viewModel = new PrinterStatusLogModelView
            {
                Reports = reports,
                MonthlyReports = monthlyReports
            };

            // Passando os dados para a View
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.PrinterId = printerId;
            ViewBag.PrinterDetails = printerDetails;

            // Retornando a View com o ViewModel
            return View(viewModel);
        }


        public ActionResult InstitutionReports(int? instituicaoId, DateTime? startDate, DateTime? endDate)
        {
            // Se instituicaoId não for fornecido, podemos redirecionar para um formulário de entrada
            if (!instituicaoId.HasValue || instituicaoId.Value == 0)
            {
                // Redireciona para a mesma página com o formulário para escolher a instituição
                ViewBag.InstituicaoId = null;  // Informa que o ID da instituição não foi passado
                return View();  // Carrega a página inicial com o formulário
            }

            // Caso o instituicaoId tenha sido passado, então executa a consulta e carrega os relatórios
            var query = from log in _context.PrinterStatusLogs
                        join printer in _context.Printers on log.PrinterId equals printer.Id
                        where printer.InstituicaoId == instituicaoId // Filtrando pela instituição
                        select new
                        {
                            log,
                            printer
                        };

            // Filtrando por data, se necessário
            if (startDate.HasValue)
            {
                query = query.Where(x => x.log.DataHoraDeBusca >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.log.DataHoraDeBusca <= endDate.Value);
            }

            // Ordenando pela data e carregando os logs
            var reports = query
                .OrderBy(x => x.log.DataHoraDeBusca)
                .ToList()
                .Select(x =>
                {
                    var log = x.log;
                    var previousLog = _context.PrinterStatusLogs
                        .Where(psl => psl.PrinterId == log.PrinterId)
                        .Where(psl => psl.DataHoraDeBusca < log.DataHoraDeBusca)
                        .OrderByDescending(psl => psl.DataHoraDeBusca)
                        .FirstOrDefault();

                    // Ignorando o primeiro log de cada impressora
                    if (previousLog != null)
                    {
                        log.QuantidadeImpressaoTotal = log.QuantidadeImpressaoTotal ?? 0;
                        previousLog.QuantidadeImpressaoTotal = previousLog.QuantidadeImpressaoTotal ?? 0;

                        // A impressão diária é a diferença entre os logs
                        log.QuantidadeImpressaoDiaria = log.QuantidadeImpressaoTotal - previousLog.QuantidadeImpressaoTotal;
                    }
                    else
                    {
                        // Ignorando o primeiro log (sem cálculo de impressão diária)
                        log.QuantidadeImpressaoDiaria = 0; // Não contamos o primeiro log
                    }

                    return log;
                }).ToList();

            // Calculando as impressões mensais por impressora e agrupando
            var monthlyReportsQuery = reports
                .GroupBy(r => new { r.DataHoraDeBusca.Year, r.DataHoraDeBusca.Month })  // Agrupa por ano e mês
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalImpressaoMensal = g.Sum(r => r.QuantidadeImpressaoDiaria) // Soma das impressões diárias
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToList();

            // Criando o ViewModel para passar para a View
            var viewModel = new PrinterStatusLogModel.ReportsViewModel
            {
                MonthlyReports = monthlyReportsQuery.Select(mr => new PrinterStatusLogModel.MonthlyReportModel
                {
                    Year = mr.Year,
                    Month = mr.Month,
                    TotalImpressaoMensal = mr.TotalImpressaoMensal ?? 0
                }).ToList()
            };

            // Passando os dados para a View
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.InstituicaoId = instituicaoId;

            // Retornando a View com o ViewModel
            return View(viewModel);
        }

    }
}
