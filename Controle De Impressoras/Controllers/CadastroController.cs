using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class CadastroController : Controller
    {
        [HttpGet]
        public ActionResult CadastrarImpressora()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult CadastrarImpressora(PrintersModel model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintersContext())
                {
                    bool printerExists = context.Printers.Any(u => u.SerialImpressora == model.SerialImpressora);

                    if (printerExists)
                    {
                        ModelState.AddModelError("Impressora", "Essa impressora já foi adicionada");
                        return View(model);
                    }

                    var printer = new PrintersModel
                    {
                        DeviceManufacturer = model.DeviceManufacturer,
                        DeviceModel = model.DeviceModel,
                        PrinterModel = model.PrinterModel,
                        SerialImpressora = model.SerialImpressora,
                        SerialUniImage = model.SerialUniImage,
                        QuantidadeImpressaoTotal = model.QuantidadeImpressaoTotal,
                        PorcentagemBlack = model.PorcentagemBlack,
                        PorcentagemCyan = model.PorcentagemCyan,
                        PorcentagemYellow = model.PorcentagemYellow,
                        PorcentagemMagenta = model.PorcentagemMagenta,
                        PorcentagemFusor = model.PorcentagemFusor,
                        PorcentagemBelt = model.PorcentagemBelt,
                        PorcentagemKitManutencao = model.PorcentagemKitManutencao,
                        PrinterStatus = model.PrinterStatus,
                        Tipo = model.Tipo,
                        Patrimonio = model.Patrimonio,
                        DataHoraDeBusca = model.DataHoraDeBusca,
                    };

                    context.Printers.Add(printer);
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Impressora cadastrada com sucesso!";
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

    }
}