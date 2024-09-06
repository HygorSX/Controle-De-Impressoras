using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int? printerId)
        {
            var reports = ReportModel.RecuperarRelatorios(startDate, endDate, printerId);
            return View(reports);
        }

        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                if (report == null)
                {
                    return HttpNotFound();
                }
                return View(report);
            }
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportModel report)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintersContext())
                {
                    context.DailyReports.Add(report);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(report);
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                if (report == null)
                {
                    return HttpNotFound();
                }
                return View(report);
            }
        }

        // POST: Reports/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReportModel report)
        {
            if (ModelState.IsValid)
            {
                using (var context = new PrintersContext())
                {
                    context.Entry(report).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(report);
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                if (report == null)
                {
                    return HttpNotFound();
                }
                return View(report);
            }
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new PrintersContext())
            {
                var report = context.DailyReports.Find(id);
                context.DailyReports.Remove(report);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}
