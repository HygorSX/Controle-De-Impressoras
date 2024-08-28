using Controle_De_Impressoras.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Controle_De_Impressoras.Models
{
    [Table("PrinterMonitoringTESTE")]
    public class PrintersModel
    {
        public int Id { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string PrinterModel { get; set; }
        public string SerialImpressora { get; set; }
        public string SerialUniImage { get; set; }
        public int QuantidadeImpressaoTotal { get; set; }
        public int PorcentagemBlack { get; set; }
        public int PorcentagemCyan { get; set; }
        public int PorcentagemYellow { get; set; }
        public int PorcentagemMagenta { get; set; }
        public int PorcentagemFusor { get; set; }
        public int PorcentagemBelt { get; set; }
        public int PorcentagemKitManutencao { get; set; }
        public string PrinterStatus { get; set; }
        public string Tipo { get; set; }
        public int? Patrimonio { get; set; }
        public DateTime DataHoraDeBusca { get; set; }

        public static List<PrintersModel> RecuperarImpressoras(string tipo = null, string marca = null, string modelo = null, int patrimonio = 0)
        {
            using (var context = new PrintersContext())
            {
                var query = context.Printers.AsQueryable();

                if (!string.IsNullOrEmpty(tipo))
                {
                    query = query.Where(p => p.Tipo.Contains(tipo));
                }

                if (!string.IsNullOrEmpty(marca))
                {
                    query = query.Where(p => p.DeviceManufacturer.Contains(marca));
                }

                if (!string.IsNullOrEmpty(modelo))
                {
                    query = query.Where(p => p.PrinterModel.Contains(modelo));
                }

                if (patrimonio != 0)
                {
                    query = query.Where(p => p.Patrimonio == patrimonio);
                }

                return query.OrderBy(p => p.DeviceManufacturer).ToList();
            }
        }

    }
}