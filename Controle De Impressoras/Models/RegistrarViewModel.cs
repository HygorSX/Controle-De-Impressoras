using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Controle_De_Impressoras.Models
{
    [Table("UserPrinters")]
    public class RegistrarViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A confirmação de senha é obrigatória.")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senha não coincidem.")]
        public string ConfirmarSenha { get; set; }
    }
}
