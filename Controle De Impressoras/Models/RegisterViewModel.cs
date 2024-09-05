using System.ComponentModel.DataAnnotations;

namespace Controle_De_Impressoras.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O campo Usu�rio � obrigat�rio.")]
        [Display(Name = "Usu�rio")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "O campo Senha � obrigat�rio.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A confirma��o da senha � obrigat�ria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "As senhas n�o coincidem.")]
        public string ConfirmarSenha { get; set; }
    }
}
