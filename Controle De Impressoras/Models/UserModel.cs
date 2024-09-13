using Controle_De_Impressoras.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

[Table("UserPrinters")]
public class UserModel
{
    public int Id { get; set; }
    public string Usuario { get; set; }
    public string Senha { get; set; }
    public string Role { get; set; }
}
