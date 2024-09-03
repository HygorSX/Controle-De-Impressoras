using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Controle_De_Impressoras.Models
{
    [Table("UserPrinters")]
    public class LoginViewModel
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o usuário")]
        [Display(Name = "Usuário:")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha:")]
        public string Senha { get; set; }


        [Display(Name = "Lembrar-me")]
        public bool LembrarMe { get; set; }
    }
}