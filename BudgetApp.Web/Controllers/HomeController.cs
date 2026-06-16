using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BudgetApp.Web.Data;
using BudgetApp.Web.Models;

namespace BudgetApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    // Dodajemy pole dla bazy danych
    private readonly ApplicationDbContext _context;

    // Wstrzykujemy bazê danych w konstruktorze
    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // Pobieranie statystyk z bazy danych dla strony g³ównej
        var totalExpenses = _context.Expenses.Sum(e => e.Amount);
        var totalCategories = _context.Categories.Count();

        // Przekazujemy dane do widoku przez ViewBag
        ViewBag.TotalExpenses = totalExpenses;
        ViewBag.TotalCategories = totalCategories;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}