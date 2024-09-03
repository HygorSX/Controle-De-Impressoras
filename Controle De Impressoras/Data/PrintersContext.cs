using Controle_De_Impressoras.Models;
using System.Data.Entity;

public class PrintersContext : DbContext
{
    public PrintersContext() : base("printerMonitoring")
    {
<<<<<<< HEAD
=======
        public PrintersContext() : base("printerMonitoring")
        {

        }
        public DbSet<PrintersModel> Printers { get; set; }
        public DbSet<UserModel> User { get; set; }
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
    }

    public DbSet<PrintersModel> Printers { get; set; }
    public DbSet<Users> Users { get; set; }
}
