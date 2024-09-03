using Controle_De_Impressoras.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Controle_De_Impressoras.Data
{
    public class PrintersContext : DbContext
    {
        public PrintersContext() : base("printerMonitoring")
        {

        }
        public DbSet<PrintersModel> Printers { get; set; }
        public DbSet<UserModel> User { get; set; }
    }
}