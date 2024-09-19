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

        [Required(ErrorMessage = "O fabricante � obrigat�rio")]
        public string DeviceManufacturer { get; set; }

        [Required(ErrorMessage = "O nome do dispositivo � obrigat�rio")]
        [MaxLength(100, ErrorMessage = "O comprimento m�ximo permitido � 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O nome do dispositivo deve ter no m�nimo 5 caracteres")]
        public string DeviceModel { get; set; }

        [Required(ErrorMessage = "O modelo � obrigat�rio")]
        [MaxLength(100, ErrorMessage = "O comprimento m�ximo permitido � 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O modelo deve ter no m�nimo 5 caracteres")]
        public string PrinterModel { get; set; }

        [Required(ErrorMessage = "O serial � obrigat�rio")]
        [MaxLength(100, ErrorMessage = "O comprimento m�ximo permitido � 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O serial deve ter no m�nimo 5 caracteres")]
        public string SerialImpressora { get; set; }

        [Required(ErrorMessage = "O serial da unidade de imagem � obrigat�rio")]
        [MaxLength(100, ErrorMessage = "O comprimento m�ximo permitido � 100 caracteres.")]
        [MinLength(5, ErrorMessage = "O serial da unidade de imagem deve ter no m�nimo 5 caracteres")]
        public string SerialUniImage { get; set; }

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

        [Required(ErrorMessage = "O tipo da impressora � obrigat�rio")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "O patrim�nio � obrigat�rio")]
        [Range(1, int.MaxValue, ErrorMessage = "O patrim�nio deve ser um n�mero v�lido com no m�nimo 1 d�gitos")]
        public int? Patrimonio { get; set; }

        [Required(ErrorMessage = "A data e hora de busca s�o obrigat�rias")]
        public DateTime DataHoraDeBusca { get; set; }
    }
}
