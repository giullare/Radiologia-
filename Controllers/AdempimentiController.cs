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
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaNotifica(int id, int apparecchiaturaId)
{
    var item = await _db.NotifichePratica
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (item != null)
    {
        // 1. elimina i file fisici dal disco
        foreach (var f in item.FileAllegati)
        {
            EliminaFileFisico(f.NomeStorage);
        }

        // 2. elimina i record degli allegati
        if (item.FileAllegati.Any())
        {
            _db.FileAllegati.RemoveRange(item.FileAllegati);
        }

        // 3. elimina notifica
        _db.NotifichePratica.Remove(item);

        await _db.SaveChangesAsync();
    }


    return RedirectToAction("Detail", "Apparecchiature",
        new { id = apparecchiaturaId, tab = "adempimenti" });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaFileNotifica(int id, int apparecchiaturaId)
{
    // id = Id della NotificaPratica. Elimina solo l'allegato NOTIFICA_PRATICA,
    // lascia intatto l'eventuale DVR.
    var file = await _db.FileAllegati
        .FirstOrDefaultAsync(f => f.NotificaPraticaId == id
                                && f.Categoria == "NOTIFICA_PRATICA");

    if (file != null)
    {
        EliminaFileFisico(file.NomeStorage);
        _db.FileAllegati.Remove(file);
        await _db.SaveChangesAsync();
        TempData["Success"] = "File eliminato.";
    }

    return RedirectToAction("EditNotifica", new { id = id });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaFileDvr(int id, int apparecchiaturaId)
{
    // id = Id della NotificaPratica. Elimina solo l'allegato DVR,
    // lascia intatta la notifica di pratica.
    var file = await _db.FileAllegati
        .FirstOrDefaultAsync(f => f.NotificaPraticaId == id
                                && f.Categoria == "DVR");

    if (file != null)
    {
        EliminaFileFisico(file.NomeStorage);
        _db.FileAllegati.Remove(file);
        await _db.SaveChangesAsync();
        TempData["Success"] = "File eliminato.";
    }

    return RedirectToAction("EditNotifica", new { id = id });
}

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
                DataNotifica      = DateTime.Today,
                FileAllegati = new List<FileAllegato>()
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> NuovaNotifica(
            NotificaPratica model,
            IFormFile? fileNotifica,
            IFormFile? fileDvr)
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
            
// ✅ 1. SALVA PRIMA LA NOTIFICA
    model.Id = 0;
    model.CreatedAt = DateTime.UtcNow;

    _db.NotifichePratica.Add(model);
    await _db.SaveChangesAsync(); // ✅ ORA model.Id è valido

    // ✅ 2. SALVA FILE NOTIFICA
    if (fileNotifica != null)
    {


            // Upload file
            var pathFile = await SalvaFile(fileNotifica, "notifiche");
            if (pathFile != null)
            {
                _db.FileAllegati.Add(new FileAllegato
                {
                    NomeOriginale     = fileNotifica!.FileName,
                    NomeStorage       = pathFile,
                    MimeType          = fileNotifica.ContentType,
                    DimensioneBytes   = fileNotifica.Length,
                    Categoria         = "NOTIFICA_PRATICA",
                    ApparecchiaturaId = model.ApparecchiaturaId,
                    NotificaPraticaId = model.Id, // ✅ CORRETTO
                    UploadedAt        = DateTime.UtcNow,
                    UploadedByUserId  = User.FindFirst(
                        System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                });
               
            }
             }

 // ✅ 3. SALVA FILE DVR
    if (fileDvr != null)
    {

            var pathDvr = await SalvaFile(fileDvr, "notifiche");
if (pathDvr != null)
{
     _db.FileAllegati.Add(new FileAllegato
    {
        NomeOriginale     = fileDvr!.FileName,
        NomeStorage       = pathDvr,
        MimeType          = fileDvr.ContentType,
        DimensioneBytes   = fileDvr.Length,
        Categoria         = "DVR",
        ApparecchiaturaId = model.ApparecchiaturaId,
        NotificaPraticaId = model.Id, // ✅ CORRETTO
        UploadedAt        = DateTime.UtcNow,
        UploadedByUserId  = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
    });

   
}
}

           
// ✅ 4. SALVA FILE NEL DB
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
            ViewData["Title"]              = "Modifica Notifica di Pratica e DVR";
            ViewBag.ApparecchiaturaId          = n.ApparecchiaturaId;
            ViewBag.ApparecchiaturaDescrizione = app?.Descrizione;
            ViewBag.IsEdit    = true;
           
// ✅ FILE NOTIFICA
    var fileNotifica = n.FileAllegati
        .FirstOrDefault(f => f.Categoria == "NOTIFICA_PRATICA");

    ViewBag.FileAttuale = fileNotifica?.NomeStorage;
    ViewBag.FileNome = fileNotifica?.NomeOriginale;

    // ✅ FILE DVR
    var fileDvr = n.FileAllegati
        .FirstOrDefault(f => f.Categoria == "DVR");

    ViewBag.FileDvrAttuale = fileDvr?.NomeStorage;
    ViewBag.FileDvrNome = fileDvr?.NomeOriginale;


            return View("Create_NotificaPratica", n);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EditNotifica(
            int id, NotificaPratica model, IFormFile? fileNotifica, IFormFile? fileDvr)
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
                // rimuove il vecchio allegato (db + disco) prima di salvare il nuovo
                var vecchio = await _db.FileAllegati
                    .FirstOrDefaultAsync(f => f.NotificaPraticaId == existing.Id
                                            && f.Categoria == "NOTIFICA_PRATICA");
                if (vecchio != null)
                {
                    EliminaFileFisico(vecchio.NomeStorage);
                    _db.FileAllegati.Remove(vecchio);
                }

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
                        NotificaPraticaId = existing.Id,
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                }
            }
if (fileDvr != null)
{
    // rimuove il vecchio DVR (db + disco) prima di salvare il nuovo
    var vecchioDvr = await _db.FileAllegati
        .FirstOrDefaultAsync(f => f.NotificaPraticaId == existing.Id
                                && f.Categoria == "DVR");
    if (vecchioDvr != null)
    {
        EliminaFileFisico(vecchioDvr.NomeStorage);
        _db.FileAllegati.Remove(vecchioDvr);
    }

    var pathDvr = await SalvaFile(fileDvr, "notifiche");
    if (pathDvr != null)
    {
        _db.FileAllegati.Add(new FileAllegato
        {
            NomeOriginale     = fileDvr.FileName,
            NomeStorage       = pathDvr,
            MimeType          = fileDvr.ContentType,
            DimensioneBytes   = fileDvr.Length,
            Categoria         = "DVR",
            ApparecchiaturaId = existing.ApparecchiaturaId,
            NotificaPraticaId = existing.Id,
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
        // CESSAZIONI DI PRATICA
        // ═══════════════════════════════════════════════════════════
       
[HttpGet]
public async Task<IActionResult> EditCessazione(int id)
{
    var c = await _db.CessazioniPratica
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (c == null) return NotFound();

    var app = await _db.Apparecchiature.FindAsync(c.ApparecchiaturaId);

    ViewData["Title"] = "Modifica Cessazione di Pratica";
    ViewBag.ApparecchiaturaId = c.ApparecchiaturaId;
    ViewBag.ApparecchiaturaDescrizione = app?.Descrizione;
    ViewBag.IsEdit = true;

    var file = c.FileAllegati
        .FirstOrDefault(f => f.Categoria == "CESSAZIONE");
        ViewBag.FileAttuale = file?.NomeStorage;
ViewBag.FileNome = file?.NomeOriginale; // ✅ AGGIUNTO


    return View("Create_CessazionePratica", c);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditCessazione(
    int id,
    CessazionePratica model,
    IFormFile? fileCessazione)
{
    ModelState.Remove("Apparecchiatura");
    ModelState.Remove("FileAllegati");

    if (!ModelState.IsValid)
    {
        ViewBag.ApparecchiaturaId = model.ApparecchiaturaId;
        ViewBag.ApparecchiaturaDescrizione =
            (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
        ViewBag.IsEdit = true;

        return View("Create_CessazionePratica", model);
    }

    var existing = await _db.CessazioniPratica.FindAsync(id);
    if (existing == null) return NotFound();

    existing.NumeroProtocolloPec = model.NumeroProtocolloPec;
    existing.DataCessazione = model.DataCessazione;
    existing.EnteDestinatario = model.EnteDestinatario;
    existing.Note = model.Note;

    if (fileCessazione != null)
    {
        // rimuove il vecchio allegato (db + disco) prima di salvare il nuovo,
        // cosi non si accumulano file orfani e l'edit mostra sempre l'ultimo
        var vecchio = await _db.FileAllegati
            .FirstOrDefaultAsync(f => f.CessazionePraticaId == existing.Id
                                    && f.Categoria == "CESSAZIONE");
        if (vecchio != null)
        {
            EliminaFileFisico(vecchio.NomeStorage);
            _db.FileAllegati.Remove(vecchio);
        }

        var path = await SalvaFile(fileCessazione, "cessazioni");

        if (path != null)
        {
            _db.FileAllegati.Add(new FileAllegato
            {
                NomeOriginale     = fileCessazione.FileName,
                NomeStorage       = path,
                MimeType          = fileCessazione.ContentType,
                DimensioneBytes   = fileCessazione.Length,
                Categoria         = "CESSAZIONE",
                ApparecchiaturaId = existing.ApparecchiaturaId,
                CessazionePraticaId = existing.Id,
                UploadedAt        = DateTime.UtcNow,
                UploadedByUserId  = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
            });
        }
    }

    await _db.SaveChangesAsync();

    TempData["Success"] = "Cessazione aggiornata.";

    return RedirectToAction("Detail", "Apparecchiature",
        new { id = existing.ApparecchiaturaId, tab = "adempimenti" });
}

       [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaCessazione(int id, int apparecchiaturaId)
{
    var item = await _db.CessazioniPratica
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (item != null)
    {
        // 1. elimina i file fisici dal disco
        foreach (var f in item.FileAllegati)
        {
            EliminaFileFisico(f.NomeStorage);
        }

        // 2. elimina i record degli allegati
        if (item.FileAllegati.Any())
        {
            _db.FileAllegati.RemoveRange(item.FileAllegati);
        }

        // 3. elimina cessazione
        _db.CessazioniPratica.Remove(item);

        await _db.SaveChangesAsync();
    }


    return RedirectToAction("Detail", "Apparecchiature",
        new { id = apparecchiaturaId, tab = "adempimenti" });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaFileCessazione(int id, int apparecchiaturaId)
{
    // id = Id della CessazionePratica, non del FileAllegato:
    // così il bottone "X" nel form puo' richiamare l'azione senza dover
    // conoscere l'Id del FileAllegato, che la view non riceve.
    var file = await _db.FileAllegati
        .FirstOrDefaultAsync(f => f.CessazionePraticaId == id
                                && f.Categoria == "CESSAZIONE");

    if (file != null)
    {
        EliminaFileFisico(file.NomeStorage);
        _db.FileAllegati.Remove(file);
        await _db.SaveChangesAsync();
        TempData["Success"] = "File eliminato.";
    }

    return RedirectToAction("EditCessazione", new { id = id });
}

        [HttpGet]
public async Task<IActionResult> NuovaCessazione(int id)
{
    var app = await _db.Apparecchiature.FindAsync(id);
    if (app == null) return NotFound();

    ViewBag.ApparecchiaturaId = id;
    ViewBag.ApparecchiaturaDescrizione = app.Descrizione;
    ViewBag.IsEdit = false;

    return View("Create_CessazionePratica", new CessazionePratica
    {
        ApparecchiaturaId = id,
        DataCessazione = DateTime.Today,
        FileAllegati = new List<FileAllegato>()
    });
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> NuovaCessazione(
    CessazionePratica model,
    IFormFile? fileCessazione)
{
    ModelState.Remove("Apparecchiatura");
    ModelState.Remove("FileAllegati");

    if (!ModelState.IsValid)
    {
        return View("Create_CessazionePratica", model);
    }
_db.CessazioniPratica.Add(model);
    await _db.SaveChangesAsync();
     if (fileCessazione != null)
    {

    var pathFile = await SalvaFile(fileCessazione, "cessazioni");

    if (pathFile != null)
    {
        _db.FileAllegati.Add(new FileAllegato
        {
            NomeOriginale = fileCessazione!.FileName,
            NomeStorage = pathFile,
            MimeType = fileCessazione.ContentType,
            DimensioneBytes = fileCessazione.Length,
            Categoria = "CESSAZIONE",
            ApparecchiaturaId = model.ApparecchiaturaId,
            CessazionePraticaId = model.Id,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId  = User.FindFirst(
    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
        });
        await _db.SaveChangesAsync();

    }
    }
    
    _logger.LogInformation(
    "Cessazione di Pratica creata per app {Id}.", model.ApparecchiaturaId);

TempData["Success"] = "Cessazione di Pratica registrata.";

    return RedirectToAction("Detail", "Apparecchiature",
        new { id = model.ApparecchiaturaId, tab = "adempimenti" });
}
        // ═══════════════════════════════════════════════════════════
        // PRIMA VERIFICA BENESTARE
        // ═══════════════════════════════════════════════════════════

[HttpGet]
public async Task<IActionResult> EditPrimaVerifica(int id)
{
    var p = await _db.PrimeVerificheBenestare
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (p == null) return NotFound();

    var app = await _db.Apparecchiature.FindAsync(p.ApparecchiaturaId);

    ViewData["Title"] = "Modifica Prima Verifica e Benestare";
    ViewBag.ApparecchiaturaId = p.ApparecchiaturaId;
    ViewBag.ApparecchiaturaDescrizione = app?.Descrizione;
    ViewBag.IsEdit = true;

    var file =  p.FileAllegati
        .FirstOrDefault(f => f.Categoria == "PRIMA_VERIFICA");
        
ViewBag.FileAttuale = file?.NomeStorage;
ViewBag.FileNome = file?.NomeOriginale; 


    return View("Create_PrimaVerifica", p);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EditPrimaVerifica(
    int id,
    PrimaVerificaBenestare model,
    IFormFile? fileVerifica)
{
    ModelState.Remove("Apparecchiatura");
    ModelState.Remove("FileAllegati");

    if (!ModelState.IsValid)
    {
        ViewBag.ApparecchiaturaId = model.ApparecchiaturaId;
        ViewBag.ApparecchiaturaDescrizione =
            (await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId))?.Descrizione;
        ViewBag.IsEdit = true;

        return View("Create_PrimaVerifica", model);
    }

    var existing = await _db.PrimeVerificheBenestare.FindAsync(id);
    if (existing == null) return NotFound();

    existing.DataVerifica = model.DataVerifica;
    existing.EnteVerificatore = model.EnteVerificatore;
    existing.Note = model.Note;

    if (fileVerifica != null)
    {
        // rimuove il vecchio allegato (db + disco) prima di salvare il nuovo,
        // cosi non si accumulano file orfani e l'edit mostra sempre l'ultimo
        var vecchio = await _db.FileAllegati
            .FirstOrDefaultAsync(f => f.PrimaVerificaBenestareId == existing.Id
                                    && f.Categoria == "PRIMA_VERIFICA");
        if (vecchio != null)
        {
            EliminaFileFisico(vecchio.NomeStorage);
            _db.FileAllegati.Remove(vecchio);
        }

        var path = await SalvaFile(fileVerifica, "prime_verifiche");

        if (path != null)
        {
            _db.FileAllegati.Add(new FileAllegato
            {
                NomeOriginale     = fileVerifica.FileName,
                NomeStorage       = path,
                MimeType          = fileVerifica.ContentType,
                DimensioneBytes   = fileVerifica.Length,
                Categoria         = "PRIMA_VERIFICA",
                ApparecchiaturaId = existing.ApparecchiaturaId,
                PrimaVerificaBenestareId = existing.Id,
                UploadedAt        = DateTime.UtcNow,
                UploadedByUserId  = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
            });
        }
    }

    await _db.SaveChangesAsync();

    TempData["Success"] = "Prima verifica aggiornata.";

    return RedirectToAction("Detail", "Apparecchiature",
        new { id = existing.ApparecchiaturaId, tab = "adempimenti" });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaPrimaVerifica(int id, int apparecchiaturaId)
{
    var item = await _db.PrimeVerificheBenestare
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (item != null)
    {
        // 1. elimina i file fisici dal disco
        foreach (var f in item.FileAllegati)
        {
            EliminaFileFisico(f.NomeStorage);
        }

        // 2. elimina i record degli allegati
        if (item.FileAllegati.Any())
        {
            _db.FileAllegati.RemoveRange(item.FileAllegati);
        }

        // 3. elimina verifica
        _db.PrimeVerificheBenestare.Remove(item);

        await _db.SaveChangesAsync();
    }

    return RedirectToAction("Detail", "Apparecchiature",
        new { id = apparecchiaturaId, tab = "adempimenti" });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaFilePrimaVerifica(int id, int apparecchiaturaId)
{
    // id = Id della PrimaVerificaBenestare, non del FileAllegato:
    // così il bottone "X" nel form puo' richiamare l'azione senza dover
    // conoscere l'Id del FileAllegato, che la view non riceve.
    var file = await _db.FileAllegati
        .FirstOrDefaultAsync(f => f.PrimaVerificaBenestareId == id
                                && f.Categoria == "PRIMA_VERIFICA");

    if (file != null)
    {
        EliminaFileFisico(file.NomeStorage);
        _db.FileAllegati.Remove(file);
        await _db.SaveChangesAsync();
        TempData["Success"] = "File eliminato.";
    }

    return RedirectToAction("EditPrimaVerifica", new { id = id });
}

[HttpGet]
public async Task<IActionResult> NuovaPrimaVerifica(int id)
{
    var app = await _db.Apparecchiature.FindAsync(id);
    if (app == null) return NotFound();

    ViewBag.ApparecchiaturaId = id;
    ViewBag.ApparecchiaturaDescrizione = app.Descrizione;
    ViewBag.IsEdit = false;

    return View("Create_PrimaVerifica", new PrimaVerificaBenestare
    {
        ApparecchiaturaId = id,
        DataVerifica = DateTime.Today,
        FileAllegati = new List<FileAllegato>() // ✅ IMPORTANTISSIMO
    });
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> NuovaPrimaVerifica(
    PrimaVerificaBenestare model,
    IFormFile? fileVerifica)
{
    ModelState.Remove("Apparecchiatura");
    ModelState.Remove("FileAllegati");

    if (!ModelState.IsValid)
        return View("Create_PrimaVerifica", model);


// ✅ 1. SALVA PRIMA LA VERIFICA
    _db.PrimeVerificheBenestare.Add(model);
    await _db.SaveChangesAsync(); // ✅ ORA model.Id è valorizzato

    // ✅ 2. POI SALVA IL FILE (con FK CORRETTA)
    if (fileVerifica != null)
    {

    var path = await SalvaFile(fileVerifica, "prime_verifiche");


    if (path != null)
    {
        _db.FileAllegati.Add(new FileAllegato
        {
            NomeOriginale = fileVerifica!.FileName,
            NomeStorage = path,
            MimeType = fileVerifica.ContentType,
            DimensioneBytes = fileVerifica.Length,
            Categoria = "PRIMA_VERIFICA",
            ApparecchiaturaId = model.ApparecchiaturaId,
            PrimaVerificaBenestareId = model.Id,
            UploadedAt = DateTime.UtcNow,
            UploadedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
        });
         await _db.SaveChangesAsync();
    }
      }
    TempData["Success"] = "Prima verifica registrata.";

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
                Stato             = StatoNullaOsta.Valido,
                FileAllegati = new List<FileAllegato>()
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
            
// ✅ 1. SALVA PRIMA IL NULLA OSTA
    model.Id = 0;
    model.CreatedAt = DateTime.UtcNow;

    _db.NullaOsta.Add(model);
    await _db.SaveChangesAsync(); // ✅ ORA model.Id è valorizzato


            // Upload file
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
                        ApparecchiaturaId = model.ApparecchiaturaId,
                        NullaOstaId       = model.Id, 
                        UploadedAt        = DateTime.UtcNow,
                        UploadedByUserId  = User.FindFirst(
                            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? ""
                    });
                    await _db.SaveChangesAsync();
                }
            }

            
            

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
            var file =  n.FileAllegati
                .FirstOrDefault(f => f.Categoria == "NULLA_OSTA");

            ViewBag.FileAttuale = file?.NomeStorage;
            ViewBag.FileNome = file?.NomeOriginale;

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
                // rimuove il vecchio allegato (db + disco) prima di salvare il nuovo
                var vecchio = await _db.FileAllegati
                    .FirstOrDefaultAsync(f => f.NullaOstaId == existing.Id
                                            && f.Categoria == "NULLA_OSTA");
                if (vecchio != null)
                {
                    EliminaFileFisico(vecchio.NomeStorage);
                    _db.FileAllegati.Remove(vecchio);
                }

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
                        NullaOstaId       = existing.Id,
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
         var item = await _db.NullaOsta
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (item != null)
    {
        // 1. elimina i file fisici dal disco
        foreach (var f in item.FileAllegati)
        {
            EliminaFileFisico(f.NomeStorage);
        }

        // 2. elimina i record degli allegati
        if (item.FileAllegati.Any())
        {
            _db.FileAllegati.RemoveRange(item.FileAllegati);
        }

        // 3. elimina nulla osta
        _db.NullaOsta.Remove(item);

        await _db.SaveChangesAsync();
    }

            return RedirectToAction("Detail", "Apparecchiature",
                new { id = apparecchiaturaId, tab = "adempimenti" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EDR")]
        public async Task<IActionResult> EliminaFileNullaOsta(int id, int apparecchiaturaId)
        {
            // id = Id del NullaOsta, non del FileAllegato.
            var file = await _db.FileAllegati
                .FirstOrDefaultAsync(f => f.NullaOstaId == id
                                        && f.Categoria == "NULLA_OSTA");

            if (file != null)
            {
                EliminaFileFisico(file.NomeStorage);
                _db.FileAllegati.Remove(file);
                await _db.SaveChangesAsync();
                TempData["Success"] = "File eliminato.";
            }

            return RedirectToAction("EditNullaOsta", new { id = id });
        }

        // ═══════════════════════════════════════════════════════════
        // SOPRALLUOGHI  (ex Verbali)
        // ═══════════════════════════════════════════════════════════
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaVerbale(int id, int apparecchiaturaId)
{
    var item = await _db.Verbali
        .Include(x => x.FileAllegati)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (item != null)
    {
        // 1. elimina i file fisici dal disco
        foreach (var f in item.FileAllegati)
        {
            EliminaFileFisico(f.NomeStorage);
        }

        // 2. elimina i record degli allegati
        if (item.FileAllegati.Any())
        {
            _db.FileAllegati.RemoveRange(item.FileAllegati);
        }

        // 3. elimina verbale
        _db.Verbali.Remove(item);

        await _db.SaveChangesAsync();
    }

    return RedirectToAction("Detail", "Apparecchiature",
        new { id = apparecchiaturaId, tab = "adempimenti" });
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "ADMIN_ORG,EDR")]
public async Task<IActionResult> EliminaFileVerbale(int id, int apparecchiaturaId)
{
    // id = Id del Verbale, non del FileAllegato.
    var file = await _db.FileAllegati
        .FirstOrDefaultAsync(f => f.VerbaleId == id
                                && f.Categoria == "VERBALE_SOPRALLUOGO");

    if (file != null)
    {
        EliminaFileFisico(file.NomeStorage);
        _db.FileAllegati.Remove(file);
        await _db.SaveChangesAsync();
        TempData["Success"] = "File eliminato.";
    }

    return RedirectToAction("EditVerbale", new { id = id });
}


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

            var file = v.FileAllegati
                .FirstOrDefault(f => f.Categoria == "VERBALE_SOPRALLUOGO");

            ViewBag.FileAttuale = file?.NomeStorage;
            ViewBag.FileNome = file?.NomeOriginale;

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
                // rimuove il vecchio allegato (db + disco) prima di salvare il nuovo
                var vecchio = await _db.FileAllegati
                    .FirstOrDefaultAsync(f => f.VerbaleId == existing.Id
                                            && f.Categoria == "VERBALE_SOPRALLUOGO");
                if (vecchio != null)
                {
                    EliminaFileFisico(vecchio.NomeStorage);
                    _db.FileAllegati.Remove(vecchio);
                }

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
                        VerbaleId         = existing.Id, // ⚠️ FIX: mancava, il file restava orfano
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

        // ─── HELPER elimina file fisico dal disco ──────────────────────────
        // nomeStorage e' il path relativo salvato in DB, es: "/uploads/cessazioni/xxx_file.pdf"
        private void EliminaFileFisico(string? nomeStorage)
        {
            if (string.IsNullOrWhiteSpace(nomeStorage)) return;

            try
            {
                var relativo = nomeStorage.TrimStart('/', '\\');
                var fullPath = Path.Combine(_env.WebRootPath, relativo);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                // non blocchiamo l'operazione utente se la cancellazione fisica fallisce
                // (es. file gia' rimosso a mano, permessi, file in uso, ecc.)
                _logger.LogWarning(ex,
                    "Impossibile eliminare il file fisico {Path}", nomeStorage);
            }
        }
    }
}