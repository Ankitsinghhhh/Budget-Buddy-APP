using Budget_Buddy_App.Models;
using Budget_Buddy_App.Models;
using Budget_Buddy_App.Services;
using Budget_Buddy_App.Services;
using Budget_Buddy_app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Budget_Buddy_App.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExportPdfService _exportPdfService;

        public TransactionController(ApplicationDbContext context, IExportPdfService exportPdfService)
        {
            _context = context;
            _exportPdfService = exportPdfService;
        }

        // ✅ EXPORT TO PDF ACTION
        public async Task<IActionResult> ExportToPdf()
        {
            var transactions = await _context.Transactions
                                             .Include(t => t.Category)
                                             .ToListAsync();

            var pdfBytes = _exportPdfService.GeneratePdf(transactions);

            return File(pdfBytes, "application/pdf", "BudgetBuddy_Report.pdf");
        }



        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Transactions.Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            PopulateCategories();
            if (id == 0)
                return View(new Transaction());
            else
                return View(_context.Transactions.Find(id));
        }

        // POST: Transaction/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                    _context.Add(transaction);
                else
                    _context.Update(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateCategories();
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //// ✅ EXPORT TO PDF ACTION
        //public async Task<IActionResult> ExportToPdf()
        //{
        //    var transactions = await _context.Transactions.Include(t => t.Category).ToListAsync();
        //    var pdfBytes = _exportPdfService.GeneratePdf(transactions);
        //    return File(pdfBytes, "application/pdf", "Transactions.pdf");
        //}

        // 🛠 Helper Method
        [NonAction]
        public void PopulateCategories()
        {
            var CategoryCollection = _context.Categories.ToList();
            Category DefaultCategory = new Category() { CategoryId = 0, Title = "Choose a Category" };
            CategoryCollection.Insert(0, DefaultCategory);
            ViewBag.Categories = CategoryCollection;
        }
    }
}
