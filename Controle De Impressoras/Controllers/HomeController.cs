﻿using Controle_De_Impressoras.Models;
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
            // Recupera as impressoras com base nos filtros gerais
            var impressoras = PrintersModel.RecuperarImpressoras(tipo, marca, modelo, patrimonio);

            // Aplica o filtro de tinta baixa se necessário
            if (tintaBaixa)
            {
                impressoras = impressoras.Where(i => i.PorcentagemBlack < 10).ToList();
            }

            // Aplica os filtros dos intervalos se algum estiver selecionado
            if (!intervalo1 && !intervalo2 && !intervalo3 && !esgotado)
            {
                return View(impressoras);  // Retorna sem aplicar outros filtros se nenhum intervalo foi selecionado
            }

            // Aplica os filtros específicos para os intervalos e "esgotado"
            impressoras = impressoras.Where(i =>
                (intervalo1 && i.PorcentagemBlack <= 100 && i.PorcentagemBlack > 60) ||
                (intervalo2 && i.PorcentagemBlack <= 60 && i.PorcentagemBlack > 20) ||
                (intervalo3 && i.PorcentagemBlack <= 20 && i.PorcentagemBlack > 1) ||
                (esgotado && i.PorcentagemBlack == 0)
            ).ToList();

            // Retorna a lista filtrada junto com os parâmetros para manter os filtros aplicados
            ViewBag.Tipo = tipo;
            ViewBag.Marca = marca;
            ViewBag.Modelo = modelo;
            ViewBag.Patrimonio = patrimonio;
            ViewBag.TintaBaixa = tintaBaixa;
            ViewBag.Intervalo1 = intervalo1;
            ViewBag.Intervalo2 = intervalo2;
            ViewBag.Intervalo3 = intervalo3;
            ViewBag.Esgotado = esgotado;

            return View(impressoras);
        }



    }
}
