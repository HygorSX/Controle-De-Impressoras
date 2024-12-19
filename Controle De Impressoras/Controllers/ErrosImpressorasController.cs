using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System.Linq;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class ErrosImpressorasController : Controller
    {
        private PrintersContext db = new PrintersContext();

        // GET: ErrosImpressoras
        public ActionResult Index()
        {
            // Recupera todos os registros da tabela ErrosImpressoras
            var errosImpressoras = db.ErrosImpressoras.ToList();

            // Passa os dados para a View
            return View(errosImpressoras);
        }
    }
}
