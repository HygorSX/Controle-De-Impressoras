using Controle_De_Impressoras.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class HomeController : Controller
    {
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

            var secretarias = impressoras
                .Where(i => !string.IsNullOrEmpty(i.AbrSecretaria))  // Filtra impressoras que possuem uma secretaria
                .Select(i => i.AbrSecretaria)  // Seleciona apenas o campo 'AbrSecretaria'
                .Distinct()  // Garante que as secretarias sejam únicas
                .OrderBy(s => s)  // Ordena as secretarias em ordem alfabética
                .ToList();

            // Passa as secretarias para a ViewBag
            ViewBag.Secretarias = secretarias;

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

            // Passa o total de impressoras para a ViewBag
            ViewBag.TotalImpressoras = impressoras.Count();

            // Passa os parâmetros para manter os filtros aplicados
            ViewBag.Tipo = tipo;
            ViewBag.Marca = marca;
            ViewBag.Modelo = modelo;
            if (patrimonio != 0)
            {
                ViewBag.Patrimonio = patrimonio;
            }
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


    }
}
