using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Controle_De_Impressoras.Utils
{
    public class PrinterService
    {
        private readonly PrintersContext _context;

        public PrinterService(PrintersContext context)
        {
            _context = context;
        }

        public bool AddPrinter(PrintersModel model)
        {
            bool printerExists = _context.Printers.Any(p => p.SerialImpressora == model.SerialImpressora);

            if (printerExists)
            {
                return false;
            }

            _context.Printers.Add(model);
            _context.SaveChanges();
            return true;
        }
    }
}