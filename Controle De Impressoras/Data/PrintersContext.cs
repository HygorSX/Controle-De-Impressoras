using Controle_De_Impressoras.Models;
using System.Data.Entity;

public class PrintersContext : DbContext
{
    public PrintersContext() : base("printerMonitoring")
    {
    }

    public DbSet<PrintersModel> Printers { get; set; }
    public DbSet<Users> Users { get; set; }
}
