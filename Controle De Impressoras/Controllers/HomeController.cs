using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;

namespace Controle_De_Impressoras.Controllers
{
    public class HomeController : Controller
    {

        private PrintersContext db = new PrintersContext();

        public static string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            texto = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in texto)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public ActionResult Index(string tipo, string marca, string modelo, int patrimonio = 0, bool? tintaBaixa = false, int? instituicaoId = null, bool apenasOffline = false, string AbrSecretaria = null, string localizacao = null, string departamento = null)
        {
            // Recupera as impressoras com base nos filtros gerais
            var impressoras = PrintersModel.RecuperarImpressoras(tipo, marca, modelo, patrimonio);

            // Aplica o filtro de tinta baixa se necessário
            if (tintaBaixa.HasValue && tintaBaixa.Value)
            {
                impressoras = impressoras.Where(i =>
                    i.PorcentagemBlack < 10 ||
                    (i.PorcentagemCyan < 10 && i.Tipo == "COLOR") ||
                    (i.PorcentagemMagenta < 10 && i.Tipo == "COLOR") ||
                    (i.PorcentagemYellow < 10 && i.Tipo == "COLOR")).ToList();
            }

            // Filtra as secretarias únicas
            var secretarias = impressoras
                .Where(i => !string.IsNullOrEmpty(i.AbrSecretaria))  // Filtra impressoras que possuem uma secretaria
                .Select(i => i.AbrSecretaria)  // Seleciona apenas o campo 'AbrSecretaria'
                .Distinct()  // Garante que as secretarias sejam únicas
                .OrderBy(s => s)  // Ordena as secretarias em ordem alfabética
                .ToList();

            // Passa as secretarias para a ViewBag
            ViewBag.Secretarias = secretarias;

            // Filtra os modelos únicos, da mesma forma que fizemos com os departamentos
            var modelos = impressoras
                .Where(i => !string.IsNullOrEmpty(i.PrinterModel))  // Filtra impressoras que possuem um modelo
                .Select(i => i.PrinterModel)  // Seleciona o campo 'Modelo'
                .Distinct()  // Garante que os modelos sejam únicos
                .OrderBy(m => m)  // Ordena os modelos em ordem alfabética
                .ToList();

            // Passa os modelos para a ViewBag
            ViewBag.Modelos = modelos;

            // Obtém os departamentos para a secretaria selecionada, similar ao que foi feito nos relatórios
            List<string> departamentos = new List<string>();
            if (!string.IsNullOrEmpty(AbrSecretaria))
            {
                departamentos = impressoras
                    .Where(i => i.AbrSecretaria == AbrSecretaria)  // Filtra impressoras pela Secretaria selecionada
                    .Select(i => i.Depto)  // Seleciona o campo 'Depto'
                    .Distinct()  // Garante que os departamentos sejam únicos
                    .OrderBy(d => d)  // Ordena os departamentos em ordem alfabética
                    .ToList();
            }

            // Passa os departamentos para a ViewBag
            ViewBag.Departamentos = departamentos;

            // Aplica o filtro de InstituicaoId se for fornecido
            if (instituicaoId.HasValue)
            {
                impressoras = impressoras.Where(i => i.InstituicaoId == instituicaoId.Value).ToList();
            }

            // Aplica o filtro de Secretaria se for fornecido
            if (!string.IsNullOrEmpty(AbrSecretaria))
            {
                impressoras = impressoras.Where(i => string.Equals(i.AbrSecretaria, AbrSecretaria, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Aplica o filtro apenasOffline se a flag for verdadeira
            if (apenasOffline)
            {
                // Filtro apenas para impressoras onde Ativa == 0 (offline)
                impressoras = impressoras.Where(i => i.Ativa == 0).ToList();
            }

            // Aplica o filtro de localizacao e departamento sem considerar acentuação
            if (!string.IsNullOrEmpty(localizacao) || !string.IsNullOrEmpty(departamento))
            {
                string localizacaoSemAcento = RemoverAcentos(localizacao);
                string departamentoSemAcento = RemoverAcentos(departamento);

                impressoras = impressoras.Where(i =>
                    (string.IsNullOrEmpty(localizacao) ||
                     RemoverAcentos(i.Localizacao).IndexOf(localizacaoSemAcento, StringComparison.OrdinalIgnoreCase) >= 0) &&
                    (string.IsNullOrEmpty(departamento) ||
                     RemoverAcentos(i.Depto).IndexOf(departamentoSemAcento, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (string.IsNullOrEmpty(localizacao) && string.IsNullOrEmpty(departamento))
                ).ToList();
            }

            // Aplica o filtro de modelo se fornecido
            if (!string.IsNullOrEmpty(modelo))
            {
                impressoras = impressoras.Where(i => string.Equals(i.PrinterModel, modelo, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Passa o total de impressoras para a ViewBag
            ViewBag.TotalImpressoras = impressoras.Count();

            // Passa os parâmetros para manter os filtros aplicados
            ViewBag.Tipo = tipo;
            ViewBag.Marca = marca;
            if (!string.IsNullOrEmpty(modelo)) ViewBag.Modelo = modelo;
            if (patrimonio != 0) ViewBag.Patrimonio = patrimonio;
            ViewBag.TintaBaixa = tintaBaixa;
            ViewBag.InstituicaoId = instituicaoId;
            ViewBag.ApenasOffline = apenasOffline;
            ViewBag.AbrSecretaria = AbrSecretaria;
            ViewBag.Localizacao = localizacao;  // Novo parâmetro para localização
            ViewBag.Departamento = departamento;  // Novo parâmetro para departamento

            return View(impressoras);
        }




        public ActionResult AcessoNegado()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public ActionResult Painel(string tipo, string marca, string modelo, int patrimonio = 0, bool tintaBaixa = false, bool intervalo1 = false, bool intervalo2 = false, bool intervalo3 = false, bool intervalo4 = false, bool intervalo5 = false, bool esgotado = false, int? instituicaoId = null, bool apenasOffline = false, string AbrSecretaria = null)
        {
            // Recupera as impressoras com base nos filtros gerais
            var impressoras = PrintersModel.RecuperarImpressoras(tipo, marca, modelo, patrimonio);

            var secretarias = impressoras
               .Where(i => !string.IsNullOrEmpty(i.AbrSecretaria))  // Filtra impressoras que possuem uma secretaria
               .Select(i => i.AbrSecretaria)  // Seleciona apenas o campo 'AbrSecretaria'
               .Distinct()  // Garante que as secretarias sejam únicas
               .OrderBy(s => s)  // Ordena as secretarias em ordem alfabética
               .ToList();

            // Passa as secretarias para a ViewBag
            ViewBag.Secretarias = secretarias;

            // Aplica o filtro de tinta baixa se necessário
            if (tintaBaixa)
            {
                impressoras = impressoras.Where(i => i.PorcentagemBlack < 10).ToList();
            }

            // Aplica o filtro de InstituicaoId se for fornecido
            if (instituicaoId.HasValue)
            {
                impressoras = impressoras.Where(i => i.InstituicaoId == instituicaoId.Value).ToList();
            }

            // Modifica para garantir que impressoras offline não apareçam por padrão
            if (apenasOffline)
            {
                // Filtro apenas para impressoras onde Ativa == 0 (offline)
                impressoras = impressoras.Where(i => i.Ativa == 0).ToList();
            }

            // Aplica o filtro de Secretaria se for fornecido
            if (!string.IsNullOrEmpty(AbrSecretaria))
            {
                impressoras = impressoras.Where(i => string.Equals(i.AbrSecretaria, AbrSecretaria, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Aplica os filtros dos intervalos se algum estiver selecionado
            if (!intervalo1 && !intervalo2 && !intervalo3 && !intervalo4 && !intervalo5 && !esgotado)
            {
                ViewBag.TotalImpressoras = impressoras.Count();
                return View(impressoras);
            }

            impressoras = impressoras.Where(i =>
                (intervalo1 && i.PorcentagemBlack <= 100 && i.PorcentagemBlack > 60) ||
                (intervalo2 && i.PorcentagemBlack <= 60 && i.PorcentagemBlack > 20) ||
                (intervalo3 && i.PorcentagemBlack <= 20 && i.PorcentagemBlack > 10) ||
                (intervalo4 && i.PorcentagemBlack <= 10 && i.PorcentagemBlack > 1) ||
                (esgotado && i.PorcentagemBlack == 0)
            ).ToList();

            // Passa o total de impressoras para a ViewBag
            ViewBag.TotalImpressoras = impressoras.Count();

            // Passa os parâmetros para manter os filtros aplicados
            ViewBag.Tipo = tipo;
            ViewBag.Marca = marca;
            ViewBag.Modelo = modelo;
            if (patrimonio != 0) {
                ViewBag.Patrimonio = patrimonio;
            }
            ViewBag.TintaBaixa = tintaBaixa;
            ViewBag.Intervalo1 = intervalo1;
            ViewBag.Intervalo2 = intervalo2;
            ViewBag.Intervalo3 = intervalo3;
            ViewBag.Intervalo4 = intervalo4;
            ViewBag.Intervalo5 = intervalo5;
            ViewBag.Esgotado = esgotado;
            ViewBag.InstituicaoId = instituicaoId;
            ViewBag.ApenasOffline = apenasOffline;
            ViewBag.AbrSecretaria = AbrSecretaria;

            return View(impressoras);
        }

        public JsonResult BuscarDepartamentos(string termo)
        {
            var departamentos = PrintersModel.RecuperarImpressoras(null, null, null, 0)
                .Where(i => !string.IsNullOrEmpty(i.Depto) && i.Depto.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(i => i.Depto)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            return Json(departamentos, JsonRequestBehavior.AllowGet);
        }


        public JsonResult BuscarLocalizacoes(string termo)
        {
            var localizacoes = PrintersModel.RecuperarImpressoras(null, null, null, 0)
                .Where(i => !string.IsNullOrEmpty(i.Localizacao) && i.Localizacao.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(i => i.Localizacao)
                .Distinct()
                .OrderBy(l => l)
                .ToList();

            return Json(localizacoes, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ErrosImpressoras(string Patrimonio, string Marca, string Modelo, string Ip, string AbrSecretaria, string Depto, string Localizacao, int? InstituicaoId)
        {
            // Inicializa a lista com todos os erros de impressora
            var errosImpressoras = db.ErrosImpressoras.AsQueryable();

            // Contagem do total de impressoras com erro (sem filtros)

            // Cria uma lista com as AbrSecretarias únicas
            var abrSecretarias = errosImpressoras
                .Where(e => !string.IsNullOrEmpty(e.AbrSecretaria)) // Filtra erros que possuem uma AbrSecretaria
                .Select(e => e.AbrSecretaria) // Seleciona o campo 'AbrSecretaria'
                .Distinct() // Garante que as AbrSecretarias sejam únicas
                .OrderBy(s => s) // Ordena as AbrSecretarias em ordem alfabética
                .ToList();

            // Cria uma lista com as Marcas únicas
            var marcas = errosImpressoras
                .Where(e => !string.IsNullOrEmpty(e.Marca)) // Filtra erros que possuem uma marca
                .Select(e => e.Marca) // Seleciona o campo 'Marca'
                .Distinct() // Garante que as marcas sejam únicas
                .OrderBy(m => m) // Ordena as marcas em ordem alfabética
                .ToList();

            // Cria uma lista com os Departamentos únicos baseados na AbrSecretaria
            var departamentos = errosImpressoras
                .Where(e => !string.IsNullOrEmpty(e.Depto)) // Filtra erros que possuem um departamento
                .Where(e => e.AbrSecretaria == AbrSecretaria) // Filtra departamentos pela AbrSecretaria selecionada
                .Select(e => e.Depto) // Seleciona o campo 'Depto'
                .Distinct() // Garante que os departamentos sejam únicos
                .OrderBy(d => d) // Ordena os departamentos em ordem alfabética
                .ToList();

            // Passa as AbrSecretarias, marcas, departamentos e o total de erros para o ViewBag
            ViewBag.AbrSecretarias = abrSecretarias;
            ViewBag.Marcas = marcas;
            ViewBag.Departamentos = departamentos;
            ViewBag.AbrSecretaria = AbrSecretaria;  // Passando para a View a Secretaria Selecionada
            ViewBag.Marca = Marca;  // Passando para a View a Marca Selecionada
            ViewBag.Depto = Depto;  // Passando para a View o Departamento Selecionado
            ViewBag.InstituicaoId = InstituicaoId; // Passando o id da Instituição para o View

            // Aplica os filtros, caso existam
            if (!string.IsNullOrEmpty(Marca))
                errosImpressoras = errosImpressoras.Where(e => e.Marca.Contains(Marca));

            if (!string.IsNullOrEmpty(Modelo))
                errosImpressoras = errosImpressoras.Where(e => e.Modelo.Contains(Modelo));

            if (!string.IsNullOrEmpty(Ip))
                errosImpressoras = errosImpressoras.Where(e => e.Ip.Contains(Ip));

            if (!string.IsNullOrEmpty(AbrSecretaria))
                errosImpressoras = errosImpressoras.Where(e => e.AbrSecretaria == AbrSecretaria);

            if (!string.IsNullOrEmpty(Depto))
                errosImpressoras = errosImpressoras.Where(e => e.Depto.Contains(Depto));

            if (!string.IsNullOrEmpty(Localizacao))
                errosImpressoras = errosImpressoras.Where(e => e.Localizacao.Contains(Localizacao));

            // Filtro para o campo InstituicaoId, caso fornecido
            if (InstituicaoId.HasValue)
                errosImpressoras = errosImpressoras.Where(e => e.InstituicaoId == InstituicaoId);

            // Para o campo Patrimonio que é int, fazemos a comparação com int, se o filtro for fornecido
            if (!string.IsNullOrEmpty(Patrimonio) && int.TryParse(Patrimonio, out var patrimonioInt))
            {
                errosImpressoras = errosImpressoras.Where(e => e.Patrimonio == patrimonioInt);
            }

            var totalErros = errosImpressoras.Count();

            // Converte o resultado para lista
            var resultado = errosImpressoras.ToList();
            ViewBag.TotalErros = totalErros;  // Passa a quantidade total de erros de impressora para a View

            return View(resultado);
        }

        public ActionResult DadosRelatorio(DateTime? dataInicio, DateTime? dataFim, string secretaria, string depto, int? instituicaoId)
        {
            // Definindo valores padrão para dataInicio e dataFim, caso sejam nulos
            if (!dataInicio.HasValue) dataInicio = DateTime.MinValue;
            if (!dataFim.HasValue) dataFim = DateTime.MaxValue;

            // Chamando o método para obter os dados do relatório
            List<DadosRelatorioModel> relatorio = PrintersModel.DadosRelatorio(dataInicio, dataFim, secretaria, depto, instituicaoId);

            // Passando os filtros e os dados para a view
            ViewBag.DataInicio = dataInicio;
            ViewBag.DataFim = dataFim;
            ViewBag.Secretaria = secretaria;
            ViewBag.Depto = depto;
            ViewBag.InstituicaoId = instituicaoId;

            // Retorna a view com os dados do relatório
            return View(relatorio);
        }


        public ActionResult DownloadRelatorio(DateTime? dataInicio, DateTime? dataFim, string secretaria, string depto, int? instituicaoId)
        {
            // Definindo valores padrão para dataInicio e dataFim, caso sejam nulos
            if (!dataInicio.HasValue) dataInicio = DateTime.MinValue;
            if (!dataFim.HasValue) dataFim = DateTime.MaxValue;

            // Chamando o método para obter os dados do relatório
            List<DadosRelatorioModel> relatorio = PrintersModel.DadosRelatorio(dataInicio, dataFim, secretaria, depto, instituicaoId);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Relatório");

                // Cabeçalhos do Excel
                worksheet.Cell(1, 1).Value = "Data e Hora de Busca";
                worksheet.Cell(1, 2).Value = "Patrimônio";
                worksheet.Cell(1, 3).Value = "Modelo";
                worksheet.Cell(1, 4).Value = "Secretaria";
                worksheet.Cell(1, 5).Value = "Depto";
                worksheet.Cell(1, 6).Value = "Tipo (ColorMono)";
                worksheet.Cell(1, 7).Value = "Impressões Totais";
                worksheet.Cell(1, 8).Value = "% Black";
                worksheet.Cell(1, 9).Value = "% Cyan";
                worksheet.Cell(1, 10).Value = "% Yellow";
                worksheet.Cell(1, 11).Value = "% Magenta";
                worksheet.Cell(1, 12).Value = "% Unidade Imagem";
                worksheet.Cell(1, 13).Value = "% Fusor";
                worksheet.Cell(1, 14).Value = "% Belt";
                worksheet.Cell(1, 15).Value = "% Kit Manutenção";
                worksheet.Cell(1, 16).Value = "Instituição";

                // Dados no Excel
                for (int i = 0; i < relatorio.Count; i++)
                {
                    var item = relatorio[i];
                    worksheet.Cell(i + 2, 1).Value = item.DataHoraDeBusca.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cell(i + 2, 2).Value = item.Patrimonio;
                    worksheet.Cell(i + 2, 3).Value = item.Modelo;
                    worksheet.Cell(i + 2, 4).Value = item.Secretaria;
                    worksheet.Cell(i + 2, 5).Value = item.Depto;
                    worksheet.Cell(i + 2, 6).Value = item.ColorMono;
                    worksheet.Cell(i + 2, 7).Value = item.ImpressoesTotais;
                    worksheet.Cell(i + 2, 8).Value = item.PorcentagemBlack;
                    worksheet.Cell(i + 2, 9).Value = item.PorcentagemCyan;
                    worksheet.Cell(i + 2, 10).Value = item.PorcentagemYellow;
                    worksheet.Cell(i + 2, 11).Value = item.PorcentagemMagenta;
                    worksheet.Cell(i + 2, 12).Value = item.UnidadeImagem;
                    worksheet.Cell(i + 2, 13).Value = item.Fusor;
                    worksheet.Cell(i + 2, 14).Value = item.Belt;
                    worksheet.Cell(i + 2, 15).Value = item.KitManutencao;
                    worksheet.Cell(i + 2, 16).Value = item.InstituicaoId;
                }

                worksheet.Columns().AdjustToContents();  // Ajusta as colunas

                // Criar o arquivo para download
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();
                    stream.Position = 0;  // Move o ponteiro do stream para o início

                    string excelName = $"RelatorioImpressoras-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    // Definir cabeçalhos de resposta HTTP para o download correto
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-Disposition", $"attachment; filename={excelName}");
                    Response.BinaryWrite(stream.ToArray());
                    Response.End();

                    return new EmptyResult();  // Retorna um resultado vazio após o download
                }
            }
        }

    }
}
