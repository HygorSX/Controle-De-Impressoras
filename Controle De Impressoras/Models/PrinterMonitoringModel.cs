using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_Impressoras.Models
{
    [Table("PrinterMonitoringTESTE")]
    public class PrinterMonitoringModel
    {
        [Key]
        public int Id { get; set; }

        public string Ip { get; set; }

        [Required(ErrorMessage = "O fabricante é obrigatório")]
        public string DeviceManufacturer { get; set; }

        [Required(ErrorMessage = "O nome do dispositivo é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O nome do dispositivo deve ter no mínimo 5 caracteres")]
        public string DeviceModel { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O modelo deve ter no mínimo 5 caracteres")]
        public string PrinterModel { get; set; }

        [Required(ErrorMessage = "O serial é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O serial deve ter no mínimo 5 caracteres")]
        public string SerialImpressora { get; set; }

        [Required(ErrorMessage = "O serial da unidade de imagem é obrigatório")]
        [MaxLength(100, ErrorMessage = "O comprimento máximo permitido é 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O serial da unidade de imagem deve ter no mínimo 5 caracteres")]
        public string SerialUniImage { get; set; }

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

        [Required(ErrorMessage = "O tipo da impressora é obrigatório")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "O patrimônio é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O patrimônio deve ser um número válido com no mínimo 1 dígitos")]
        public int? Patrimonio { get; set; }

        [Required(ErrorMessage = "A data e hora de busca são obrigatórias")]
        public DateTime DataHoraDeBusca { get; set; }
    }
}
