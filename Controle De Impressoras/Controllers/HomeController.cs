using Controle_De_Impressoras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string tipo, string marca, string modelo, int patrimonio = 0, bool? tintaBaixa = false)
        {
            var impressoras = PrintersModel.RecuperarImpressoras(tipo, marca, modelo, patrimonio);

            if (tintaBaixa.HasValue && tintaBaixa.Value)
            {

                impressoras = impressoras.Where(i =>
                    i.PorcentagemBlack < 10 ||
                    (i.PorcentagemCyan < 10 && i.Tipo == "COLOR") ||
                    (i.PorcentagemMagenta < 10 && i.Tipo == "COLOR") ||
                    (i.PorcentagemYellow < 10 && i.Tipo == "COLOR")).ToList();
            }

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

        public ActionResult Painel(string tipo, string marca, string modelo, int patrimonio = 0, bool tintaBaixa = false, bool intervalo1 = false, bool intervalo2 = false, bool intervalo3 = false, bool esgotado = false)
        {
            var impressoras = PrintersModel.RecuperarImpressoras(tipo, marca, modelo, patrimonio);

            if (tintaBaixa)
            {
                impressoras = impressoras.Where(i => i.PorcentagemBlack < 10).ToList();
            }

            if (!intervalo1 && !intervalo2 && !intervalo3 && !esgotado)
            {
                return View(impressoras);
            }

            impressoras = impressoras.Where(i =>
                (intervalo1 && i.PorcentagemBlack <= 100 && i.PorcentagemBlack > 60) ||
                (intervalo2 && i.PorcentagemBlack <= 60 && i.PorcentagemBlack > 20) ||
                (intervalo3 && i.PorcentagemBlack <= 20 && i.PorcentagemBlack > 1) ||
                (esgotado && i.PorcentagemBlack == 0)
            ).ToList();

            return View(impressoras);
        }



    }
}
