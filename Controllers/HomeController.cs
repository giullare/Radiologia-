using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.ViewModels.Dashboard;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            var vm = new DashboardViewModel();

            try
            {
                // ── KPI ──────────────────────────────────────────────────
                vm.TotaleApparecchiatureAttive = await _db.Apparecchiature
                    .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva);

                vm.ApparecchiatureInManutenzione = await _db.Apparecchiature
                    .CountAsync(a => a.Stato == StatoApparecchiatura.InManutenzione);

                vm.PazientiLu177Attivi = await _db.PazientiLu177
                    .CountAsync(p => p.StatoPaziente == StatoPaziente.InTrattamento);

                // Verifiche scadute (prossima verifica < oggi)
                var oggi = DateTime.Today;
                var tra30gg = oggi.AddDays(30);

                vm.VerificheCQScadute = await _db.RecordVerifiche
                    .CountAsync(v => v.ProssimaVerificaData != null
                                 && v.ProssimaVerificaData < oggi);

                vm.VerificheCQInScadenza30gg = await _db.RecordVerifiche
                    .CountAsync(v => v.ProssimaVerificaData != null
                                 && v.ProssimaVerificaData >= oggi
                                 && v.ProssimaVerificaData <= tra30gg);

                vm.NullaOstaInScadenza = await _db.NullaOsta
                    .CountAsync(n => n.DataScadenza != null
                                 && n.DataScadenza <= tra30gg
                                 && n.Stato == StatoNullaOsta.Valido);

                vm.AlertCriticiTotali = vm.VerificheCQScadute + vm.NullaOstaInScadenza;

                // Compliance %
                var totApp = await _db.Apparecchiature
                    .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva);
                if (totApp > 0)
                {
                    var appInRegola = await _db.Apparecchiature
                        .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva
                                      && a.StatoInail == StatoAdempimento.Registrato
                                      && a.StatoStrims == StatoAdempimento.Registrato);
                    vm.PercentualeCompliance = Math.Round((double)appInRegola / totApp * 100, 1);
                }

                // ── SCADENZE IMMINENTI ────────────────────────────────────
                var verificheInScadenza = await _db.RecordVerifiche
                    .Include(v => v.Apparecchiatura)
                    .Where(v => v.ProssimaVerificaData != null
                             && v.ProssimaVerificaData <= tra30gg
                             && v.ProssimaVerificaData >= oggi.AddDays(-7))
                    .OrderBy(v => v.ProssimaVerificaData)
                    .Take(10)
                    .ToListAsync();

                vm.ScadenzeImminenti = verificheInScadenza.Select(v => new ScadenzaItem
                {
                    Apparecchiatura   = v.Apparecchiatura.Descrizione,
                    Tipo              = v.Tipo.ToString(),
                    DataScadenza      = v.ProssimaVerificaData!.Value,
                    ApparecchiaturaId = v.ApparecchiaturaId,
                    Priorita          = v.ProssimaVerificaData < oggi ? "danger"
                                      : v.ProssimaVerificaData <= oggi.AddDays(7) ? "danger"
                                      : "warning"
                }).ToList();

                // ── ULTIME VERIFICHE ──────────────────────────────────────
                vm.UltimeVerifiche = await _db.RecordVerifiche
                    .Include(v => v.Apparecchiatura)
                    .OrderByDescending(v => v.DataInizio)
                    .Take(8)
                    .Select(v => new UltimaVerificaItem
                    {
                        Apparecchiatura   = v.Apparecchiatura.Descrizione,
                        TipoVerifica      = v.Tipo.ToString(),
                        Data              = v.DataInizio,
                        Esito             = v.Esito.ToString(),
                        ApparecchiaturaId = v.ApparecchiaturaId
                    })
                    .ToListAsync();

                // ── DATI GRAFICI — CQ per mese ────────────────────────────
                var inizioAnno = new DateTime(oggi.Year, 1, 1);
                var verificheAnno = await _db.RecordVerifiche
                    .Where(v => v.DataInizio >= inizioAnno)
                    .ToListAsync();

                for (int m = 1; m <= oggi.Month; m++)
                {
                    var mese = new DateTime(oggi.Year, m, 1);
                    vm.MesiLabels.Add(mese.ToString("MMM"));

                    vm.CQSuperati.Add(verificheAnno.Count(v =>
                        v.DataInizio.Month == m &&
                        v.Esito == EsitoVerifica.Superato));

                    vm.CQNonSuperati.Add(verificheAnno.Count(v =>
                        v.DataInizio.Month == m &&
                        v.Esito == EsitoVerifica.NonSuperato));
                }

                // ── DATI GRAFICI — Distribuzione ambiti ──────────────────
                var apparecchiature = await _db.Apparecchiature
                    .Where(a => a.Stato == StatoApparecchiatura.Attiva)
                    .ToListAsync();

                vm.CountRadiologia       = apparecchiature.Count(a =>
                    a.AmbitoIntervento == AmbitoIntervento.Radiologia);
                vm.CountInterventistica  = apparecchiature.Count(a =>
                    a.AmbitoIntervento == AmbitoIntervento.RadiologiaInterventistica);
                vm.CountMedicinaNucleare = apparecchiature.Count(a =>
                    a.AmbitoIntervento == AmbitoIntervento.MedicinaNucleare);
                vm.CountRadioterapia     = apparecchiature.Count(a =>
                    a.AmbitoIntervento == AmbitoIntervento.Radioterapia);
                vm.CountRM               = apparecchiature.Count(a =>
                    a.Modulo == TipoModulo.RM);
// Mostra grafico se c'è almeno una apparecchiatura
// anche se i contatori per ambito sono 0
var totGrafico = vm.CountRadiologia + vm.CountInterventistica +
                 vm.CountMedicinaNucleare + vm.CountRadioterapia +
                 vm.CountRM;
ViewData["MostraGraficoAmbiti"] = totGrafico > 0 ||
                                   vm.TotaleApparecchiatureAttive > 0;
                // Aggiorna conteggio alert in header
                ViewData["AlertCount"] = vm.AlertCriticiTotali;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore caricamento dashboard.");
            }

            return View(vm);
        }

       public async Task<IActionResult> Scadenze(
    string? tipo,
    int? giorni,
    int pagina = 1)
{
    ViewData["Title"] = "Scadenze e Alert";
    ViewData["BreadcrumbParent"] = "Dashboard";
    ViewData["BreadcrumbParentUrl"] = "/";

    const int perPagina = 30;
    var oggi   = DateTime.Today;
    var limite = oggi.AddDays(giorni ?? 60);

    var query = _db.RecordVerifiche
        .Include(v => v.Apparecchiatura)
        .Include(v => v.Protocollo)
        .Where(v => v.ProssimaVerificaData.HasValue &&
                    v.ProssimaVerificaData <= limite)
        .AsQueryable();

    if (!string.IsNullOrEmpty(tipo) &&
        Enum.TryParse<EsitoVerifica>(tipo, out _))
    {
        // filtra solo scadute se richiesto
        if (tipo == "scadute")
            query = query.Where(v =>
                v.ProssimaVerificaData < oggi);
    }

    var totale = await query.CountAsync();
    var items  = await query
        .OrderBy(v => v.ProssimaVerificaData)
        .Skip((pagina - 1) * perPagina)
        .Take(perPagina)
        .ToListAsync();

    // Nulla osta in scadenza
    var noScadenza = await _db.NullaOsta
        .Include(n => n.Apparecchiatura)
        .Where(n => n.DataScadenza.HasValue &&
                    n.DataScadenza <= limite &&
                    n.Stato == StatoNullaOsta.Valido)
        .OrderBy(n => n.DataScadenza)
        .ToListAsync();

    ViewData["Oggi"]        = oggi;
    ViewData["Limite"]      = limite;
    ViewData["GiorniFiltro"] = giorni ?? 60;
    ViewData["TipoFiltro"]  = tipo;
    ViewData["PaginaCorrente"]  = pagina;
    ViewData["TotalePagine"]    =
        (int)Math.Ceiling((double)totale / perPagina);
    ViewData["TotaleRisultati"] = totale;
    ViewData["NoScadenza"]      = noScadenza;
    ViewData["AlertCount"]      = items.Count(v =>
        v.ProssimaVerificaData < oggi) + noScadenza.Count;

    return View(items);
}
    }
}