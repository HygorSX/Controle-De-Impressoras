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
        public ActionResult Index(string tipo, string marca, string modelo, int patrimonio = 0)
        {
            var impressoras = PrintersModel.RecuperarImpressoras(tipo, marca, modelo, patrimonio);
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
    }
}