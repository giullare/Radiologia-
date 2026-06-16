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
        private readonly IWebHostEnvironment _env;

        public AdempimentiController(
            ApplicationDbContext db,
            ILogger<AdempimentiController> logger,
            IWebHostEnvironment env)
        {
            _db     = db;
            _logger = logger;
            _env    = env;
        }

        // ═══════════════════════════════════════════════════════════
        // NOTIFICHE DI PRATICA
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> NuovaNotifica(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"]              = "Nuova Notifica di Pratica";
            ViewData["BreadcrumbParent"]   = app.Descrizione;
            ViewData["BreadcrumbParentUrl"] = $"/Apparecchiature/Detail/{id}?tab=adempimenti";
            ViewBag.ApparecchiaturaId          = id;
            ViewBag.ApparecchiaturaDescrizione = app.Descrizione;
            ViewBag.IsEdit = false;

            return View("Create_NotificaPratica", new NotificaPratica
            {
                ApparecchiaturaId = id,
                DataNotifica      = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovaNotifica(
            NotificaPratica model,
            IFormFile? fileNotifica)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.ApparecchiaturaId          = model.ApparecchiaturaId;
                ViewBag.ApparecchiaturaDescrizione =
                    (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
                ViewBag.IsEdit = false;
                return View("Create_NotificaPratica", model);
            }

            // Upload file
            var pathFile = await SalvaFile(fileNotifica, "notifiche");
            if (pathFile != null)
            {
                var allegato = new FileAllegato
                {
                    NomeOriginale     = fileNotifica!.FileName,
                    NomeStorage       = pathFile,
                    MimeType          = fileNotifica.ContentType,
                    DimensioneBytes   = fileNotifica.Length,
                    Categoria         = "NOTIFICA_PRATICA",
                    ApparecchiaturaId = model.ApparecchiaturaId,
                    UploadedAt        = DateTime.UtcNow,
                    UploadedByUserId  = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                };
                model.FileAllegati.Add(allegato);
            }

            model.Id        = 0;
            model.CreatedAt = DateTime.UtcNow;
            _db.NotifichePratica.Add(model);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Notifica di Pratica creata per app {Id}.", model.ApparecchiaturaId);

            TempData["Success"] = "Notifica di Pratica registrata.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "adempimenti" });
        }

        // Edit Notifica
        [HttpGet]
        public async Task<IActionResult> EditNotifica(int id)
        {
            var n = await _db.NotifichePratica
                .Include(x => x.FileAllegati)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (n == null) return NotFound();

            var app = await _db.Apparecchiature.FindAsync(n.ApparecchiaturaId);
            ViewData["Title"]              = "Modifica Notifica di Pratica";
            ViewBag.ApparecchiaturaId          = n.ApparecchiaturaId;
            ViewBag.ApparecchiaturaDescrizione = app?.Descrizione;
            ViewBag.IsEdit    = true;
            ViewBag.FileAttuale = n.FileAllegati
                .FirstOrDefault(f => f.Categoria == "NOTIFICA_PRATICA")?.NomeStorage;

            return View("Create_NotificaPratica", n);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EditNotifica(
            int id, NotificaPratica model, IFormFile? fileNotifica)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.ApparecchiaturaId          = model.ApparecchiaturaId;
                ViewBag.ApparecchiaturaDescrizione =
                    (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
                ViewBag.IsEdit = true;
                return View("Create_NotificaPratica", model);
            }

            var existing = await _db.NotifichePratica.FindAsync(id);
            if (existing == null) return NotFound();

            existing.NumeroProtocolloPec = model.NumeroProtocolloPec;
            existing.DataNotifica        = model.DataNotifica;
            existing.EnteDestinatario    = model.EnteDestinatario;
            existing.InviatoRspp         = model.InviatoRspp;
            existing.DataInvioRspp       = model.DataInvioRspp;
            existing.Note                = model.Note;

            if (fileNotifica != null)
            {
                var pathFile = await SalvaFile(fileNotifica, "notifiche");
                if (pathFile != null)
                {
                    _db.FileAllegati.Add(new FileAllegato
                    {
                        NomeOriginale     = fileNotifica.FileName,
                        NomeStorage       = pathFile,
                        MimeType          = fileNotifica.ContentType,
                        DimensioneBytes   = fileNotifica.Length,
                        Categoria         = "NOTIFICA_PRATICA",
                        ApparecchiaturaId = existing.ApparecchiaturaId,
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Notifica di Pratica aggiornata.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = existing.ApparecchiaturaId, tab = "adempimenti" });
        }

        // ═══════════════════════════════════════════════════════════
        // NULLA OSTA
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> NuovoNullaOsta(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"]              = "Nuovo Nulla Osta";
            ViewData["BreadcrumbParent"]   = app.Descrizione;
            ViewData["BreadcrumbParentUrl"] = $"/Apparecchiature/Detail/{id}?tab=adempimenti";
            ViewBag.ApparecchiaturaId          = id;
            ViewBag.ApparecchiaturaDescrizione = app.Descrizione;
            ViewBag.IsEdit = false;

            return View("Create_NullaOsta", new NullaOsta
            {
                ApparecchiaturaId = id,
                DataRilascio      = DateTime.Today,
                Stato             = StatoNullaOsta.Valido
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovoNullaOsta(
            NullaOsta model, IFormFile? fileNullaOsta)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.ApparecchiaturaId          = model.ApparecchiaturaId;
                ViewBag.ApparecchiaturaDescrizione =
                    (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
                ViewBag.IsEdit = false;
                return View("Create_NullaOsta", model);
            }

            // Upload file
            if (fileNullaOsta != null)
            {
                var pathFile = await SalvaFile(fileNullaOsta, "nullaosta");
                if (pathFile != null)
                {
                    model.FileAllegati.Add(new FileAllegato
                    {
                        NomeOriginale     = fileNullaOsta.FileName,
                        NomeStorage       = pathFile,
                        MimeType          = fileNullaOsta.ContentType,
                        DimensioneBytes   = fileNullaOsta.Length,
                        Categoria         = "NULLA_OSTA",
                        ApparecchiaturaId = model.ApparecchiaturaId,
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                }
            }

            // Stato: solo Valido o InRinnovo (non calcolare automatico scaduto)
            // Il calcolo automatico viene fatto nella vista Compliance
            model.Id        = 0;
            model.CreatedAt = DateTime.UtcNow;
            _db.NullaOsta.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Nulla Osta registrato.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "adempimenti" });
        }

        // Edit Nulla Osta
        [HttpGet]
        public async Task<IActionResult> EditNullaOsta(int id)
        {
            var n = await _db.NullaOsta
                .Include(x => x.FileAllegati)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (n == null) return NotFound();

            var app = await _db.Apparecchiature.FindAsync(n.ApparecchiaturaId);
            ViewData["Title"]              = "Modifica Nulla Osta";
            ViewBag.ApparecchiaturaId          = n.ApparecchiaturaId;
            ViewBag.ApparecchiaturaDescrizione = app?.Descrizione;
            ViewBag.IsEdit    = true;
            ViewBag.FileAttuale = n.FileAllegati
                .FirstOrDefault(f => f.Categoria == "NULLA_OSTA")?.NomeStorage;

            return View("Create_NullaOsta", n);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EditNullaOsta(
            int id, NullaOsta model, IFormFile? fileNullaOsta)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.ApparecchiaturaId          = model.ApparecchiaturaId;
                ViewBag.ApparecchiaturaDescrizione =
                    (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
                ViewBag.IsEdit = true;
                return View("Create_NullaOsta", model);
            }

            var existing = await _db.NullaOsta.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Tipo          = model.Tipo;
            existing.Numero        = model.Numero;
            existing.DataRilascio  = model.DataRilascio;
            existing.EnteRilascio  = model.EnteRilascio;
            existing.DataScadenza  = model.DataScadenza;
            existing.Stato         = model.Stato;
            existing.Note          = model.Note;

            if (fileNullaOsta != null)
            {
                var pathFile = await SalvaFile(fileNullaOsta, "nullaosta");
                if (pathFile != null)
                {
                    _db.FileAllegati.Add(new FileAllegato
                    {
                        NomeOriginale     = fileNullaOsta.FileName,
                        NomeStorage       = pathFile,
                        MimeType          = fileNullaOsta.ContentType,
                        DimensioneBytes   = fileNullaOsta.Length,
                        Categoria         = "NULLA_OSTA",
                        ApparecchiaturaId = existing.ApparecchiaturaId,
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Nulla Osta aggiornato.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = existing.ApparecchiaturaId, tab = "adempimenti" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EliminaNullaOsta(int id, int apparecchiaturaId)
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
        // SOPRALLUOGHI  (ex Verbali)
        // ═══════════════════════════════════════════════════════════

        [HttpGet]
        public async Task<IActionResult> NuovoVerbale(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"]              = "Nuovo Sopralluogo";
            ViewData["BreadcrumbParent"]   = app.Descrizione;
            ViewData["BreadcrumbParentUrl"] = $"/Apparecchiature/Detail/{id}?tab=verbali";
            ViewBag.ApparecchiaturaId          = id;
            ViewBag.ApparecchiaturaDescrizione = app.Descrizione;
            ViewBag.IsEdit = false;

            return View("Create_Verbale", new Verbale
            {
                ApparecchiaturaId = id,
                DataSopralluogo   = DateTime.Today,
                Stato             = "Aperto"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovoVerbale(
            Verbale model, IFormFile? fileVerbale)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.ApparecchiaturaId          = model.ApparecchiaturaId;
                ViewBag.ApparecchiaturaDescrizione =
                    (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
                ViewBag.IsEdit = false;
                return View("Create_Verbale", model);
            }

            // Upload file verbale
            if (fileVerbale != null)
            {
                var pathFile = await SalvaFile(fileVerbale, "verbali");
                if (pathFile != null)
                {
                    model.FileAllegati.Add(new FileAllegato
                    {
                        NomeOriginale     = fileVerbale.FileName,
                        NomeStorage       = pathFile,
                        MimeType          = fileVerbale.ContentType,
                        DimensioneBytes   = fileVerbale.Length,
                        Categoria         = "VERBALE_SOPRALLUOGO",
                        ApparecchiaturaId = model.ApparecchiaturaId,
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                }
            }

            model.Id              = 0;
            model.CreatedAt       = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _db.Verbali.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Sopralluogo registrato.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "adempimenti" });
        }

        // Edit Verbale
        [HttpGet]
        public async Task<IActionResult> EditVerbale(int id)
        {
            var v = await _db.Verbali
                .Include(x => x.FileAllegati)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return NotFound();

            var app = await _db.Apparecchiature.FindAsync(v.ApparecchiaturaId);
            ViewData["Title"]              = "Modifica Sopralluogo";
            ViewBag.ApparecchiaturaId          = v.ApparecchiaturaId;
            ViewBag.ApparecchiaturaDescrizione = app?.Descrizione;
            ViewBag.IsEdit = true;

            return View("Create_Verbale", v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EditVerbale(
            int id, Verbale model, IFormFile? fileVerbale)
        {
            ModelState.Remove("Apparecchiatura");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.ApparecchiaturaId          = model.ApparecchiaturaId;
                ViewBag.ApparecchiaturaDescrizione =
                    (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
                ViewBag.IsEdit = true;
                return View("Create_Verbale", model);
            }

            var existing = await _db.Verbali.FindAsync(id);
            if (existing == null) return NotFound();

            // Campi base
            existing.DataSopralluogo  = model.DataSopralluogo;
            existing.Partecipanti     = model.Partecipanti;
            existing.Oggetto          = model.Oggetto;
            existing.Rilievi          = model.Rilievi;
            existing.NonConformita    = model.NonConformita;
            existing.AzioniCorrettive = model.AzioniCorrettive;
            existing.ScadenzaAzioni   = model.ScadenzaAzioni;
            existing.Stato            = model.Stato;
            existing.DataChiusura     = model.DataChiusura;

            // Specifiche sopralluogo — Generali
            existing.PresenzaDosimetroAmbientale    = model.PresenzaDosimetroAmbientale;
            existing.CorrettezzaDosimetroAmbientale = model.CorrettezzaDosimetroAmbientale;
            existing.PresenzaNormeRadioprotezione   = model.PresenzaNormeRadioprotezione;
            existing.Piantina                       = model.Piantina;
            // Consolle
            existing.SegnaleticaConsolle                       = model.SegnaleticaConsolle;
            existing.ClassificazioneConsolle                   = model.ClassificazioneConsolle;
            existing.FunzionamentoSegnaleticaLuminosaConsolle  = model.FunzionamentoSegnaleticaLuminosaConsolle;
            existing.SegnaleticaGravidanzaConsolle             = model.SegnaleticaGravidanzaConsolle;
            existing.InterLockConsolle                         = model.InterLockConsolle;
            // Diagnostica
            existing.SegnaleticaSalaDiagnostica                  = model.SegnaleticaSalaDiagnostica;
            existing.ClassificazioneSalaDiagnostica              = model.ClassificazioneSalaDiagnostica;
            existing.FunzionamentoSegnaleticaLuminosaDiagnostica = model.FunzionamentoSegnaleticaLuminosaDiagnostica;
            existing.SegnaleticaGravidanzaDiagnostica            = model.SegnaleticaGravidanzaDiagnostica;
            // Preparazione
            existing.SegnaleticaSalaPreparazione                 = model.SegnaleticaSalaPreparazione;
            existing.ClassificazioneSalaPreparazione             = model.ClassificazioneSalaPreparazione;
            existing.FunzionamentoSegnaleticaLuminosaPreparazione = model.FunzionamentoSegnaleticaLuminosaPreparazione;
            existing.SegnaleticaGravidanzaPreparazione           = model.SegnaleticaGravidanzaPreparazione;
            existing.InterLockPreparazione                       = model.InterLockPreparazione;
            // Portatili
            existing.SegnaleticaRischioRadiazioni = model.SegnaleticaRischioRadiazioni;
            existing.SegnaleticaRischioGravidanza = model.SegnaleticaRischioGravidanza;
            existing.PresenzaNormePortatili       = model.PresenzaNormePortatili;

            if (fileVerbale != null)
            {
                var pathFile = await SalvaFile(fileVerbale, "verbali");
                if (pathFile != null)
                {
                    _db.FileAllegati.Add(new FileAllegato
                    {
                        NomeOriginale     = fileVerbale.FileName,
                        NomeStorage       = pathFile,
                        MimeType          = fileVerbale.ContentType,
                        DimensioneBytes   = fileVerbale.Length,
                        Categoria         = "VERBALE_SOPRALLUOGO",
                        ApparecchiaturaId = existing.ApparecchiaturaId,
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                }
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "Sopralluogo aggiornato.";
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = existing.ApparecchiaturaId, tab = "adempimenti" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> ChiudiVerbale(int id, int apparecchiaturaId)
        {
            var v = await _db.Verbali.FindAsync(id);
            if (v != null)
            {
                v.Stato        = "Chiuso";
                v.DataChiusura = DateTime.Today;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Sopralluogo chiuso.";
            }
            return RedirectToAction("Detail", "Apparecchiature",
                new { id = apparecchiaturaId, tab = "adempimenti" });
        }

        // ═══════════════════════════════════════════════════════════
        // LISTE GENERALI
        // ═══════════════════════════════════════════════════════════

        public async Task<IActionResult> NotifichePratica()
        {
            ViewData["Title"]            = "Notifiche di Pratica";
            ViewData["BreadcrumbParent"] = "Sorveglianza Fisica";

            var items = await _db.NotifichePratica
                .Include(n => n.Apparecchiatura)
                .OrderByDescending(n => n.DataNotifica)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> NullaOsta()
        {
            ViewData["Title"]            = "Nulla Osta";
            ViewData["BreadcrumbParent"] = "Sorveglianza Fisica";

            var items = await _db.NullaOsta
                .Include(n => n.Apparecchiatura)
                .OrderByDescending(n => n.DataRilascio)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Verbali()
        {
            ViewData["Title"]            = "Sopralluoghi";
            ViewData["BreadcrumbParent"] = "Sorveglianza Fisica";

            var items = await _db.Verbali
                .Include(v => v.Apparecchiatura)
                .OrderByDescending(v => v.DataSopralluogo)
                .ToListAsync();

            return View(items);
        }

        // ─── HELPER upload file ───────────────────────────────────────────
        private async Task<string?> SalvaFile(IFormFile? file, string subfolder)
        {
            if (file == null || file.Length == 0) return null;
            var dir  = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(dir);
            var nome = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var path = Path.Combine(dir, nome);
            await using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/{subfolder}/{nome}";
        }
    }
}