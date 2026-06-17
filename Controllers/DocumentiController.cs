using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class DocumentiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DocumentiController> _logger;

        private static readonly string[] TipiConsentiti = {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx",
            ".jpg", ".jpeg", ".png", ".tiff", ".zip"
        };
        private const long MaxBytes = 50 * 1024 * 1024; // 50 MB

        public DocumentiController(
            ApplicationDbContext db,
            IWebHostEnvironment env,
            ILogger<DocumentiController> logger)
        {
            _db  = db;
            _env = env;
            _logger = logger;
        }

        // ─── UPLOAD — apparecchiatura ────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Upload(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"] = "Carica Documento";
            ViewBag.Apparecchiatura = app;
            ViewBag.Tipo = "apparecchiatura";
            ViewBag.EntitaId = id;
            ViewBag.ReturnUrl =
                $"/Apparecchiature/Detail/{id}?tab=documenti";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            int id,
            IFormFile file,
            string categoria,
            string? descrizione,
            string returnUrl = "/Apparecchiature")
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Nessun file selezionato.";
                return Redirect(returnUrl);
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!TipiConsentiti.Contains(ext))
            {
                TempData["Error"] =
                    $"Tipo file non consentito ({ext}). " +
                    "Usa PDF, Word, Excel o immagini.";
                return Redirect(returnUrl);
            }

            if (file.Length > MaxBytes)
            {
                TempData["Error"] =
                    "File troppo grande. Massimo 50 MB.";
                return Redirect(returnUrl);
            }

            // Calcola versione (se esiste già un file della stessa categoria)
            var versione = 1;
            var esistente = await _db.FileAllegati
                .Where(f => f.ApparecchiaturaId == id &&
                            f.Categoria == categoria)
                .OrderByDescending(f => f.Versione)
                .FirstOrDefaultAsync();
            if (esistente != null)
                versione = esistente.Versione + 1;

            // Salva su disco
            var uploadsPath = Path.Combine(
                _env.WebRootPath, "uploads", "apparecchiature",
                id.ToString());
            Directory.CreateDirectory(uploadsPath);

            var nomeStorage = $"{Guid.NewGuid()}{ext}";
            var percorsoCompleto = Path.Combine(uploadsPath, nomeStorage);

            using (var stream = new FileStream(percorsoCompleto,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Salva in DB
            var allegato = new FileAllegato
            {
                NomeOriginale     = file.FileName,
                NomeStorage       = Path.Combine(
                    "uploads", "apparecchiature",
                    id.ToString(), nomeStorage),
                MimeType          = file.ContentType,
                DimensioneBytes   = file.Length,
                Categoria         = string.IsNullOrEmpty(categoria)
                                    ? "Generico" : categoria,
                Descrizione       = descrizione,
                Versione          = versione,
                SostituisceFileId = esistente?.Id,
                ApparecchiaturaId = id,
                UploadedAt        = DateTime.UtcNow,
                UploadedByUserId  = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            _db.FileAllegati.Add(allegato);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "File {Nome} caricato per app {Id} da {User}.",
                file.FileName, id, User.Identity?.Name);

            TempData["Success"] =
                $"File «{file.FileName}» caricato con successo" +
                (versione > 1 ? $" (versione {versione})" : "") + ".";

            return Redirect(returnUrl);
        }

        // ─── UPLOAD — verifica ───────────────────────────────────────────
        [HttpGet]
        [Route("Documenti/Upload/verifica/{id}")]
        public async Task<IActionResult> UploadVerifica(int id)
        {
            var v = await _db.RecordVerifiche
                .Include(v => v.Apparecchiatura)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (v == null) return NotFound();

            ViewData["Title"] = "Carica Documento Verifica";
            ViewBag.Verifica        = v;
            ViewBag.Apparecchiatura = v.Apparecchiatura;
            ViewBag.Tipo            = "verifica";
            ViewBag.EntitaId        = id;
            ViewBag.ReturnUrl       = $"/Verifiche/Detail/{id}";
            return View("Upload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Documenti/Upload/verifica/{id}")]
        public async Task<IActionResult> UploadVerifica(
            int id,
            IFormFile file,
            string categoria,
            string? descrizione,
            string returnUrl = "/Verifiche")
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Nessun file selezionato.";
                return Redirect(returnUrl);
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!TipiConsentiti.Contains(ext))
            {
                TempData["Error"] =
                    $"Tipo file non consentito ({ext}).";
                return Redirect(returnUrl);
            }

            if (file.Length > MaxBytes)
            {
                TempData["Error"] = "File troppo grande. Massimo 50 MB.";
                return Redirect(returnUrl);
            }

            var uploadsPath = Path.Combine(
                _env.WebRootPath, "uploads", "verifiche", id.ToString());
            Directory.CreateDirectory(uploadsPath);

            var nomeStorage = $"{Guid.NewGuid()}{ext}";
            var percorsoCompleto = Path.Combine(uploadsPath, nomeStorage);

            using (var stream = new FileStream(
                percorsoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var allegato = new FileAllegato
            {
                NomeOriginale   = file.FileName,
                NomeStorage     = Path.Combine(
                    "uploads", "verifiche", id.ToString(), nomeStorage),
                MimeType        = file.ContentType,
                DimensioneBytes = file.Length,
                Categoria       = string.IsNullOrEmpty(categoria)
                                  ? "Report Verifica" : categoria,
                Descrizione     = descrizione,
                Versione        = 1,
                VerificaId      = id,
                UploadedAt      = DateTime.UtcNow,
                UploadedByUserId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            _db.FileAllegati.Add(allegato);
            await _db.SaveChangesAsync();

            TempData["Success"] =
                $"File «{file.FileName}» caricato con successo.";
            return Redirect(returnUrl);
        }
       // ─── VIEW ────────────────────────────────────────────────────
        public async Task<IActionResult> Visualizza(int id)
        {
    var file = await _db.FileAllegati.FindAsync(id);
    if (file == null) return NotFound();

    var percorso = Path.Combine(_env.WebRootPath, file.NomeStorage);
    if (!System.IO.File.Exists(percorso))
        return NotFound();

    var mimeType = file.MimeType ?? "application/octet-stream";

    // INLINE => apertura nel browser
    return PhysicalFile(percorso, mimeType);
       }

        // ─── DOWNLOAD ────────────────────────────────────────────────────
        public async Task<IActionResult> Download(int id)
        {
            var file = await _db.FileAllegati.FindAsync(id);
            if (file == null) return NotFound();

            var percorso = Path.Combine(_env.WebRootPath, file.NomeStorage);
            if (!System.IO.File.Exists(percorso))
            {
                TempData["Error"] =
                    "File non trovato sul server. " +
                    "Potrebbe essere stato eliminato.";
                return RedirectToAction("Index", "Home");
            }

            var mimeType = file.MimeType ?? "application/octet-stream";
            return PhysicalFile(percorso, mimeType,
                file.NomeOriginale);
        }

        // ─── ELIMINA ─────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Elimina(int id, string returnUrl)
        {
            
            var file = await _db.FileAllegati.FindAsync(id);
            if (file != null)
            {
                // Elimina da disco
                var percorso = Path.Combine(
                    _env.WebRootPath, file.NomeStorage);
                if (System.IO.File.Exists(percorso))
                    System.IO.File.Delete(percorso);

                _db.FileAllegati.Remove(file);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Documento eliminato.";
            }

            return Redirect(
                string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }
    }
}