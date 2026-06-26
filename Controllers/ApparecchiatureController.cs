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
        private readonly IWebHostEnvironment _env;
        public ApparecchiatureController(
            ApplicationDbContext db,
            ILogger<ApparecchiatureController> logger,
            IWebHostEnvironment env)
        {
            _db     = db;
            _logger = logger;
            _env    = env;
        }
        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index(
            string? search,
            string? stato,
            string? ambito,
            string? tipologia,   // NUOVO — sostituisce reparto nel filtro
            int pagina = 1)
        {
            ViewData["Title"] = "Inventario Apparecchiature";
            ViewData["BreadcrumbParent"] = "Modulo 1";
            const int perPagina = 20;
            var query = _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Locale)
                .Include(a => a.RecordVerifiche)
                .AsQueryable();
            // Solo modulo Radiologica in questo elenco
            query = query.Where(a => a.Modulo == TipoModulo.Radiologica);
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
            if (!string.IsNullOrWhiteSpace(ambito) &&
                Enum.TryParse<AmbitoIntervento>(ambito, out var ambitoEnum))
                query = query.Where(a => a.AmbitoIntervento == ambitoEnum);
            // Filtro tipologia (dipendente da ambito)
            if (!string.IsNullOrWhiteSpace(tipologia))
                query = query.Where(a => a.Tipologia == tipologia);
            var totale = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.Descrizione)
                .Skip((pagina - 1) * perPagina)
                .Take(perPagina)
                .ToListAsync();
            var oggi = DateTime.Today;
            var vm = new ApparecchiaturaListViewModel
            {
                SearchText        = search,
                StatoFiltro       = stato,
                AmbitoFiltro      = ambito,
                TipologiaFiltro   = tipologia,
                PaginaCorrente    = pagina,
                TotaleRisultati   = totale,
                TotalePagine      = (int)Math.Ceiling((double)totale / perPagina),
                ElementiPerPagina = perPagina,
                Reparti           = await _db.Reparti.OrderBy(r => r.Nome).ToListAsync(),
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
                        compliance = "missing";
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
            // Passa tipologie per ambito selezionato al filtro
            var tipologie = new List<string>();
if (!string.IsNullOrWhiteSpace(ambito))
    tipologie = GetTipologieList(ambito).ToList();
// ✅ FIX: mantieni la tipologia selezionata
if (!string.IsNullOrWhiteSpace(tipologia) && !tipologie.Contains(tipologia))
{
    tipologie.Add(tipologia);
}
ViewBag.TipologiePerAmbito = tipologie;
            return View(vm);
        }
        // ─── DETAIL ──────────────────────────────────────────────────────
        public async Task<IActionResult> Detail(int id, string tab = "anagrafica")
        {
            var app = await _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Dipartimento)
                .Include(a => a.Locale)                
                  .ThenInclude(l => l.Piano)
                  .ThenInclude(p => p.Immobile)
                  .ThenInclude(i => i.Sito)
                .Include(a => a.FigureResponsabili)
                .Include(a => a.RecordVerifiche)
                    .ThenInclude(v => v.Protocollo)
                .Include(a => a.RecordVerifiche)
                    .ThenInclude(v => v.FileAllegati)
                .Include(a => a.FileAllegati)
                .Include(a => a.NotifichePratica)
                    .ThenInclude(n => n.FileAllegati)
                .Include(a => a.CessazioniPratica)
                    .ThenInclude(c => c.FileAllegati)
                .Include(a => a.PrimeVerificheBenestare)
                    .ThenInclude(p => p.FileAllegati)
                .Include(a => a.NullaOsta)
                    .ThenInclude(n => n.FileAllegati)
                .Include(a => a.Verbali)
                    .ThenInclude(v => v.FileAllegati)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound();
            ViewData["Title"] = app.Descrizione;
            ViewData["BreadcrumbParent"] = "Inventario";
            ViewData["BreadcrumbParentUrl"] = "/Apparecchiature";
            var oggi  = DateTime.Today;
            var tra30 = oggi.AddDays(30);
            var verifiche = await _db.RecordVerifiche
                .Include(v => v.Protocollo)
                .Include(v => v.FileAllegati)
                .Where(v => v.ApparecchiaturaId == id)
                .OrderByDescending(v => v.DataInizio)
                .ToListAsync();
            var compliance = new List<ComplianceItemVm>();
            var accettazione = verifiche.FirstOrDefault(v => v.Tipo == TipoProtocollo.Accettazione);
            compliance.Add(new ComplianceItemVm { Label = "Collaudo di Accettazione", Stato = accettazione?.Esito == EsitoVerifica.Superato ? "ok" : accettazione != null ? "warning" : "missing", Dettaglio = accettazione != null ? $"Eseguito il {accettazione.DataInizio:dd/MM/yyyy} — {accettazione.Esito}" : "Non ancora eseguito", Icona = "bi-check2-circle" });
            var ultimoCQ  = verifiche.Where(v => v.Tipo == TipoProtocollo.Periodico).OrderByDescending(v => v.DataInizio).FirstOrDefault();
            var prossimoVm = ultimoCQ?.ProssimaVerificaData;
            compliance.Add(new ComplianceItemVm { Label = "Controllo Periodico/Manutentivo", Stato = prossimoVm == null ? "missing" : prossimoVm < oggi ? "danger" : prossimoVm <= tra30 ? "warning" : "ok", Dettaglio = prossimoVm.HasValue ? $"Prossimo controllo: {prossimoVm:dd/MM/yyyy}" : "Nessun controllo registrato", Icona = "bi-clipboard2-check" });
            var ultimoLdr = verifiche.Where(v => v.Tipo == TipoProtocollo.Ldr).OrderByDescending(v => v.DataInizio).FirstOrDefault();
            compliance.Add(new ComplianceItemVm { Label = "Verifica LDR", Stato = ultimoLdr == null ? "missing" : ultimoLdr.Anno < oggi.Year - 1 ? "danger" : "ok", Dettaglio = ultimoLdr != null ? $"Anno {ultimoLdr.Anno} — {ultimoLdr.Esito}" : "Nessuna verifica LDR registrata", Icona = "bi-graph-up" });
            compliance.Add(new ComplianceItemVm { Label = "Registrazione STRIMS", Stato = app.StatoStrims == StatoAdempimento.Registrato ? "ok" : "todo", Dettaglio = app.StatoStrims == StatoAdempimento.Registrato ? $"Registrato il {app.DataRegistrazioneStrims:dd/MM/yyyy}" : "Da registrare", Icona = "bi-database-check" });
            compliance.Add(new ComplianceItemVm { Label = "Registrazione INAIL", Stato = app.StatoInail == StatoAdempimento.Registrato ? "ok" : "todo", Dettaglio = app.StatoInail == StatoAdempimento.Registrato ? $"Registrato il {app.DataRegistrazioneInail:dd/MM/yyyy}" : "Da registrare", Icona = "bi-building-check" });
            var no = app.NullaOsta.OrderByDescending(n => n.DataRilascio).FirstOrDefault();
            compliance.Add(new ComplianceItemVm { Label = "Nulla Osta", Stato = no == null ? "missing" : no.Stato == StatoNullaOsta.Scaduto ? "danger" : no.DataScadenza.HasValue && no.DataScadenza <= tra30 ? "warning" : "ok", Dettaglio = no != null ? $"NO {no.Tipo} — {no.Numero} — {no.Stato}" : "Nessun Nulla Osta", Icona = "bi-shield-check" });
            var primaVerifica = verifiche.FirstOrDefault(v => v.Tipo == TipoProtocollo.PrimaVerificaEdr);
            compliance.Add(new ComplianceItemVm { Label = "Prima Verifica EDR", Stato = primaVerifica?.Esito == EsitoVerifica.Superato ? "ok" : primaVerifica != null ? "warning" : "missing", Dettaglio = primaVerifica != null ? $"Eseguita il {primaVerifica.DataInizio:dd/MM/yyyy}" : "Non ancora eseguita", Icona = "bi-person-check" });
            var vm = new ApparecchiaturaDetailViewModel
            {
                Apparecchiatura  = app,
                FigureEFM        = app.FigureResponsabili.Where(f => f.Ruolo == RuoloResponsabile.EFM).ToList(),
                FigureEDR        = app.FigureResponsabili.Where(f => f.Ruolo == RuoloResponsabile.EdR).ToList(),
                FigureRIR        = app.FigureResponsabili.Where(f => f.Ruolo == RuoloResponsabile.RIR).ToList(),
                FigureMA         = app.FigureResponsabili.Where(f => f.Ruolo == RuoloResponsabile.MA).ToList(),
                VerificheCQ      = verifiche,
                VerificheEDR     = verifiche.Where(v => v.Tipo == TipoProtocollo.PrimaVerificaEdr || v.Tipo == TipoProtocollo.SorveglianzaPeriodicaEdr).ToList(),
                UltimaAccettazione = accettazione,
                NotichePratica   = app.NotifichePratica.OrderByDescending(n => n.DataNotifica).ToList(),
                PrimeVerificheBenestare = app.PrimeVerificheBenestare.OrderByDescending(p => p.DataVerifica).ToList(),
                CessazioniPratica = app.CessazioniPratica.OrderByDescending(c => c.DataCessazione).ToList(),
                NullaOsta        = app.NullaOsta.OrderByDescending(n => n.DataRilascio).ToList(),
                Verbali          = app.Verbali.OrderByDescending(v => v.DataSopralluogo).ToList(),
                FileAllegati     = app.FileAllegati.OrderByDescending(f => f.UploadedAt).ToList(),
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
            ViewData["BreadcrumbParent"] = "Inventario";
            ViewData["BreadcrumbParentUrl"] = "/Apparecchiature";
            await PopolateViewBag();
            return View(new Apparecchiatura { Modulo = TipoModulo.Radiologica });
        }
        // ─── CREATE POST ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Create(
            Apparecchiatura model,
            IFormFile? filePiantina,
            IFormFile? fileInailReg,
            IFormFile? fileInailCess,
            IFormFile? fileStrimsReg,
            IFormFile? fileStrimsCess)
        {
            ModelState.Remove("Locale");
            ModelState.Remove("Reparto");
            ModelState.Remove("Dipartimento");
            ModelState.Remove("FigureResponsabili");
            ModelState.Remove("FileAllegati");
            ModelState.Remove("RecordVerifiche");
            ModelState.Remove("NotichePratica");
            ModelState.Remove("NullaOsta");
            ModelState.Remove("Verbali");
            
// ✅ COLLOCAZIONE
ModelState.Remove("LocaleId");
ModelState.Remove("SitoId");
ModelState.Remove("ImmobileId");
ModelState.Remove("PianoId");
            // Forza modulo Radiologica
            model.Modulo = TipoModulo.Radiologica;
            if (!ModelState.IsValid)
            {
                await PopolateViewBag();
                return View(model);
            }
            if (await _db.Apparecchiature.AnyAsync(a => a.Codice == model.Codice))
            {
                ModelState.AddModelError("Codice", "Questo codice è già in uso.");
                await PopolateViewBag();
                return View(model);
            }
            // Upload file
            model.PiantinaZoneClassificateFile   = await SalvaFile(filePiantina, "piantina");
            if (model.PiantinaZoneClassificateFile != null)
                model.PiantinaZoneClassificateNomeOriginale = filePiantina!.FileName;
            model.InailRicevutaRegistrazioneFile = await SalvaFile(fileInailReg, "inail");
            if (model.InailRicevutaRegistrazioneFile != null)
                model.InailRicevutaRegistrazioneNomeOriginale = fileInailReg!.FileName;
            model.InailRicevutaCessioneFile      = await SalvaFile(fileInailCess, "inail");
            if (model.InailRicevutaCessioneFile != null)
                model.InailRicevutaCessioneNomeOriginale = fileInailCess!.FileName;
            model.StrimsRicevutaRegistrazioneFile = await SalvaFile(fileStrimsReg, "strims");
            if (model.StrimsRicevutaRegistrazioneFile != null)
                model.StrimsRicevutaRegistrazioneNomeOriginale = fileStrimsReg!.FileName;
            model.StrimsRicevutaCessioneFile     = await SalvaFile(fileStrimsCess, "strims");
            if (model.StrimsRicevutaCessioneFile != null)
                model.StrimsRicevutaCessioneNomeOriginale = fileStrimsCess!.FileName;
            model.CreatedAt        = DateTime.UtcNow;
            model.UpdatedAt        = DateTime.UtcNow;
            model.CreatedByUserId  = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _db.Apparecchiature.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Apparecchiatura «{model.Descrizione}» creata con successo.";
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }
        // ─── INDEX RM ────────────────────────────────────────────────────
        public async Task<IActionResult> IndexRM(
            string? search,
            string? stato,
            string? tipologia,
            int pagina = 1)
        {
            ViewData["Title"] = "Apparecchiature RM";
            ViewData["BreadcrumbParent"] = "Apparecchiature a Risonanza Magnetica (DM 14/01/21)";
            ViewData["IsRM"] = true;
            const int perPagina = 20;
            var query = _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Locale)
                .Include(a => a.RecordVerifiche)
                .Where(a => a.Modulo == TipoModulo.RM)
                .AsQueryable();
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
            if (!string.IsNullOrWhiteSpace(tipologia))
                query = query.Where(a => a.Tipologia == tipologia);
            var totale = await query.CountAsync();
            var items  = await query
                .OrderBy(a => a.Descrizione)
                .Skip((pagina - 1) * perPagina)
                .Take(perPagina)
                .ToListAsync();
            var oggi = DateTime.Today;
            var vm = new ApparecchiaturaListViewModel
            {
                SearchText        = search,
                StatoFiltro       = stato,
                TipologiaFiltro   = tipologia,
                PaginaCorrente    = pagina,
                TotaleRisultati   = totale,
                TotalePagine      = (int)Math.Ceiling((double)totale / perPagina),
                ElementiPerPagina = perPagina,
                Reparti           = await _db.Reparti.OrderBy(r => r.Nome).ToListAsync(),
                Apparecchiature   = items.Select(a =>
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
                        compliance = "missing";
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
            ViewBag.TipologiePerAmbito = GetTipologieList("RM").ToList();
            return View(vm);
        }
        // ─── CREATE RM GET ───────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,ES,MR")]
        public async Task<IActionResult> CreateRM()
        {
            ViewData["Title"] = "Nuova Apparecchiatura RM";
            ViewData["BreadcrumbParent"] = "Apparecchiature RM";
            ViewData["BreadcrumbParentUrl"] = "/Apparecchiature/IndexRM";
            ViewData["IsRM"] = true;
            await PopolateViewBag();
            return View("Create", new Apparecchiatura { Modulo = TipoModulo.RM });
        }
        // ─── CREATE RM POST ──────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,ES,MR")]
        public async Task<IActionResult> CreateRM(
            Apparecchiatura model,
            IFormFile? filePiantina,
            IFormFile? fileInailReg,
            IFormFile? fileInailCess,
            IFormFile? fileStrimsReg,
            IFormFile? fileStrimsCess)
        {
            ModelState.Remove("Locale");
            ModelState.Remove("Reparto");
            ModelState.Remove("Dipartimento");
            ModelState.Remove("FigureResponsabili");
            ModelState.Remove("FileAllegati");
            ModelState.Remove("RecordVerifiche");
            ModelState.Remove("NotichePratica");
            ModelState.Remove("NullaOsta");
            ModelState.Remove("Verbali");
            // Forza modulo RM
            model.Modulo = TipoModulo.RM;
            if (!ModelState.IsValid)
            {
                ViewData["IsRM"] = true;
                await PopolateViewBag();
                return View("Create", model);
            }
            if (await _db.Apparecchiature.AnyAsync(a => a.Codice == model.Codice))
            {
                ModelState.AddModelError("Codice", "Questo codice è già in uso.");
                ViewData["IsRM"] = true;
                await PopolateViewBag();
                return View("Create", model);
            }
            model.PiantinaZoneClassificateFile    = await SalvaFile(filePiantina, "piantina");
            if (model.PiantinaZoneClassificateFile != null)
                model.PiantinaZoneClassificateNomeOriginale = filePiantina!.FileName;
            model.InailRicevutaRegistrazioneFile  = await SalvaFile(fileInailReg, "inail");
            if (model.InailRicevutaRegistrazioneFile != null)
                model.InailRicevutaRegistrazioneNomeOriginale = fileInailReg!.FileName;
            model.InailRicevutaCessioneFile       = await SalvaFile(fileInailCess, "inail");
            if (model.InailRicevutaCessioneFile != null)
                model.InailRicevutaCessioneNomeOriginale = fileInailCess!.FileName;
            model.StrimsRicevutaRegistrazioneFile = await SalvaFile(fileStrimsReg, "strims");
            if (model.StrimsRicevutaRegistrazioneFile != null)
                model.StrimsRicevutaRegistrazioneNomeOriginale = fileStrimsReg!.FileName;
            model.StrimsRicevutaCessioneFile      = await SalvaFile(fileStrimsCess, "strims");
            if (model.StrimsRicevutaCessioneFile != null)
                model.StrimsRicevutaCessioneNomeOriginale = fileStrimsCess!.FileName;
            model.CreatedAt       = DateTime.UtcNow;
            model.UpdatedAt       = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _db.Apparecchiature.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] =
                $"Apparecchiatura RM «{model.Descrizione}» creata con successo.";
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }
        // ─── EDIT GET ────────────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR,ES,MR")]
        public async Task<IActionResult> Edit(int id)
        {
           // var app = await _db.Apparecchiature.FindAsync(id);
           var app = await _db.Apparecchiature
    .Include(a => a.Locale)
        .ThenInclude(l => l.Piano)
            .ThenInclude(p => p.Immobile)
    .FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound();
            ViewData["Title"] = $"Modifica — {app.Descrizione}";
            if (app.Modulo == TipoModulo.RM)
            {
                ViewData["BreadcrumbParent"]    = "Apparecchiature RM";
                ViewData["BreadcrumbParentUrl"] = "/Apparecchiature/IndexRM";
                ViewData["IsRM"] = true;
            }
            else
            {
                ViewData["BreadcrumbParent"]    = "Inventario";
                ViewData["BreadcrumbParentUrl"] = "/Apparecchiature";
            }
            await PopolateViewBag();
            return View(app);
        }
        // ─── EDIT POST ───────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Edit(
            int id,
            Apparecchiatura model,
            IFormFile? filePiantina,
            IFormFile? fileInailReg,
            IFormFile? fileInailCess,
            IFormFile? fileStrimsReg,
            IFormFile? fileStrimsCess)
        {
            if (id != model.Id) return BadRequest();
            ModelState.Remove("Locale");
            ModelState.Remove("Reparto");
            ModelState.Remove("Dipartimento");
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
            // ── Campi base
            existing.Codice              = model.Codice;
            existing.Descrizione         = model.Descrizione;
            // Preserva il modulo originale — non sovrascrivere
            existing.AmbitoIntervento    = model.AmbitoIntervento;
            existing.Tipologia           = model.Tipologia;
            existing.SapId               = model.SapId;
            existing.SiapDescrizione     = model.SiapDescrizione;
            existing.Modello             = model.Modello;
            existing.Costruttore         = model.Costruttore;
            existing.Matricola           = model.Matricola;
            existing.SerialNumber        = model.SerialNumber;
            existing.CorrenteMaxMa       = model.CorrenteMaxMa;
            existing.TensioneMaxKvolt    = model.TensioneMaxKvolt;
            existing.EnergiaMaxKev       = model.EnergiaMaxKev;
            existing.IntensitaCampoTesla = model.IntensitaCampoTesla;
            existing.TipoMagnete         = model.TipoMagnete;
            existing.LanCollegata        = model.LanCollegata;
            existing.MedsquareInstallato = model.MedsquareInstallato;
            // ── Collocazione
            existing.LocaleId            = model.LocaleId;
            existing.SitoId     = model.SitoId;
existing.ImmobileId = model.ImmobileId;
existing.PianoId    = model.PianoId;
            existing.RepartoId           = model.RepartoId;
            existing.DipartimentoId      = model.DipartimentoId;
            // ── Responsabili
            existing.DirettoreDipartimento          = model.DirettoreDipartimento;
            existing.DirettoreDipartimentoEmail     = model.DirettoreDipartimentoEmail;
            existing.DirettoreDipartimentoEmailPec  = model.DirettoreDipartimentoEmailPec;
            existing.DirettoreDipartimentoTelefono  = model.DirettoreDipartimentoTelefono;
            existing.DirettoreStruttura             = model.DirettoreStruttura;
            existing.DirettoreStrutturaEmail        = model.DirettoreStrutturaEmail;
            existing.DirettoreStrutturaEmailPec     = model.DirettoreStrutturaEmailPec;
            existing.DirettoreStrutturaTelefono     = model.DirettoreStrutturaTelefono;
            existing.Caposala                       = model.Caposala;
            existing.PrepostoEmail                  = model.PrepostoEmail;
            existing.PrepostoEmailPec               = model.PrepostoEmailPec;
            existing.PrepostoTelefono               = model.PrepostoTelefono;
            existing.RirNome                        = model.RirNome;
            existing.RirEmail                       = model.RirEmail;
            existing.RirEmailPec                    = model.RirEmailPec;
            existing.RirTelefono                    = model.RirTelefono;
            existing.SfmNome                        = model.SfmNome;
            existing.SfmEmail                       = model.SfmEmail;
            existing.SfmEmailPec                    = model.SfmEmailPec;
            existing.SfmTelefono                    = model.SfmTelefono;
            existing.EdrNome                        = model.EdrNome;
            existing.EdrEmail                       = model.EdrEmail;
            existing.EdrEmailPec                    = model.EdrEmailPec;
            existing.EdrTelefono                    = model.EdrTelefono;
            // ── Zone
            existing.DescrizioneZoneClassificate    = model.DescrizioneZoneClassificate;
            // ── Assistenza
            existing.SocietaManutenzione = model.SocietaManutenzione;
            existing.TecnicoRiferimento  = model.TecnicoRiferimento;
            existing.NumeroAssistenza    = model.NumeroAssistenza;
            existing.GlobalService       = model.GlobalService;
            existing.EmailAssistenza     = model.EmailAssistenza;
            // ── Ciclo di vita
            existing.Stato               = model.Stato;
            existing.DataAccettazione    = model.DataAccettazione;
            existing.DataCessazione      = model.DataCessazione;
            existing.MotivoCessazione    = model.MotivoCessazione;
            // ── INAIL
            existing.StatoInail              = model.StatoInail;
            existing.DataRegistrazioneInail  = model.DataRegistrazioneInail;
            existing.NumeroPraticaInail      = model.NumeroPraticaInail;
            // ── STRIMS
            existing.StatoStrims             = model.StatoStrims;
            existing.StrimsIdApparecchiatura = model.StrimsIdApparecchiatura;
            existing.DataRegistrazioneStrims = model.DataRegistrazioneStrims;
            existing.StrimsNpCaricata        = model.StrimsNpCaricata;
            existing.StrimsNcCaricata        = model.StrimsNcCaricata;
            existing.UpdatedAt               = DateTime.UtcNow;
            // ── Upload file (sovrascrive solo se nuovo file caricato)
            var piantina = await SalvaFile(filePiantina, "piantina");
            if (piantina != null)
            {
                EliminaFileFisico(existing.PiantinaZoneClassificateFile);
                existing.PiantinaZoneClassificateFile = piantina;
                existing.PiantinaZoneClassificateNomeOriginale = filePiantina!.FileName;
            }
            var inailReg = await SalvaFile(fileInailReg, "inail");
            if (inailReg != null)
            {
                EliminaFileFisico(existing.InailRicevutaRegistrazioneFile);
                existing.InailRicevutaRegistrazioneFile = inailReg;
                existing.InailRicevutaRegistrazioneNomeOriginale = fileInailReg!.FileName;
            }
            var inailCess = await SalvaFile(fileInailCess, "inail");
            if (inailCess != null)
            {
                EliminaFileFisico(existing.InailRicevutaCessioneFile);
                existing.InailRicevutaCessioneFile = inailCess;
                existing.InailRicevutaCessioneNomeOriginale = fileInailCess!.FileName;
            }
            var strimsReg = await SalvaFile(fileStrimsReg, "strims");
            if (strimsReg != null)
            {
                EliminaFileFisico(existing.StrimsRicevutaRegistrazioneFile);
                existing.StrimsRicevutaRegistrazioneFile = strimsReg;
                existing.StrimsRicevutaRegistrazioneNomeOriginale = fileStrimsReg!.FileName;
            }
            var strimsCess = await SalvaFile(fileStrimsCess, "strims");
            if (strimsCess != null)
            {
                EliminaFileFisico(existing.StrimsRicevutaCessioneFile);
                existing.StrimsRicevutaCessioneFile = strimsCess;
                existing.StrimsRicevutaCessioneNomeOriginale = fileStrimsCess!.FileName;
            }
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
            app.Stato          = StatoApparecchiatura.Cessata;
            app.DataCessazione = DateTime.Today;
            app.UpdatedAt      = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Apparecchiatura «{app.Descrizione}» impostata come Cessata.";
            return RedirectToAction(nameof(Index));
        }
        // ─── ELIMINA FILE ALLEGATO ───────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> EliminaFile(int id, string campo)
        {
            var app = await _db.Apparecchiature.FindAsync(id);
            if (app == null) return NotFound();
            // Mappa campo → percorso file
            string? percorsoFile = campo switch
            {
                "PiantinaZoneClassificateFile"    => app.PiantinaZoneClassificateFile,
                "StrimsRicevutaRegistrazioneFile" => app.StrimsRicevutaRegistrazioneFile,
                "StrimsRicevutaCessioneFile"      => app.StrimsRicevutaCessioneFile,
                "InailRicevutaRegistrazioneFile"  => app.InailRicevutaRegistrazioneFile,
                "InailRicevutaCessioneFile"        => app.InailRicevutaCessioneFile,
                _ => null
            };
            if (percorsoFile == null)
            {
                TempData["Error"] = "File non trovato.";
                return RedirectToAction(nameof(Edit), new { id });
            }
            // Elimina fisicamente il file
            EliminaFileFisico(percorsoFile);
            // Azzera il campo nel DB (path + nome originale)
            switch (campo)
            {
                case "PiantinaZoneClassificateFile":
                    app.PiantinaZoneClassificateFile = null;
                    app.PiantinaZoneClassificateNomeOriginale = null;
                    break;
                case "StrimsRicevutaRegistrazioneFile":
                    app.StrimsRicevutaRegistrazioneFile = null;
                    app.StrimsRicevutaRegistrazioneNomeOriginale = null;
                    break;
                case "StrimsRicevutaCessioneFile":
                    app.StrimsRicevutaCessioneFile = null;
                    app.StrimsRicevutaCessioneNomeOriginale = null;
                    break;
                case "InailRicevutaRegistrazioneFile":
                    app.InailRicevutaRegistrazioneFile = null;
                    app.InailRicevutaRegistrazioneNomeOriginale = null;
                    break;
                case "InailRicevutaCessioneFile":
                    app.InailRicevutaCessioneFile = null;
                    app.InailRicevutaCessioneNomeOriginale = null;
                    break;
            }
            app.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = "File eliminato con successo.";
            return RedirectToAction(nameof(Edit), new { id });
        }
        // ─── STAMPA ──────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Stampa(int id)
        {
            var app = await _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Dipartimento)
                .Include(a => a.FigureResponsabili)
                .Include(a => a.Locale)
                  .ThenInclude(l => l.Piano)
                  .ThenInclude(p => p.Immobile)
                  .ThenInclude(i => i.Sito)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (app == null) return NotFound();
            ViewData["Title"] = $"Stampa — {app.Descrizione}";
            return View(app);
        }
        // ─── API AJAX — Tipologie per ambito ─────────────────────────────
        [HttpGet]
        public JsonResult GetTipologiePerAmbito(string ambito)
        {
            return Json(GetTipologieList(ambito));
        }
        private static string[] GetTipologieList(string ambito) => ambito switch
        {
            "Radiologia" => new[]
            {
                "Tavoli radiografici tradizionali", "Apparecchi radiologici portatili",
                "Tomografia Computerizzata (TAC)", "Mammografi", "Endorale",
                "Ortopantomografi", "CBCT dentale", "CBCT",
                "MOC (Mineralometria Ossea Computerizzata)"
            },
            "RadiologiaInterventistica" => new[]
            {
                "Angiografi fissi (o Sale Ibride)", "Archi a C mobili"
            },
            "MedicinaNucleare" => new[]
            {
                "Gamma Camera", "Tomografi PET", "Sistemi ibridi (PET-TC / SPECT-TC)"
            },
            "Radioterapia" => new[]
            {
                "Acceleratori Lineari (LINAC)", "Sistemi per Brachiterapia",
                "Sistemi per Adroterapia", "Sistemi di Radiochirurgia Stereotassica"
            },
            "RM" => new[] { "RM 1.5T", "RM 3T", "RM 7T", "RM Aperto", "Altro" },
            _ => new[] { "Altro" }
        };
        // ─── API AJAX — Immobili / Piani / Locali ────────────────────────
        [HttpGet]
        public async Task<JsonResult> GetImmobiliPerSito(int sitoId)
        {
            var r = await _db.Immobili.Where(i => i.SitoId == sitoId)
                .Select(i => new { i.Id, i.Nome }).ToListAsync();
            return Json(r);
        }
        [HttpGet]
        public async Task<JsonResult> GetPianiPerImmobile(int immobileId)
        {
            var r = await _db.Piani.Where(p => p.ImmobileId == immobileId)
                .Select(p => new { p.Id, p.Nome }).ToListAsync();
            return Json(r);
        }
        [HttpGet]
        public async Task<JsonResult> GetLocaliPerPiano(int pianoId)
        {
            var r = await _db.Locali.Where(l => l.PianoId == pianoId)
                .Select(l => new { l.Id, l.Nome }).ToListAsync();
            return Json(r);
        }
        [HttpGet]
        public async Task<JsonResult> GetModelliPerCostruttore(int costruttoreId)
        {
            var r = await _db.ModelliApparecchiatura
                .Where(m => m.CostrutoreId == costruttoreId && m.Attivo)
                .OrderBy(m => m.Nome)
                .Select(m => new { m.Id, m.Nome, m.Tipologia })
                .ToListAsync();
            return Json(r);
        }
        // ─── FIGURE RESPONSABILI ─────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> AggiungiFigura(FiguraResponsabile model)
        {
            ModelState.Remove("Apparecchiatura");
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dati non validi.";
                return RedirectToAction(nameof(Detail), new { id = model.ApparecchiaturaId, tab = "anagrafica" });
            }
            model.Id = 0;
            model.ValidoDal = model.ValidoDal == default ? DateTime.Today : model.ValidoDal;
            _db.FigureResponsabili.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Figura {model.Ruolo} — {model.Nome} {model.Cognome} aggiunta.";
            return RedirectToAction(nameof(Detail), new { id = model.ApparecchiaturaId, tab = "anagrafica" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> EliminaFigura(int id, int apparecchiaturaId)
        {
            var figura = await _db.FigureResponsabili.FindAsync(id);
            if (figura != null)
            {
                _db.FigureResponsabili.Remove(figura);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Figura rimossa.";
            }
            return RedirectToAction(nameof(Detail), new { id = apparecchiaturaId, tab = "anagrafica" });
        }
        // ─── EXPORT CSV RM ───────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ExportCsvRM(string? stato)
        {
            var query = _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Locale)
                .Include(a => a.RecordVerifiche)
                .Where(a => a.Modulo == TipoModulo.RM)
                .AsQueryable();
            if (!string.IsNullOrEmpty(stato) && Enum.TryParse<StatoApparecchiatura>(stato, out var s))
                query = query.Where(a => a.Stato == s);
            else
                query = query.Where(a => a.Stato != StatoApparecchiatura.Cessata);
            var items = await query.OrderBy(a => a.Descrizione).ToListAsync();
            var csv   = new System.Text.StringBuilder();
            csv.AppendLine("Codice;Descrizione;Tipologia;Costruttore;Modello;Matricola;" +
                           "Struttura;Stato;DataAccettazione;StatoSTRIMS;StatoINAIL;ProssimoControllo");
            foreach (var a in items)
            {
                var prossima = a.RecordVerifiche
                    .Where(v => v.ProssimaVerificaData.HasValue)
                    .OrderBy(v => v.ProssimaVerificaData)
                    .FirstOrDefault()?.ProssimaVerificaData;
                csv.AppendLine(string.Join(";",
                    Q2(a.Codice), Q2(a.Descrizione),
                    Q2(a.Tipologia), Q2(a.Costruttore), Q2(a.Modello), Q2(a.Matricola),
                    Q2(a.Reparto?.Nome ?? ""), Q2(a.Stato.ToString()),
                    Q2(a.DataAccettazione?.ToString("dd/MM/yyyy") ?? ""),
                    Q2(a.StatoStrims.ToString()), Q2(a.StatoInail.ToString()),
                    Q2(prossima?.ToString("dd/MM/yyyy") ?? "")));
            }
            var bytes    = System.Text.Encoding.UTF8.GetPreamble()
                .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv", $"apparecchiature_rm_{DateTime.Today:yyyyMMdd}.csv");
            static string Q2(string s) => $"\"{s.Replace("\"", "\"\"")}\"";
        }
        // ─── EXPORT CSV ──────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ExportCsv(string? stato, string? ambito)
        {
            var query = _db.Apparecchiature
                .Include(a => a.Reparto)
                .Include(a => a.Locale)
                .Include(a => a.RecordVerifiche)
                .Where(a => a.Modulo == TipoModulo.Radiologica)
                .AsQueryable();
            if (!string.IsNullOrEmpty(stato) && Enum.TryParse<StatoApparecchiatura>(stato, out var s))
                query = query.Where(a => a.Stato == s);
            else
                query = query.Where(a => a.Stato != StatoApparecchiatura.Cessata);
            if (!string.IsNullOrEmpty(ambito) && Enum.TryParse<AmbitoIntervento>(ambito, out var am))
                query = query.Where(a => a.AmbitoIntervento == am);
            var items = await query.OrderBy(a => a.Descrizione).ToListAsync();
            var csv   = new System.Text.StringBuilder();
            csv.AppendLine("Codice;Descrizione;Ambito;Tipologia;Costruttore;Modello;Matricola;" +
                           "Struttura;Stato;DataAccettazione;StatoSTRIMS;StatoINAIL;LAN;MonitoraggioSwDose;ProssimoControllo");
            foreach (var a in items)
            {
                var prossima = a.RecordVerifiche
                    .Where(v => v.ProssimaVerificaData.HasValue)
                    .OrderBy(v => v.ProssimaVerificaData)
                    .FirstOrDefault()?.ProssimaVerificaData;
                csv.AppendLine(string.Join(";",
                    Q(a.Codice), Q(a.Descrizione),
                    Q(a.AmbitoIntervento?.ToString() ?? ""),
                    Q(a.Tipologia), Q(a.Costruttore), Q(a.Modello), Q(a.Matricola),
                    Q(a.Reparto?.Nome ?? ""), Q(a.Stato.ToString()),
                    Q(a.DataAccettazione?.ToString("dd/MM/yyyy") ?? ""),
                    Q(a.StatoStrims.ToString()), Q(a.StatoInail.ToString()),
                    Q(a.LanCollegata ? "Sì" : "No"),
                    Q(a.MedsquareInstallato ? "Sì" : "No"),
                    Q(prossima?.ToString("dd/MM/yyyy") ?? "")));
            }
            var bytes    = System.Text.Encoding.UTF8.GetPreamble()
                .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            var fileName = $"apparecchiature_{DateTime.Today:yyyyMMdd}.csv";
            return File(bytes, "text/csv", fileName);
            static string Q(string s) => $"\"{s.Replace("\"", "\"\"")}\"";
        }
        // ─── HELPER — Upload file ─────────────────────────────────────────
        private async Task<string?> SalvaFile(IFormFile? file, string sottocartella)
        {
            if (file == null || file.Length == 0) return null;
            var dir = Path.Combine(_env.WebRootPath, "uploads", sottocartella);
            Directory.CreateDirectory(dir);
            var nomeFile = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var percorso = Path.Combine(dir, nomeFile);
            await using var stream = new FileStream(percorso, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/{sottocartella}/{nomeFile}";
        }
        // ─── HELPER — elimina file fisico dal disco ────────────────────────
        // percorsoFile e' il path relativo salvato in DB, es: "/uploads/piantina/xxx_file.pdf"
        private void EliminaFileFisico(string? percorsoFile)
        {
            if (string.IsNullOrWhiteSpace(percorsoFile)) return;
            try
            {
                var fullPath = Path.Combine(
                    _env.WebRootPath,
                    percorsoFile.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
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
                    "Impossibile eliminare il file fisico {Path}", percorsoFile);
            }
        }
        // ─── HELPER — PopolateViewBag ─────────────────────────────────────
        private async Task PopolateViewBag()
        {
            ViewBag.Siti = new SelectList(
                await _db.Siti.OrderBy(s => s.Nome).ToListAsync(), "Id", "Nome");
            ViewBag.Reparti = new SelectList(
                await _db.Reparti.OrderBy(r => r.Nome).ToListAsync(), "Id", "Nome");
            ViewBag.Dipartimenti = new SelectList(
                await _db.Dipartimenti.Where(d => d.Attivo).OrderBy(d => d.Nome).ToListAsync(),
                "Id", "Nome");
            ViewBag.Protocolli = await _db.ProtocolliVerifica
                .Where(p => p.Attivo).OrderBy(p => p.Codice).ToListAsync();
            ViewBag.Costruttori = await _db.Costruttori
                .Where(c => c.Attivo).OrderBy(c => c.Nome).ToListAsync();
            ViewBag.SocietaManutenzione = await _db.SocietaManutenzione
                .Where(s => s.Attivo).OrderBy(s => s.Nome).ToListAsync();
        }
    }
}