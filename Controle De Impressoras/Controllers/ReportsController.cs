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

        public ActionResult InstitutionReports(int? instituicaoId, DateTime? startDate, DateTime? endDate, string abrSecretaria, string depto)
        {
            // Se algum desses parâmetros for nulo, atribua valores padrão
            if (!instituicaoId.HasValue)
            {
                instituicaoId = 0; // ou qualquer valor padrão que faça sentido
            }

            if (string.IsNullOrEmpty(abrSecretaria))
            {
                abrSecretaria = "todos"; // ou outro valor default como 'nenhuma' ou 'não especificado'
            }

            if (string.IsNullOrEmpty(depto))
            {
                depto = "todos"; // ou outro valor default como 'nenhum' ou 'não especificado'
            }

            // Buscar as secretarias dinamicamente
            var secretariasQuery = _context.Printers.AsQueryable();

            // Filtra pela instituição, se o ID for fornecido e válido
            if (instituicaoId.HasValue && instituicaoId.Value > 0)
            {
                secretariasQuery = secretariasQuery.Where(i => i.InstituicaoId == instituicaoId.Value);
            }

            // Busca as secretarias distintas
            var secretarias = secretariasQuery
                .Where(i => !string.IsNullOrEmpty(i.AbrSecretaria)) // Filtra secretarias não nulas
                .Select(i => i.AbrSecretaria) // Seleciona apenas o campo 'AbrSecretaria'
                .Distinct() // Garante que as secretarias sejam únicas
                .OrderBy(s => s) // Ordena as secretarias em ordem alfabética
                .ToList();

            ViewBag.Secretarias = secretarias ?? new List<string>();

            // Se uma secretaria for selecionada, busca os departamentos dessa secretaria
            List<string> departamentos = new List<string>();

            if (!string.IsNullOrEmpty(abrSecretaria) && abrSecretaria != "todos")
            {
                departamentos = _context.Printers
                    .Where(p => p.AbrSecretaria == abrSecretaria)
                    .Select(p => p.Depto)  // Seleciona o campo 'Depto'
                    .Distinct()  // Garante que os departamentos sejam únicos
                    .ToList();
            }

            // Passa a lista de departamentos para a View
            ViewBag.Departamentos = departamentos;

            // Inicia a consulta para os relatórios de impressões
            var query = from log in _context.PrinterStatusLogs
                        join printer in _context.Printers on log.PrinterId equals printer.Id
                        select new { log, printer };

            // Filtrando pela instituição, se necessário
            if (instituicaoId.HasValue && instituicaoId.Value > 0)
            {
                query = query.Where(x => x.printer.InstituicaoId == instituicaoId.Value);
            }

            // Filtrando por data, se necessário
            if (startDate.HasValue)
            {
                query = query.Where(x => x.log.DataHoraDeBusca >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.log.DataHoraDeBusca <= endDate.Value);
            }

            // Filtrando pela abreviação da secretaria, se fornecido
            if (!string.IsNullOrEmpty(abrSecretaria) && abrSecretaria != "todos")
            {
                query = query.Where(x => x.printer.AbrSecretaria != null && x.printer.AbrSecretaria.Contains(abrSecretaria));
            }

            // Filtrando pelo departamento, se fornecido
            if (!string.IsNullOrEmpty(depto) && depto != "todos")
            {
                query = query.Where(x => x.printer.Depto != null && x.printer.Depto.Contains(depto));
            }

            // Verifica se a consulta resultou em dados antes de prosseguir
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
                        // Se for nulo, define como 0
                        if (log.QuantidadeImpressaoTotal == null)
                        {
                            log.QuantidadeImpressaoTotal = 0;
                        }

                        if (previousLog.QuantidadeImpressaoTotal == null)
                        {
                            previousLog.QuantidadeImpressaoTotal = 0;
                        }

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
                    TotalImpressaoMensal = g.Sum(r => r.QuantidadeImpressaoDiaria), // Soma das impressões diárias
                    MonitoredPrintersCount = g.Select(r => r.PrinterId).Distinct().Count(), // Contagem de impressoras monitoradas no mês
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
                    TotalImpressaoMensal = mr.TotalImpressaoMensal ?? 0,
                    MonitoredPrintersCount = mr.MonitoredPrintersCount, // Número de impressoras monitoradas no mês
                }).ToList()
            };

            // Passando os dados para a View
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.InstituicaoId = instituicaoId;
            ViewBag.AbrSecretaria = abrSecretaria; // Passando o valor do filtro para a view
            ViewBag.Depto = depto; // Passando o valor do filtro para a view

            // Retornando a View com o ViewModel
            return View(viewModel);
        }
    }
}
