using Controle_De_Impressoras.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Controle_De_Impressoras.Models
{
    [Table("DailyReports")]
    public class ReportModel
    {
        [Key]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "O ID da impressora é obrigatório")]
        public int PrinterId { get; set; }

        [Required(ErrorMessage = "A data do relatório é obrigatória")]
        public DateTime ReportDate { get; set; }

        public string DeviceManufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string PrinterModel { get; set; }
        public string SerialImpressora { get; set; }
        public string SerialUniImage { get; set; }
        public int? QuantidadeImpressaoTotal { get; set; }
        public int? PorcentagemBlack { get; set; }
        public int? PorcentagemCyan { get; set; }
        public int? PorcentagemYellow { get; set; }
        public int? PorcentagemMagenta { get; set; }
        public int? PorcentagemFusor { get; set; }
        public int? PorcentagemBelt { get; set; }
        public int? PorcentagemKitManutencao { get; set; }
        public string PrinterStatus { get; set; }
        public string Tipo { get; set; }
        public int? Patrimonio { get; set; }

        public static List<ReportModel> RecuperarRelatorios(DateTime? startDate = null, DateTime? endDate = null, int? printerId = null)
        {
            using (var context = new PrintersContext())
            {
                var query = context.DailyReports.AsQueryable();

                if (startDate.HasValue)
                {
                    query = query.Where(r => r.ReportDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(r => r.ReportDate <= endDate.Value);
                }

                if (printerId.HasValue)
                {
                    query = query.Where(r => r.PrinterId == printerId.Value);
                }

                    return query
                    .OrderBy(r => r.PrinterId)
                    .ToList();
            }
        }
    }
}
