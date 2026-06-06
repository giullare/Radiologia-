using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class MedicinaNucleareController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<MedicinaNucleareController> _logger;

        public MedicinaNucleareController(
            ApplicationDbContext db,
            ILogger<MedicinaNucleareController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Registri Medicina Nucleare";
            ViewData["BreadcrumbParent"] = "Modulo 3";

            var apparecchiature = await _db.Apparecchiature
                .Include(a => a.Reparto)
                .Where(a => a.AmbitoIntervento ==
                            AmbitoIntervento.MedicinaNucleare &&
                            a.Stato != StatoApparecchiatura.Cessata)
                .OrderBy(a => a.Descrizione)
                .ToListAsync();

            var vm = new MedicinaNucleareIndexVm
            {
                Apparecchiature = apparecchiature,
                TotaleApparecchiature = apparecchiature.Count
            };

            return View(vm);
        }
    }

    public class MedicinaNucleareIndexVm
    {
        public List<Apparecchiatura> Apparecchiature { get; set; } = new();
        public int TotaleApparecchiature { get; set; }
    }
}