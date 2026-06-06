using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;
using RadiologiaAppNew.Models.Collocazione;
using RadiologiaAppNew.ViewModels.Apparecchiature;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class ApparecchiatureController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ApparecchiatureController> _logger;

        public ApparecchiatureController(
            ApplicationDbContext db,
            ILogger<ApparecchiatureController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index(
            string? search,
            string? stato,
            string? ambito,
            int? reparto,
            int pagina = 1)
        {
            ViewData["Title"] = "Catalogo Apparecchiature";
            ViewData["BreadcrumbParent"] = "Modulo 1";

            const int perPagina = 20;

            var query = _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Locale)
                .Include(a => a.RecordVerifiche)
                .AsQueryable();

            // Filtri
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(a =>
                    a.Descrizione.Contains(search) ||
                    a.Codice.Contains(search) ||
                    a.Modello.Contains(search) ||
                    a.Costruttore.Contains(search) ||
                    a.Matricola.Contains(search));

            if (!string.IsNullOrWhiteSpace(stato) &&
                Enum.TryParse<StatoApparecchiatura>(stato, out var statoEnum))
                query = query.Where(a => a.Stato == statoEnum);
           // else
             //   query = query.Where(a => a.Stato != StatoApparecchiatura.Cessata);

            if (!string.IsNullOrWhiteSpace(ambito) &&
                Enum.TryParse<AmbitoIntervento>(ambito, out var ambitoEnum))
                query = query.Where(a => a.AmbitoIntervento == ambitoEnum);

            if (reparto.HasValue)
                query = query.Where(a => a.RepartoId == reparto);

            var totale = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.Descrizione)
                .Skip((pagina - 1) * perPagina)
                .Take(perPagina)
                .ToListAsync();

            var oggi = DateTime.Today;

            var vm = new ApparecchiaturaListViewModel
            {
                SearchText     = search,
                StatoFiltro    = stato,
                AmbitoFiltro   = ambito,
                RepartoFiltro  = reparto,
                PaginaCorrente = pagina,
                TotaleRisultati = totale,
                TotalePagine   = (int)Math.Ceiling((double)totale / perPagina),
                ElementiPerPagina = perPagina,
                Reparti        = await _db.Reparti.OrderBy(r => r.Nome).ToListAsync(),
                Apparecchiature = items.Select(a =>
                {
                    var prossimaVerifica = a.RecordVerifiche
                        .Where(v => v.ProssimaVerificaData.HasValue)
                        .OrderBy(v => v.ProssimaVerificaData)
                        .FirstOrDefault()?.ProssimaVerificaData;

                    var compliance = "ok";
                    if (prossimaVerifica.HasValue)
                    {
                        if (prossimaVerifica < oggi) compliance = "danger";
                        else if (prossimaVerifica <= oggi.AddDays(30)) compliance = "warning";
                    }
                    else if (!a.DataAccettazione.HasValue)
                    {
                        compliance = "missing";
                    }

                    return new ApparecchiaturaRowViewModel
                    {
                        Id               = a.Id,
                        Codice           = a.Codice,
                        Descrizione      = a.Descrizione,
                        Tipologia        = a.Tipologia,
                        Costruttore      = a.Costruttore,
                        Modello          = a.Modello,
                        Reparto          = a.Reparto?.Nome,
                        Locale           = a.Locale?.Nome,
                        Stato            = a.Stato,
                        Modulo           = a.Modulo,
                        AmbitoIntervento = a.AmbitoIntervento,
                        DataAccettazione = a.DataAccettazione,
                        ProssimaVerifica = prossimaVerifica,
                        ComplianceStato  = compliance,
                        NumeroVerifiche  = a.RecordVerifiche.Count
                    };
                }).ToList()
            };

            return View(vm);
        }

        // ─── DETAIL ──────────────────────────────────────────────────────
        public async Task<IActionResult> Detail(int id, string tab = "anagrafica")
{
    var app = await _db.Apparecchiature
        .Include(a => a.Reparto)
        .Include(a => a.Locale)
            .ThenInclude(l => l != null ? l.Piano : null)
        .Include(a => a.FigureResponsabili)
        .Include(a => a.RecordVerifiche)
            .ThenInclude(v => v.Protocollo)
        .Include(a => a.RecordVerifiche)
            .ThenInclude(v => v.FileAllegati)
        .Include(a => a.FileAllegati)
        .Include(a => a.NotifichePratica)
        .Include(a => a.NullaOsta)
        .Include(a => a.Verbali)
        .FirstOrDefaultAsync(a => a.Id == id);

    if (app == null) return NotFound();

    ViewData["Title"] = app.Descrizione;
    ViewData["BreadcrumbParent"] = "Apparecchiature";
    ViewData["BreadcrumbParentUrl"] = "/Apparecchiature";

    var oggi = DateTime.Today;
    var tra30 = oggi.AddDays(30);

    // ── Carica verifiche separatamente per sicurezza ──────────────
    var verifiche = await _db.RecordVerifiche
        .Include(v => v.Protocollo)
        .Include(v => v.FileAllegati)
        .Where(v => v.ApparecchiaturaId == id)
        .OrderByDescending(v => v.DataInizio)
        .ToListAsync();

// DEBUG TEMPORANEO — rimuovere dopo il fix
_logger.LogWarning("Verifiche trovate per app {Id}: {Count} — Tipi: {Tipi}",
    id,
    verifiche.Count,
    string.Join(", ", verifiche.Select(v => v.Tipo.ToString())));


    // Compliance items
    var compliance = new List<ComplianceItemVm>();

    var accettazione = verifiche
        .FirstOrDefault(v => v.Tipo == TipoProtocollo.Accettazione);
    compliance.Add(new ComplianceItemVm
    {
        Label     = "Collaudo di Accettazione",
        Stato     = accettazione?.Esito == EsitoVerifica.Superato ? "ok"
                  : accettazione != null ? "warning" : "missing",
        Dettaglio = accettazione != null
            ? $"Eseguito il {accettazione.DataInizio:dd/MM/yyyy} — {accettazione.Esito}"
            : "Non ancora eseguito",
        Icona     = "bi-check2-circle"
    });

    var ultimoCQ = verifiche
        .Where(v => v.Tipo == TipoProtocollo.Periodico)
        .OrderByDescending(v => v.DataInizio)
        .FirstOrDefault();
    var prossimoVm = ultimoCQ?.ProssimaVerificaData;
    compliance.Add(new ComplianceItemVm
    {
        Label     = "Controllo di Qualità Periodico",
        Stato     = prossimoVm == null ? "missing"
                  : prossimoVm < oggi ? "danger"
                  : prossimoVm <= tra30 ? "warning" : "ok",
        Dettaglio = prossimoVm.HasValue
            ? $"Prossima verifica: {prossimoVm:dd/MM/yyyy}"
            : "Nessun CQ registrato",
        Icona     = "bi-clipboard2-check"
    });

    var ultimoLdr = verifiche
        .Where(v => v.Tipo == TipoProtocollo.Ldr)
        .OrderByDescending(v => v.DataInizio)
        .FirstOrDefault();
    compliance.Add(new ComplianceItemVm
    {
        Label     = "Verifica LDR",
        Stato     = ultimoLdr == null ? "missing"
                  : ultimoLdr.Anno < oggi.Year - 1 ? "danger" : "ok",
        Dettaglio = ultimoLdr != null
            ? $"Anno {ultimoLdr.Anno} — {ultimoLdr.Esito}"
            : "Nessuna verifica LDR registrata",
        Icona     = "bi-graph-up"
    });

    compliance.Add(new ComplianceItemVm
    {
        Label     = "Registrazione STRIMS",
        Stato     = app.StatoStrims == StatoAdempimento.Registrato ? "ok"
                  : app.StatoStrims == StatoAdempimento.DaRegistrare
                      ? "todo" : "missing",
        Dettaglio = app.StatoStrims == StatoAdempimento.Registrato
            ? $"Registrato il {app.DataRegistrazioneStrims:dd/MM/yyyy}"
            : "Da registrare",
        Icona     = "bi-database-check"
    });

    compliance.Add(new ComplianceItemVm
    {
        Label     = "Registrazione INAIL",
        Stato     = app.StatoInail == StatoAdempimento.Registrato ? "ok"
                  : app.StatoInail == StatoAdempimento.DaRegistrare
                      ? "todo" : "missing",
        Dettaglio = app.StatoInail == StatoAdempimento.Registrato
            ? $"Registrato il {app.DataRegistrazioneInail:dd/MM/yyyy}"
            : "Da registrare",
        Icona     = "bi-building-check"
    });

    var no = app.NullaOsta
        .OrderByDescending(n => n.DataRilascio)
        .FirstOrDefault();
    compliance.Add(new ComplianceItemVm
    {
        Label     = "Nulla Osta",
        Stato     = no == null ? "missing"
                  : no.Stato == StatoNullaOsta.Scaduto ? "danger"
                  : no.DataScadenza.HasValue &&
                    no.DataScadenza <= tra30 ? "warning" : "ok",
        Dettaglio = no != null
            ? $"NO {no.Tipo} — {no.Numero} — {no.Stato}"
            : "Nessun Nulla Osta registrato",
        Icona     = "bi-shield-check"
    });

    var primaVerifica = verifiche
        .FirstOrDefault(v => v.Tipo == TipoProtocollo.PrimaVerificaEdr);
    compliance.Add(new ComplianceItemVm
    {
        Label     = "Prima Verifica EDR",
        Stato     = primaVerifica?.Esito == EsitoVerifica.Superato ? "ok"
                  : primaVerifica != null ? "warning" : "missing",
        Dettaglio = primaVerifica != null
            ? $"Eseguita il {primaVerifica.DataInizio:dd/MM/yyyy}"
            : "Non ancora eseguita",
        Icona     = "bi-person-check"
    });

    var vm = new ApparecchiaturaDetailViewModel
    {
        Apparecchiatura  = app,
        FigureEFM        = app.FigureResponsabili
            .Where(f => f.Ruolo == RuoloResponsabile.EFM).ToList(),
        FigureEDR        = app.FigureResponsabili
            .Where(f => f.Ruolo == RuoloResponsabile.EdR).ToList(),
        FigureRIR        = app.FigureResponsabili
            .Where(f => f.Ruolo == RuoloResponsabile.RIR).ToList(),
        FigureMA         = app.FigureResponsabili
            .Where(f => f.Ruolo == RuoloResponsabile.MA).ToList(),
        VerificheCQ  = verifiche, // mostra TUTTE le verifiche nel tab
VerificheEDR = verifiche
    .Where(v => v.Tipo == TipoProtocollo.PrimaVerificaEdr ||
                v.Tipo == TipoProtocollo.SorveglianzaPeriodicaEdr)
    .ToList(),
        UltimaAccettazione = accettazione,
        NotichePratica   = app.NotifichePratica
            .OrderByDescending(n => n.DataNotifica).ToList(),
        NullaOsta        = app.NullaOsta
            .OrderByDescending(n => n.DataRilascio).ToList(),
        Verbali          = app.Verbali
            .OrderByDescending(v => v.DataSopralluogo).ToList(),
        FileAllegati     = app.FileAllegati
            .OrderByDescending(f => f.UploadedAt).ToList(),
        ItemsCompliance  = compliance,
        TabAttiva        = tab
    };

    return View(vm);
}

        // ─── CREATE GET ──────────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Nuova Apparecchiatura";
            ViewData["BreadcrumbParent"] = "Apparecchiature";
            ViewData["BreadcrumbParentUrl"] = "/Apparecchiature";

            await PopolateViewBag();
            return View(new Apparecchiatura());
        }

        // ─── CREATE POST ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Create(Apparecchiatura model)
        {
            // Rimuovi validazioni per navigation properties
            ModelState.Remove("Locale");
            ModelState.Remove("Reparto");
            ModelState.Remove("FigureResponsabili");
            ModelState.Remove("FileAllegati");
            ModelState.Remove("RecordVerifiche");
            ModelState.Remove("NotichePratica");
            ModelState.Remove("NullaOsta");
            ModelState.Remove("Verbali");

            if (!ModelState.IsValid)
            {
                await PopolateViewBag();
                return View(model);
            }

            // Verifica unicità codice
            if (await _db.Apparecchiature.AnyAsync(a => a.Codice == model.Codice))
            {
                ModelState.AddModelError("Codice", "Questo codice è già in uso.");
                await PopolateViewBag();
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _db.Apparecchiature.Add(model);
            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Apparecchiatura {Codice} creata da {User}.",
                model.Codice, User.Identity?.Name);

            TempData["Success"] =
                $"Apparecchiatura «{model.Descrizione}» creata con successo.";

            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }

        // ─── EDIT GET ────────────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Edit(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            ViewData["Title"] = $"Modifica — {app.Descrizione}";
            ViewData["BreadcrumbParent"] = "Apparecchiature";
            ViewData["BreadcrumbParentUrl"] = "/Apparecchiature";

            await PopolateViewBag();
            return View(app);
        }

        // ─── EDIT POST ───────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Edit(int id, Apparecchiatura model)
        {
            if (id != model.Id) return BadRequest();

            ModelState.Remove("Locale");
            ModelState.Remove("Reparto");
            ModelState.Remove("FigureResponsabili");
            ModelState.Remove("FileAllegati");
            ModelState.Remove("RecordVerifiche");
            ModelState.Remove("NotichePratica");
            ModelState.Remove("NullaOsta");
            ModelState.Remove("Verbali");

            if (!ModelState.IsValid)
            {
                await PopolateViewBag();
                return View(model);
            }

            var existing = await _db.Apparecchiature.FindAsync(id);
            if (existing == null) return NotFound();

            // Aggiorna campi
            existing.Codice              = model.Codice;
            existing.Descrizione         = model.Descrizione;
            existing.Modulo              = model.Modulo;
            existing.AmbitoIntervento    = model.AmbitoIntervento;
            existing.Tipologia           = model.Tipologia;
            existing.Modello             = model.Modello;
            existing.Costruttore         = model.Costruttore;
            existing.Matricola           = model.Matricola;
            existing.SerialNumber        = model.SerialNumber;
            existing.CorrenteMaxMa       = model.CorrenteMaxMa;
            existing.TensioneMaxKvolt    = model.TensioneMaxKvolt;
            existing.EnergiaMaxKev       = model.EnergiaMaxKev;
            existing.IntensitaCampoTesla = model.IntensitaCampoTesla;
            existing.TipoMagnete         = model.TipoMagnete;
            existing.LocaleId            = model.LocaleId;
            existing.RepartoId           = model.RepartoId;
            existing.Caposala            = model.Caposala;
            existing.SocietaManutenzione = model.SocietaManutenzione;
            existing.TecnicoRiferimento  = model.TecnicoRiferimento;
            existing.NumeroAssistenza    = model.NumeroAssistenza;
            existing.GlobalService       = model.GlobalService;
            existing.EmailAssistenza     = model.EmailAssistenza;
            existing.LanCollegata        = model.LanCollegata;
            existing.MedsquareInstallato = model.MedsquareInstallato;
            existing.Stato               = model.Stato;
            existing.DataAccettazione    = model.DataAccettazione;
            existing.DataCessazione      = model.DataCessazione;
            existing.MotivoCessazione    = model.MotivoCessazione;
            existing.SapId               = model.SapId;
            existing.SiapDescrizione     = model.SiapDescrizione;
            existing.StatoInail          = model.StatoInail;
            existing.DataRegistrazioneInail = model.DataRegistrazioneInail;
            existing.NumeroPraticaInail  = model.NumeroPraticaInail;
            existing.StatoStrims         = model.StatoStrims;
            existing.StrimsIdApparecchiatura = model.StrimsIdApparecchiatura;
            existing.DataRegistrazioneStrims = model.DataRegistrazioneStrims;
            existing.StrimsNpCaricata    = model.StrimsNpCaricata;
            existing.StrimsNcCaricata    = model.StrimsNcCaricata;
            existing.DescrizioneZoneClassificate = model.DescrizioneZoneClassificate;
            existing.UpdatedAt           = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Apparecchiatura aggiornata con successo.";
            return RedirectToAction(nameof(Detail), new { id = id });
        }

        // ─── DELETE ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG")]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();

            // Invece di cancellare: imposta come Cessata
            app.Stato           = StatoApparecchiatura.Cessata;
            app.DataCessazione  = DateTime.Today;
            app.UpdatedAt       = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            TempData["Success"] =
                $"Apparecchiatura «{app.Descrizione}» impostata come Cessata.";

            return RedirectToAction(nameof(Index));
        }

        // ─── API AJAX — Tipologie per ambito ────────────────────────────
        [HttpGet]
        public JsonResult GetTipologiePerAmbito(string ambito)
        {
            var tipologie = ambito switch
            {
                "Radiologia" => new[]
                {
                    "Mammografo", "Endorale", "MOC", "Portatile", "TAC",
                    "Pensile", "Telecomandato", "Ortopantomografo", "Altro"
                },
                "RadiologiaInterventistica" => new[]
                {
                    "Angiografo", "Arco a C", "CBCT",
                    "Litotritore", "TAC Interventistica", "Altro"
                },
                "MedicinaNucleare" => new[]
                {
                    "PET-CT", "SPECT", "SPECT-CT", "D-SPECT",
                    "Gammacamera piccolo campo",
                    "Gammacamera grande campo", "Altro"
                },
                "Radioterapia" => new[]
                {
                    "Acceleratore Lineare", "Tomoterapia",
                    "CyberKnife", "GammaPod", "Yort", "Altro"
                },
                "RM" => new[]
                {
                    "RM 1.5T", "RM 3T", "RM 7T", "RM Aperto", "Altro"
                },
                _ => new[] { "Altro" }
            };

            return Json(tipologie);
        }

        // ─── API AJAX — Immobili per sito ────────────────────────────────
        [HttpGet]
        public async Task<JsonResult> GetImmobiliPerSito(int sitoId)
        {
            var immobili = await _db.Immobili
                .Where(i => i.SitoId == sitoId)
                .Select(i => new { i.Id, i.Nome })
                .ToListAsync();
            return Json(immobili);
        }

        // ─── API AJAX — Piani per immobile ───────────────────────────────
        [HttpGet]
        public async Task<JsonResult> GetPianiPerImmobile(int immobileId)
        {
            var piani = await _db.Piani
                .Where(p => p.ImmobileId == immobileId)
                .Select(p => new { p.Id, p.Nome })
                .ToListAsync();
            return Json(piani);
        }

        // ─── API AJAX — Locali per piano ─────────────────────────────────
        [HttpGet]
        public async Task<JsonResult> GetLocaliPerPiano(int pianoId)
        {
            var locali = await _db.Locali
                .Where(l => l.PianoId == pianoId)
                .Select(l => new { l.Id, l.Nome })
                .ToListAsync();
            return Json(locali);
        }

// ─── FIGURE RESPONSABILI ─────────────────────────────────────────

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
public async Task<IActionResult> AggiungiFigura(
    FiguraResponsabile model)
{
    ModelState.Remove("Apparecchiatura");

    if (!ModelState.IsValid)
    {
        TempData["Error"] =
            "Dati non validi. Verifica i campi obbligatori.";
        return RedirectToAction(nameof(Detail),
            new { id = model.ApparecchiaturaId,
                  tab = "anagrafica" });
    }

    model.Id    = 0;
    model.ValidoDal = model.ValidoDal == default
        ? DateTime.Today : model.ValidoDal;

    _db.FigureResponsabili.Add(model);
    await _db.SaveChangesAsync();

    TempData["Success"] =
        $"Figura {model.Ruolo} — {model.Nome} {model.Cognome} aggiunta.";
    return RedirectToAction(nameof(Detail),
        new { id = model.ApparecchiaturaId, tab = "anagrafica" });
}

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
public async Task<IActionResult> EliminaFigura(
    int id, int apparecchiaturaId)
{
    var figura = await _db.FigureResponsabili.FindAsync(id);
    if (figura != null)
    {
        _db.FigureResponsabili.Remove(figura);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Figura rimossa.";
    }
    return RedirectToAction(nameof(Detail),
        new { id = apparecchiaturaId, tab = "anagrafica" });
}

        // ─── HELPER ──────────────────────────────────────────────────────
       private async Task PopolateViewBag()
{
    ViewBag.Siti = new SelectList(
        await _db.Siti.OrderBy(s => s.Nome).ToListAsync(),
        "Id", "Nome");

    ViewBag.Reparti = new SelectList(
        await _db.Reparti.OrderBy(r => r.Nome).ToListAsync(),
        "Id", "Nome");

    ViewBag.Protocolli = await _db.ProtocolliVerifica
        .Where(p => p.Attivo)
        .OrderBy(p => p.Codice)
        .ToListAsync();

    // Nuovo — costruttori e società
    ViewBag.Costruttori = await _db.Costruttori
        .Where(c => c.Attivo)
        .OrderBy(c => c.Nome)
        .ToListAsync();

    ViewBag.SocietaManutenzione = await _db.SocietaManutenzione
        .Where(s => s.Attivo)
        .OrderBy(s => s.Nome)
        .ToListAsync();
}

[HttpGet]
public async Task<JsonResult> GetModelliPerCostruttore(int costruttoreId)
{
    var modelli = await _db.ModelliApparecchiatura
        .Where(m => m.CostrutoreId == costruttoreId && m.Attivo)
        .OrderBy(m => m.Nome)
        .Select(m => new { m.Id, m.Nome, m.Tipologia })
        .ToListAsync();
    return Json(modelli);
}

        // ─── EXPORT CSV ──────────────────────────────────────────────────
[HttpGet]
public async Task<IActionResult> ExportCsv(
    string? stato, string? ambito)
{
    var query = _db.Apparecchiature
        .Include(a => a.Reparto)
        .Include(a => a.Locale)
        .Include(a => a.RecordVerifiche)
        .AsQueryable();

    if (!string.IsNullOrEmpty(stato) &&
        Enum.TryParse<StatoApparecchiatura>(stato, out var s))
        query = query.Where(a => a.Stato == s);
    else
        query = query.Where(a =>
            a.Stato != StatoApparecchiatura.Cessata);

    if (!string.IsNullOrEmpty(ambito) &&
        Enum.TryParse<AmbitoIntervento>(ambito, out var am))
        query = query.Where(a => a.AmbitoIntervento == am);

    var items = await query
        .OrderBy(a => a.Descrizione)
        .ToListAsync();

    var csv = new System.Text.StringBuilder();

    // Header
    csv.AppendLine(
        "Codice;Descrizione;Modulo;Ambito;Tipologia;" +
        "Costruttore;Modello;Matricola;Reparto;Stato;" +
        "DataAccettazione;StatoSTRIMS;StatoINAIL;" +
        "LAN;MedSquare;ProssimaVerifica");

    var oggi = DateTime.Today;

    foreach (var a in items)
    {
        var prossima = a.RecordVerifiche
            .Where(v => v.ProssimaVerificaData.HasValue)
            .OrderBy(v => v.ProssimaVerificaData)
            .FirstOrDefault()?.ProssimaVerificaData;

        csv.AppendLine(string.Join(";",
            Q(a.Codice),
            Q(a.Descrizione),
            Q(a.Modulo.ToString()),
            Q(a.AmbitoIntervento?.ToString() ?? "RM"),
            Q(a.Tipologia),
            Q(a.Costruttore),
            Q(a.Modello),
            Q(a.Matricola),
            Q(a.Reparto?.Nome ?? ""),
            Q(a.Stato.ToString()),
            Q(a.DataAccettazione?.ToString("dd/MM/yyyy") ?? ""),
            Q(a.StatoStrims.ToString()),
            Q(a.StatoInail.ToString()),
            Q(a.LanCollegata ? "Sì" : "No"),
            Q(a.MedsquareInstallato ? "Sì" : "No"),
            Q(prossima?.ToString("dd/MM/yyyy") ?? "")
        ));
    }

    var bytes    = System.Text.Encoding.UTF8.GetPreamble()
        .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
        .ToArray();
    var fileName =
        $"apparecchiature_{DateTime.Today:yyyyMMdd}.csv";

    return File(bytes, "text/csv", fileName);

    static string Q(string s) =>
        $"\"{s.Replace("\"", "\"\"")}\"";
}
    }
}