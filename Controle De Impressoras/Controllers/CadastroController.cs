using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using Controle_De_Impressoras.Utils;
using Controle_De_Impressoras.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class CadastroController : Controller
    {
        private readonly PrinterService _printerService;

        public CadastroController()
        {
            _printerService = new PrinterService(new PrintersContext());
        }

        [HttpGet]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult CadastrarImpressora()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult CadastrarImpressora(PrintersModel model)
        {
            if (ModelState.IsValid)
            {
                if (_printerService.AddPrinter(model))
                {
                    TempData["SuccessMessage"] = "Impressora cadastrada com sucesso!";
                    return RedirectToAction("CadastrarImpressora", "Cadastro");
                }
                else
                {
                    TempData["ErrorMessage"] = "Essa impressora já foi adicionada!";
                    ModelState.AddModelError("Impressora", "Essa impressora já foi adicionada");
                }
            }
            return View(model);
        }

    }
}