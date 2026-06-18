using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetApp.Web.Data;
using System.Globalization;

namespace BudgetApp.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);
            var totalCategories = await _context.Categories.CountAsync();
            var recentExpenses = await _context.Expenses
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .Take(5)
                .ToListAsync();

            // Formatujemy do stringa z kropką jako separatorem, by JavaScript na froncie zawsze poprawnie to odczytał
            ViewBag.TotalExpenses = totalExpenses.ToString("F2", CultureInfo.InvariantCulture);

            // Placeholder dla przychodów, zapobiegnie błędowi "undefined" w kodzie JS wykresu
            ViewBag.TotalIncomes = "0.00";

            ViewBag.TotalCategories = totalCategories;
            ViewBag.RecentExpenses = recentExpenses;

            return View();
        }
    }
}