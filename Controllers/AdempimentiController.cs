using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class AdempimentiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AdempimentiController> _logger;

        public AdempimentiController(
            ApplicationDbContext db,
            ILogger<AdempimentiController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ═══════════════════════════════════════════════════════════
        // NOTIFICHE DI PRATICA
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> NuovaNotifica(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"] = "Nuova Notifica di Pratica";
            ViewData["BreadcrumbParent"] = app.Descrizione;
            ViewData["BreadcrumbParentUrl"] =
                $"/Apparecchiature/Detail/{id}?tab=adempimenti";

            ViewBag.Apparecchiatura = app;
            return View(new NotificaPratica
            {
                ApparecchiaturaId = id,
                DataNotifica      = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovaNotifica(NotificaPratica model)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                var app2 = await _db.Apparecchiature
                    .FindAsync(model.ApparecchiaturaId);
                ViewBag.Apparecchiatura = app2;
                ViewData["Title"] = "Nuova Notifica di Pratica";
                return View(model);
            }

            model.Id = 0;
            model.CreatedAt = DateTime.UtcNow;
            _db.NotifichePratica.Add(model);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Notifica di Pratica creata per app {Id}.",
                model.ApparecchiaturaId);

            TempData["Success"] = "Notifica di Pratica registrata.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "adempimenti" });
        }

        // ═══════════════════════════════════════════════════════════
        // NULLA OSTA
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> NuovoNullaOsta(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"] = "Nuovo Nulla Osta";
            ViewData["BreadcrumbParent"] = app.Descrizione;
            ViewData["BreadcrumbParentUrl"] =
                $"/Apparecchiature/Detail/{id}?tab=adempimenti";

            ViewBag.Apparecchiatura = app;
            return View(new NullaOsta
            {
                ApparecchiaturaId = id,
                DataRilascio      = DateTime.Today,
                Stato             = StatoNullaOsta.Valido
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovoNullaOsta(NullaOsta model)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                var app2 = await _db.Apparecchiature
                    .FindAsync(model.ApparecchiaturaId);
                ViewBag.Apparecchiatura = app2;
                ViewData["Title"] = "Nuovo Nulla Osta";
                return View(model);
            }

            // Aggiorna automaticamente stato se ha data scadenza
            if (model.DataScadenza.HasValue &&
                model.DataScadenza < DateTime.Today)
                model.Stato = StatoNullaOsta.Scaduto;
            else if (model.DataScadenza.HasValue &&
                     model.DataScadenza <= DateTime.Today.AddDays(30))
                model.Stato = StatoNullaOsta.InScadenza;

            model.Id = 0;
            model.CreatedAt = DateTime.UtcNow;
            _db.NullaOsta.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Nulla Osta registrato.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "adempimenti" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EliminaNullaOsta(
            int id, int apparecchiaturaId)
        {
            var no = await _db.NullaOsta.FindAsync(id);
            if (no != null)
            {
                _db.NullaOsta.Remove(no);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Nulla Osta eliminato.";
            }
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = apparecchiaturaId, tab = "adempimenti" });
        }

        // ═══════════════════════════════════════════════════════════
        // VERBALI DI SOPRALLUOGO
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> NuovoVerbale(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"] = "Nuovo Verbale di Sopralluogo";
            ViewData["BreadcrumbParent"] = app.Descrizione;
            ViewData["BreadcrumbParentUrl"] =
                $"/Apparecchiature/Detail/{id}?tab=adempimenti";

            ViewBag.Apparecchiatura = app;
            return View(new Verbale
            {
                ApparecchiaturaId = id,
                DataSopralluogo   = DateTime.Today,
                Stato             = "Aperto"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovoVerbale(Verbale model)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                var app2 = await _db.Apparecchiature
                    .FindAsync(model.ApparecchiaturaId);
                ViewBag.Apparecchiatura = app2;
                ViewData["Title"] = "Nuovo Verbale di Sopralluogo";
                return View(model);
            }

            model.Id = 0;
            model.CreatedAt = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _db.Verbali.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Verbale di sopralluogo registrato.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "adempimenti" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> ChiudiVerbale(
            int id, int apparecchiaturaId)
        {
            var v = await _db.Verbali.FindAsync(id);
            if (v != null)
            {
                v.Stato        = "Chiuso";
                v.DataChiusura = DateTime.Today;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Verbale chiuso.";
            }
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = apparecchiaturaId, tab = "adempimenti" });
        }

        // ═══════════════════════════════════════════════════════════
        // LISTA GENERALE ADEMPIMENTI
        // ═══════════════════════════════════════════════════════════

        public async Task<IActionResult> NotifichePratica()
        {
            ViewData["Title"] = "Notifiche di Pratica";
            ViewData["BreadcrumbParent"] = "Adempimenti EDR";

            var items = await _db.NotifichePratica
                .Include(n => n.Apparecchiatura)
                .OrderByDescending(n => n.DataNotifica)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> NullaOsta()
        {
            ViewData["Title"] = "Nulla Osta";
            ViewData["BreadcrumbParent"] = "Adempimenti EDR";

            var items = await _db.NullaOsta
                .Include(n => n.Apparecchiatura)
                .OrderByDescending(n => n.DataRilascio)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Verbali()
        {
            ViewData["Title"] = "Verbali di Sopralluogo";
            ViewData["BreadcrumbParent"] = "Adempimenti EDR";

            var items = await _db.Verbali
                .Include(v => v.Apparecchiatura)
                .OrderByDescending(v => v.DataSopralluogo)
                .ToListAsync();

            return View(items);
        }
    }
}