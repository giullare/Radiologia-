using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;
using RadiologiaAppNew.ViewModels.Verifiche;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class VerificheController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<VerificheController> _logger;

        public VerificheController(
            ApplicationDbContext db,
            ILogger<VerificheController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index(
            int? apparecchiaturaId,
            string? tipo,
            string? esito,
            int? anno,
            string? search,
            int pagina = 1)
        {
            ViewData["Title"] = "Verifiche e Controlli di Qualità";
            ViewData["BreadcrumbParent"] = "Modulo 1";

            const int perPagina = 20;

            var query = _db.RecordVerifiche
                .Include(v => v.Apparecchiatura)
                .Include(v => v.Protocollo)
                .Include(v => v.FileAllegati)
                .AsQueryable();

            // Filtri
            if (apparecchiaturaId.HasValue)
                query = query.Where(v =>
                    v.ApparecchiaturaId == apparecchiaturaId);

            if (!string.IsNullOrEmpty(tipo) &&
                Enum.TryParse<TipoProtocollo>(tipo, out var tipoEnum))
                query = query.Where(v => v.Tipo == tipoEnum);

            if (!string.IsNullOrEmpty(esito) &&
                Enum.TryParse<EsitoVerifica>(esito, out var esitoEnum))
                query = query.Where(v => v.Esito == esitoEnum);

            if (anno.HasValue)
                query = query.Where(v => v.Anno == anno);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(v =>
                    v.Apparecchiatura.Descrizione.Contains(search) ||
                    v.Apparecchiatura.Codice.Contains(search) ||
                    (v.Note != null && v.Note.Contains(search)));

            var totale = await query.CountAsync();

            var items = await query
                .OrderByDescending(v => v.DataInizio)
                .Skip((pagina - 1) * perPagina)
                .Take(perPagina)
                .ToListAsync();

            // Anni disponibili per filtro
            var anni = await _db.RecordVerifiche
                .Where(v => v.Anno.HasValue)
                .Select(v => v.Anno!.Value)
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();

            var vm = new VerificaListViewModel
            {
                ApparecchiaturaIdFiltro = apparecchiaturaId,
                TipoFiltro    = tipo,
                EsitoFiltro   = esito,
                AnnoFiltro    = anno,
                SearchText    = search,
                PaginaCorrente  = pagina,
                TotaleRisultati = totale,
                TotalePagine    = (int)Math.Ceiling((double)totale / perPagina),
                AnniDisponibili = anni,
                Apparecchiature = await _db.Apparecchiature
                    .Where(a => a.Stato == StatoApparecchiatura.Attiva)
                    .OrderBy(a => a.Descrizione)
                    .ToListAsync(),
                Verifiche = items.Select(v => new VerificaRowViewModel
                {
                    Id                = v.Id,
                    Apparecchiatura   = v.Apparecchiatura.Descrizione,
                    ApparecchiaturaId = v.ApparecchiaturaId,
                    Protocollo        = v.Protocollo.Codice,
                    Tipo              = v.Tipo,
                    DataInizio        = v.DataInizio,
                    DataFine          = v.DataFine,
                    Esito             = v.Esito,
                    Anno              = v.Anno,
                    Semestre          = v.Semestre,
                    ProssimaVerificaData = v.ProssimaVerificaData,
                    NumeroAllegati    = v.FileAllegati.Count,
                    InfoGuasto        = v.InfoGuasto
                }).ToList()
            };

            return View(vm);
        }

        // ─── DETAIL ──────────────────────────────────────────────────────
        public async Task<IActionResult> Detail(int id)
        {
            var verifica = await _db.RecordVerifiche
                .Include(v => v.Apparecchiatura)
                .Include(v => v.Protocollo)
                .Include(v => v.FileAllegati)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (verifica == null) return NotFound();

            ViewData["Title"] =
                $"Verifica — {verifica.Apparecchiatura.Descrizione}";
            ViewData["BreadcrumbParent"] = "Verifiche CQ";
            ViewData["BreadcrumbParentUrl"] = "/Verifiche";

            int? giorniScadenza = null;
            if (verifica.ProssimaVerificaData.HasValue)
                giorniScadenza = (verifica.ProssimaVerificaData.Value.Date
                                  - DateTime.Today).Days;

            var vm = new VerificaDetailViewModel
            {
                Verifica = verifica,
                ApparecchiaturaDescrizione =
                    verifica.Apparecchiatura.Descrizione,
                FileAllegati = verifica.FileAllegati
                    .OrderByDescending(f => f.UploadedAt).ToList(),
                Protocollo       = verifica.Protocollo,
                GiorniAllaScadenza = giorniScadenza
            };

            return View(vm);
        }

        // ─── CREATE GET ──────────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,SFM")]
        public async Task<IActionResult> Create(int Id)
        {
            var app = await _db.Apparecchiature.FindAsync(Id);
            if (app == null) return NotFound();

            ViewData["Title"] = "Nuova Verifica CQ";
            ViewData["BreadcrumbParent"] = "Garanzia della Qualità";
            ViewData["BreadcrumbParentUrl"] = "/Verifiche";

            var vm = new VerificaCreateViewModel
            {
                ApparecchiaturaId          = Id,
                ApparecchiaturaDescrizione = app.Descrizione,
                ApparecchiaturaCodice      = app.Codice,
                Anno                       = DateTime.Today.Year,
                TipoUI                     = "FunzionamentoPeriodico",
                EsitoUI                    = "InCorso",
                ProtocolliDisponibili      = await _db.ProtocolliVerifica
                    .Where(p => p.Attivo)
                    .OrderBy(p => p.Tipo)
                    .ThenBy(p => p.Codice)
                    .ToListAsync()
            };

            return View(vm);
        }

        // ─── CREATE POST ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,SFM")]
        public async Task<IActionResult> Create(VerificaCreateViewModel model)
        {
            // Ricarica protocolli per eventuale ritorno alla view
            model.ProtocolliDisponibili = await _db.ProtocolliVerifica
                .Where(p => p.Attivo)
                .OrderBy(p => p.Tipo)
                .ThenBy(p => p.Codice)
                .ToListAsync();

            if (!ModelState.IsValid)
                return View(model);

            var app = await _db.Apparecchiature.FindAsync(model.ApparecchiaturaId);
            if (app == null) return NotFound();

            var protocollo = await _db.ProtocolliVerifica.FindAsync(model.ProtocolloId);
            if (protocollo == null)
            {
                ModelState.AddModelError("ProtocolloId", "Protocollo non trovato.");
                return View(model);
            }

            // Mappa TipoUI → TipoProtocollo
            model.Tipo  = TipoUIHelper.ToTipoProtocollo(model.TipoUI);
            // Mappa EsitoUI → EsitoVerifica
            model.Esito = TipoUIHelper.ToEsitoVerifica(model.EsitoUI);

            // Calcola periodicità in mesi dalla selezione UI
            int? periodicitaMesi = TipoUIHelper.PeriodicitaToMesi(
                model.Periodicita, model.PeriodicitaAltroMesi)
                ?? protocollo.PeriodicitaMesi;

            // Calcola automaticamente prossima verifica
            DateTime? prossimaVerifica = model.ProssimaVerificaData;
            if (!prossimaVerifica.HasValue && periodicitaMesi.HasValue)
                prossimaVerifica = model.DataInizio.AddMonths(periodicitaMesi.Value);

            var verifica = new RecordVerifica
            {
                ApparecchiaturaId           = model.ApparecchiaturaId,
                ProtocolloId                = model.ProtocolloId,
                Tipo                        = model.Tipo,
                DataInizio                  = model.DataInizio,
                DataFine                    = null, // rimosso
                Esito                       = model.Esito,
                Note                        = model.Note,
                Anno                        = model.Anno,
                Semestre                    = model.Semestre,
                ProssimaVerificaData        = prossimaVerifica,
                InfoGuasto                  = model.InfoGuasto,
                TipoGuasto                  = model.TipoGuasto,
                BenestareQualitaTecnicaData = model.BenestareQualitaTecnicaData,
                BenestareQualitaTecnicaBy   = model.BenestareQualitaTecnicaBy,
                BenestareCliniciData        = model.BenestareCliniciData,
                BenestareClinicoBy          = model.BenestareClinicoBy,
                TipoInterventoManutenzione  = model.TipoInterventoManutenzione,
                TecnicoManutentore          = model.TecnicoManutentore,
                SocietaManutenzione         = model.SocietaManutenzione,
                DataInterventoManutenzione  = model.DataInterventoManutenzione,
                CreatedAt                   = DateTime.UtcNow,
                CreatedByUserId             = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            if (model.Tipo == TipoProtocollo.PostManutenzione &&
                model.Esito == EsitoVerifica.InCorso)
            {
                app.Stato     = StatoApparecchiatura.InManutenzione;
                app.UpdatedAt = DateTime.UtcNow;
            }

            if (model.Tipo == TipoProtocollo.Accettazione &&
                model.Esito == EsitoVerifica.Positivo &&
                !app.DataAccettazione.HasValue)
            {
                app.DataAccettazione = model.DataInizio;
                app.Stato            = StatoApparecchiatura.Attiva;
                app.UpdatedAt        = DateTime.UtcNow;
            }

            _db.RecordVerifiche.Add(verifica);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Verifica {Tipo} creata per apparecchiatura {Id} da {User}.",
                model.Tipo, model.ApparecchiaturaId, User.Identity?.Name);

            TempData["Success"] =
                $"Verifica registrata con successo. " +
                (prossimaVerifica.HasValue
                    ? $"Prossimo controllo: {prossimaVerifica.Value:dd/MM/yyyy}" : "");

            return RedirectToAction("Detail", "Apparecchiature",
                new { id = model.ApparecchiaturaId, tab = "verifiche" });
        }

        // ─── EDIT GET ────────────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,SFM")]
        public async Task<IActionResult> Edit(int id)
        {
            var v = await _db.RecordVerifiche
                .Include(v => v.Apparecchiatura)
                .Include(v => v.Protocollo)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v == null) return NotFound();

            ViewData["Title"] = "Modifica Verifica";
            ViewData["BreadcrumbParent"] = "Verifiche CQ";
            ViewData["BreadcrumbParentUrl"] = "/Verifiche";

            var vm = new VerificaCreateViewModel
            {
                ApparecchiaturaId          = v.ApparecchiaturaId,
                ApparecchiaturaDescrizione = v.Apparecchiatura.Descrizione,
                ApparecchiaturaCodice      = v.Apparecchiatura.Codice,
                ProtocolloId              = v.ProtocolloId,
                TipoUI                    = TipoUIHelper.ToTipoUI(v.Tipo),
                Tipo                      = v.Tipo,
                EsitoUI                   = TipoUIHelper.ToEsitoUI(v.Esito),
                Esito                     = v.Esito,
                DataInizio                = v.DataInizio,
                Note                      = v.Note,
                Anno                      = v.Anno,
                Semestre                  = v.Semestre,
                ProssimaVerificaData      = v.ProssimaVerificaData,
                InfoGuasto                = v.InfoGuasto,
                TipoGuasto                = v.TipoGuasto,
                BenestareQualitaTecnicaData = v.BenestareQualitaTecnicaData,
                BenestareQualitaTecnicaBy = v.BenestareQualitaTecnicaBy,
                BenestareCliniciData      = v.BenestareCliniciData,
                BenestareClinicoBy        = v.BenestareClinicoBy,
                TipoInterventoManutenzione = v.TipoInterventoManutenzione,
                TecnicoManutentore        = v.TecnicoManutentore,
                SocietaManutenzione       = v.SocietaManutenzione,
                DataInterventoManutenzione = v.DataInterventoManutenzione,
                ProtocolliDisponibili = await _db.ProtocolliVerifica
                    .Where(p => p.Attivo)
                    .OrderBy(p => p.Tipo)
                    .ThenBy(p => p.Codice)
                    .ToListAsync()
            };

            ViewBag.VerificaId = id;
            return View("Create", vm);
        }

        // ─── EDIT POST ───────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,SFM")]
        public async Task<IActionResult> Edit(int id,
            VerificaCreateViewModel model)
        {
            model.ProtocolliDisponibili = await _db.ProtocolliVerifica
                .Where(p => p.Attivo).OrderBy(p => p.Codice).ToListAsync();

            if (!ModelState.IsValid)
            {
                ViewBag.VerificaId = id;
                return View("Create", model);
            }

            var verifica = await _db.RecordVerifiche.FindAsync(id);
            if (verifica == null) return NotFound();

            var protocollo = await _db.ProtocolliVerifica
                .FindAsync(model.ProtocolloId);

            // Mappa UI → enum
            model.Tipo  = TipoUIHelper.ToTipoProtocollo(model.TipoUI);
            model.Esito = TipoUIHelper.ToEsitoVerifica(model.EsitoUI);

            int? periodicitaMesi = TipoUIHelper.PeriodicitaToMesi(
                model.Periodicita, model.PeriodicitaAltroMesi)
                ?? protocollo?.PeriodicitaMesi;

            DateTime? prossimaVerifica = model.ProssimaVerificaData;
            if (!prossimaVerifica.HasValue && periodicitaMesi.HasValue)
                prossimaVerifica = model.DataInizio.AddMonths(periodicitaMesi.Value);

            verifica.ProtocolloId              = model.ProtocolloId;
            verifica.Tipo                      = model.Tipo;
            verifica.DataInizio                = model.DataInizio;
            verifica.DataFine                  = null;
            verifica.Esito                     = model.Esito;
            verifica.Note                      = model.Note;
            verifica.Anno                      = model.Anno;
            verifica.Semestre                  = model.Semestre;
            verifica.ProssimaVerificaData      = prossimaVerifica;
            verifica.InfoGuasto                = model.InfoGuasto;
            verifica.TipoGuasto                = model.TipoGuasto;
            verifica.BenestareQualitaTecnicaData = model.BenestareQualitaTecnicaData;
            verifica.BenestareQualitaTecnicaBy = model.BenestareQualitaTecnicaBy;
            verifica.BenestareCliniciData      = model.BenestareCliniciData;
            verifica.BenestareClinicoBy        = model.BenestareClinicoBy;
            verifica.TipoInterventoManutenzione = model.TipoInterventoManutenzione;
            verifica.TecnicoManutentore        = model.TecnicoManutentore;
            verifica.SocietaManutenzione       = model.SocietaManutenzione;
            verifica.DataInterventoManutenzione = model.DataInterventoManutenzione;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Verifica aggiornata con successo.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        // ─── DELETE ──────────────────────────────────────────────────────
        [HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "ADMIN_ORG")]
public async Task<IActionResult> Delete(int id)
{
    var verifica = await _db.RecordVerifiche
        .Include(v => v.FileAllegati)
        .FirstOrDefaultAsync(v => v.Id == id);

    if (verifica == null) return NotFound();

    var appId = verifica.ApparecchiaturaId;

    // 🔥 elimina file prima
    if (verifica.FileAllegati.Any())
    {
        _db.FileAllegati.RemoveRange(verifica.FileAllegati);
    }

    _db.RecordVerifiche.Remove(verifica);

    await _db.SaveChangesAsync();

    TempData["Success"] = "Verifica eliminata.";
    return RedirectToAction("Detail", "Apparecchiature",
        new { id = appId, tab = "verifiche" });
}

        // ─── API — File protocollo ────────────────────────────────────────
        [HttpGet]
        public async Task<JsonResult> GetProtocolloFile(int id)
        {
            var p = await _db.ProtocolliVerifica.FindAsync(id);
            if (p == null) return Json(new { fileUrl = (string?)null });
            // Il file è salvato come FileAllegato con categoria ProtocolloVerifica
            var file = await _db.FileAllegati
                .Where(f => f.Categoria == "PROTOCOLLO_VERIFICA" &&
                            f.ApparecchiaturaId == null)
                .OrderByDescending(f => f.UploadedAt)
                .FirstOrDefaultAsync();
            return Json(new { fileUrl = file?.NomeStorage });
        }

        // ─── API — Protocolli per tipo ────────────────────────────────────
        [HttpGet]
        public async Task<JsonResult> GetProtocolliPerTipo(string tipo)
        {
            if (!Enum.TryParse<TipoProtocollo>(tipo, out var tipoEnum))
                return Json(new List<object>());

            var protocolli = await _db.ProtocolliVerifica
                .Where(p => p.Attivo && p.Tipo == tipoEnum)
                .OrderBy(p => p.Codice)
                .Select(p => new
                {
                    p.Id,
                    p.Codice,
                    p.Descrizione,
                    p.PeriodicitaMesi,
                    p.Revisione
                })
                .ToListAsync();

            return Json(protocolli);
        }

// ─── EXPORT CSV ──────────────────────────────────────────────────
        [HttpGet]
public async Task<IActionResult> ExportCsv(
    int? apparecchiaturaId, string? tipo, string? esito)
{
    var query = _db.RecordVerifiche
        .Include(v => v.Apparecchiatura)
        .Include(v => v.Protocollo)
        .AsQueryable();

    if (apparecchiaturaId.HasValue)
        query = query.Where(v =>
            v.ApparecchiaturaId == apparecchiaturaId);

    if (!string.IsNullOrEmpty(tipo) &&
        Enum.TryParse<TipoProtocollo>(tipo, out var t))
        query = query.Where(v => v.Tipo == t);

    if (!string.IsNullOrEmpty(esito) &&
        Enum.TryParse<EsitoVerifica>(esito, out var e))
        query = query.Where(v => v.Esito == e);

    var items = await query
        .OrderByDescending(v => v.DataInizio)
        .ToListAsync();

    var csv = new System.Text.StringBuilder();
    csv.AppendLine(
        "Apparecchiatura;Codice;Protocollo;Tipo;" +
        "DataInizio;DataFine;Esito;Anno;ProssimaVerifica;Note");

    foreach (var v in items)
    {
        csv.AppendLine(string.Join(";",
            Q(v.Apparecchiatura.Descrizione),
            Q(v.Apparecchiatura.Codice),
            Q(v.Protocollo.Codice),
            Q(v.Tipo.ToString()),
            Q(v.DataInizio.ToString("dd/MM/yyyy")),
            Q(v.DataFine?.ToString("dd/MM/yyyy") ?? ""),
            Q(v.Esito.ToString()),
            Q(v.Anno?.ToString() ?? ""),
            Q(v.ProssimaVerificaData?.ToString("dd/MM/yyyy") ?? ""),
            Q(v.Note ?? "")
        ));
    }

    var bytes    = System.Text.Encoding.UTF8.GetPreamble()
        .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
        .ToArray();
    var fileName =
        $"verifiche_{DateTime.Today:yyyyMMdd}.csv";

    return File(bytes, "text/csv", fileName);

    static string Q(string s) =>
        $"\"{s.Replace("\"", "\"\"")}\"";
}
    }
}