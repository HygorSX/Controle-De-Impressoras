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

            // Calculando as impress�es di�rias (diferente do �ltimo log)
            foreach (var log in reports)
            {
                // Encontrar o log anterior da mesma impressora
                var previousLog = _context.PrinterStatusLogs
                    .Where(psl => psl.PrinterId == log.PrinterId)
                    .Where(psl => psl.DataHoraDeBusca < log.DataHoraDeBusca)
                    .OrderByDescending(psl => psl.DataHoraDeBusca)
                    .FirstOrDefault();

                // Se houver um log anterior, calculamos a impress�o di�ria
                if (previousLog != null)
                {
                    log.QuantidadeImpressaoTotal = log.QuantidadeImpressaoTotal ?? 0;
                    previousLog.QuantidadeImpressaoTotal = previousLog.QuantidadeImpressaoTotal ?? 0;

                    log.QuantidadeImpressaoDiaria = log.QuantidadeImpressaoTotal - previousLog.QuantidadeImpressaoTotal;
                }
                else
                {
                    // Se n�o houver log anterior (primeiro log), ignoramos a impress�o di�ria
                    log.QuantidadeImpressaoDiaria = 0; // N�o contar o primeiro log
                }
            }

            // Calculando as impress�es mensais a partir das impress�es di�rias
            var monthlyReportsQuery = reports
                .GroupBy(r => new { r.DataHoraDeBusca.Year, r.DataHoraDeBusca.Month })  // Agrupa por ano e m�s
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalImpressaoMensal = g.Sum(r => r.QuantidadeImpressaoDiaria) // Soma as impress�es di�rias para o m�s
                })
                .OrderByDescending(g => g.Year) // Ordenando de forma reversa por ano
                .ThenByDescending(g => g.Month) // Ordenando de forma reversa por m�s
                .ToList();

            // Garantir que monthlyReports n�o seja nulo
            var monthlyReports = monthlyReportsQuery.Select(mr => new PrinterStatusLogMonthlyReportModel
            {
                Year = mr.Year,
                Month = mr.Month,
                TotalImpressaoMensal = mr.TotalImpressaoMensal ?? 0
            }).ToList();

            // Obtendo os detalhes da impressora, se necess�rio
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
            // Se algum desses par�metros for nulo, atribua valores padr�o
            if (!instituicaoId.HasValue)
            {
                instituicaoId = 0; // ou qualquer valor padr�o que fa�a sentido
            }

            if (string.IsNullOrEmpty(abrSecretaria))
            {
                abrSecretaria = "todos"; // ou outro valor default como 'nenhuma' ou 'n�o especificado'
            }

            if (string.IsNullOrEmpty(depto))
            {
                depto = "todos"; // ou outro valor default como 'nenhum' ou 'n�o especificado'
            }

            // Buscar as secretarias dinamicamente
            var secretariasQuery = _context.Printers.AsQueryable();

            // Filtra pela institui��o, se o ID for fornecido e v�lido
            if (instituicaoId.HasValue && instituicaoId.Value > 0)
            {
                secretariasQuery = secretariasQuery.Where(i => i.InstituicaoId == instituicaoId.Value);
            }

            // Busca as secretarias distintas
            var secretarias = secretariasQuery
                .Where(i => !string.IsNullOrEmpty(i.AbrSecretaria)) // Filtra secretarias n�o nulas
                .Select(i => i.AbrSecretaria) // Seleciona apenas o campo 'AbrSecretaria'
                .Distinct() // Garante que as secretarias sejam �nicas
                .OrderBy(s => s) // Ordena as secretarias em ordem alfab�tica
                .ToList();

            ViewBag.Secretarias = secretarias ?? new List<string>();

            // Se uma secretaria for selecionada, busca os departamentos dessa secretaria
            List<string> departamentos = new List<string>();

            if (!string.IsNullOrEmpty(abrSecretaria) && abrSecretaria != "todos")
            {
                departamentos = _context.Printers
                    .Where(p => p.AbrSecretaria == abrSecretaria)
                    .Select(p => p.Depto)  // Seleciona o campo 'Depto'
                    .Distinct()  // Garante que os departamentos sejam �nicos
                    .ToList();
            }

            // Passa a lista de departamentos para a View
            ViewBag.Departamentos = departamentos;

            // Inicia a consulta para os relat�rios de impress�es
            var query = from log in _context.PrinterStatusLogs
                        join printer in _context.Printers on log.PrinterId equals printer.Id
                        select new { log, printer };

            // Filtrando pela institui��o, se necess�rio
            if (instituicaoId.HasValue && instituicaoId.Value > 0)
            {
                query = query.Where(x => x.printer.InstituicaoId == instituicaoId.Value);
            }

            // Filtrando por data, se necess�rio
            if (startDate.HasValue)
            {
                query = query.Where(x => x.log.DataHoraDeBusca >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.log.DataHoraDeBusca <= endDate.Value);
            }

            // Filtrando pela abrevia��o da secretaria, se fornecido
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

                        // A impress�o di�ria � a diferen�a entre os logs
                        log.QuantidadeImpressaoDiaria = log.QuantidadeImpressaoTotal - previousLog.QuantidadeImpressaoTotal;
                    }
                    else
                    {
                        // Ignorando o primeiro log (sem c�lculo de impress�o di�ria)
                        log.QuantidadeImpressaoDiaria = 0; // N�o contamos o primeiro log
                    }

                    return log;
                }).ToList();

            // Calculando as impress�es mensais por impressora e agrupando
            var monthlyReportsQuery = reports
                .GroupBy(r => new { r.DataHoraDeBusca.Year, r.DataHoraDeBusca.Month })  // Agrupa por ano e m�s
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalImpressaoMensal = g.Sum(r => r.QuantidadeImpressaoDiaria), // Soma das impress�es di�rias
                    MonitoredPrintersCount = g.Select(r => r.PrinterId).Distinct().Count(), // Contagem de impressoras monitoradas no m�s
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
                    MonitoredPrintersCount = mr.MonitoredPrintersCount, // N�mero de impressoras monitoradas no m�s
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
