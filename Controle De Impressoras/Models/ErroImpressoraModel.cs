using Controle_De_Impressoras.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_Impressoras.Models
{
    [Table("ErrosImpressoras")]
    public class ErroImpressora
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Instituição é obrigatória.")]
        public int InstituicaoId { get; set; }

        [MaxLength(100, ErrorMessage = "Marca não pode ter mais que 100 caracteres.")]
        public string Marca { get; set; }

        [MaxLength(100, ErrorMessage = "Modelo não pode ter mais que 100 caracteres.")]
        public string Modelo { get; set; }

        [MaxLength(50, ErrorMessage = "IP não pode ter mais que 50 caracteres.")]
        public string Ip { get; set; }

        public int? Patrimonio { get; set; }

        [MaxLength(100, ErrorMessage = "Secretaria não pode ter mais que 100 caracteres.")]
        public string Secretaria { get; set; }

        [MaxLength(20, ErrorMessage = "Abreviação da Secretaria não pode ter mais que 20 caracteres.")]
        public string AbrSecretaria { get; set; }

        [MaxLength(100, ErrorMessage = "Departamento não pode ter mais que 100 caracteres.")]
        public string Depto { get; set; }

        [MaxLength(150, ErrorMessage = "Localização não pode ter mais que 150 caracteres.")]
        public string Localizacao { get; set; }

        [MaxLength(500, ErrorMessage = "Motivo não pode ter mais que 500 caracteres.")]
        public string Motivo { get; set; }
    }
}
