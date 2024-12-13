using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_Impressoras.Models
{
    [Table("PrinterStatusLogs")]
    public class PrinterStatusLogModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID da impressora � obrigat�rio")]
        public int PrinterId { get; set; }

        [Required(ErrorMessage = "A quantidade de impress�o total � obrigat�ria")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um n�mero v�lido")]
        public int? QuantidadeImpressaoTotal { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemBlack { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemCyan { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemYellow { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemMagenta { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemFusor { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemBelt { get; set; }

        [Required(ErrorMessage = "A porcentagem � obrigat�ria")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemKitManutencao { get; set; }

        [Required(ErrorMessage = "O status da impressora � obrigat�rio")]
        [MaxLength(100, ErrorMessage = "O comprimento m�ximo permitido � 100 caracteres.")]
        [MinLength(3, ErrorMessage = "O status da impressora deve ter no m�nimo 3 caracteres")]
        public string PrinterStatus { get; set; }

        [Required(ErrorMessage = "A data e hora de busca s�o obrigat�rias")]
        public DateTime DataHoraDeBusca { get; set; }

        public int? PorcentagemUnidadeImagem { get; set; }
        public string SerialTonnerPreto { get; set; }

        // Adicionando a propriedade para impress�o di�ria
        [NotMapped]  // A propriedade n�o ser� mapeada para a base de dados
        public int? QuantidadeImpressaoDiaria { get; set; }


        public class ReportsViewModel
        {
            public List<PrinterStatusLogModel> Reports { get; set; }  // Relat�rios Di�rios
            public List<MonthlyReportModel> MonthlyReports { get; set; }  // Relat�rios Mensais
        }

        public class MonthlyReportModel
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int? TotalImpressaoMensal { get; set; }
            public int? MaxPrinterPatrimonio { get; set; }
            public int? MonitoredPrintersCount { get; set; }
        }
    }
}
