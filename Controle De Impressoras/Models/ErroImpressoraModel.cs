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

        [Required(ErrorMessage = "Institui��o � obrigat�ria.")]
        public int InstituicaoId { get; set; }

        [MaxLength(100, ErrorMessage = "Marca n�o pode ter mais que 100 caracteres.")]
        public string Marca { get; set; }

        [MaxLength(100, ErrorMessage = "Modelo n�o pode ter mais que 100 caracteres.")]
        public string Modelo { get; set; }

        [MaxLength(50, ErrorMessage = "IP n�o pode ter mais que 50 caracteres.")]
        public string Ip { get; set; }

        public int? Patrimonio { get; set; }

        [MaxLength(100, ErrorMessage = "Secretaria n�o pode ter mais que 100 caracteres.")]
        public string Secretaria { get; set; }

        [MaxLength(20, ErrorMessage = "Abrevia��o da Secretaria n�o pode ter mais que 20 caracteres.")]
        public string AbrSecretaria { get; set; }

        [MaxLength(100, ErrorMessage = "Departamento n�o pode ter mais que 100 caracteres.")]
        public string Depto { get; set; }

        [MaxLength(150, ErrorMessage = "Localiza��o n�o pode ter mais que 150 caracteres.")]
        public string Localizacao { get; set; }

        [MaxLength(500, ErrorMessage = "Motivo n�o pode ter mais que 500 caracteres.")]
        public string Motivo { get; set; }
    }
}
