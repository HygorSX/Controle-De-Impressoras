using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_Impressoras.Models
{
    [Table("PrinterStatusLogs")]
    public class PrinterStatusLogModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O ID da impressora é obrigatório")]
        public int PrinterId { get; set; }

        [Required(ErrorMessage = "A quantidade de impressão total é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um número válido")]
        public int? QuantidadeImpressaoTotal { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemBlack { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemCyan { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemYellow { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemMagenta { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemFusor { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemBelt { get; set; }

        [Required(ErrorMessage = "A porcentagem é obrigatória")]
        [Range(0, 100, ErrorMessage = "A porcentagem deve estar entre 0 e 100")]
        public int? PorcentagemKitManutencao { get; set; }

        [Required(ErrorMessage = "O status da impressora é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(3, ErrorMessage = "O status da impressora deve ter no mínimo 3 caracteres")]
        public string PrinterStatus { get; set; }

        [Required(ErrorMessage = "A data e hora de busca são obrigatórias")]
        public DateTime DataHoraDeBusca { get; set; } 
        public int? PorcentagemUnidadeImagem { get; set; }
        public string SerialTonnerPreto { get; set; }
    }
}
