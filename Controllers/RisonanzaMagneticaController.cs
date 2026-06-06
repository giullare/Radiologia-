using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class RisonanzaMagneticaController : Controller
    {
        private readonly ApplicationDbContext _db;

        public RisonanzaMagneticaController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Risonanza Magnetica";
            ViewData["BreadcrumbParent"] = "Modulo 2";

            var apparecchiature = await _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.RecordVerifiche)
                .Where(a => a.Modulo == TipoModulo.RM)
                .OrderBy(a => a.Descrizione)
                .ToListAsync();

            return View(apparecchiature);
        }
    }
}