using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Helpers;
using RadiologiaAppNew.Models;
using RadiologiaAppNew.ViewModels.Dashboard;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ApplicationDbContext db,
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _db          = db;
            _logger      = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Dashboard";

            // ── MODULI ATTIVI PER L'UTENTE CORRENTE ──────────────────────
            var user  = await _userManager.GetUserAsync(User);
            var ruoli = user != null
                ? (IList<string>)await _userManager.GetRolesAsync(user)
                : new List<string>();

            var moduliAttivi = ModuliHelper.GetModuliEffettivi(
                user?.ModuliAbilitati, ruoli);

            // Passiamo i moduli attivi alla view e alla sidebar
            ViewData["ModuliAttivi"] = moduliAttivi;
            ViewData["NomiModuli"]   = ModuliHelper.NomiModuli;

            var vm = new DashboardViewModel();

            try
            {
                var oggi    = DateTime.Today;
                var tra30gg = oggi.AddDays(30);

                // ── KPI MODULO 1 — Apparecchiature ───────────────────────
                if (moduliAttivi.Contains(ModuliHelper.MOD1))
                {
                    vm.TotaleApparecchiatureAttive = await _db.Apparecchiature
                        .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva
                                      && a.Modulo == TipoModulo.Radiologica);

                    vm.ApparecchiatureInManutenzione = await _db.Apparecchiature
                        .CountAsync(a => a.Stato == StatoApparecchiatura.InManutenzione
                                      && a.Modulo == TipoModulo.Radiologica);

                    vm.VerificheCQScadute = await _db.RecordVerifiche
                        .Include(v => v.Apparecchiatura)
                        .CountAsync(v => v.ProssimaVerificaData != null
                                      && v.ProssimaVerificaData < oggi
                                      && v.Apparecchiatura.Modulo == TipoModulo.Radiologica);

                    vm.VerificheCQInScadenza30gg = await _db.RecordVerifiche
                        .Include(v => v.Apparecchiatura)
                        .CountAsync(v => v.ProssimaVerificaData != null
                                      && v.ProssimaVerificaData >= oggi
                                      && v.ProssimaVerificaData <= tra30gg
                                      && v.Apparecchiatura.Modulo == TipoModulo.Radiologica);

                    vm.NullaOstaInScadenza = await _db.NullaOsta
                        .CountAsync(n => n.DataScadenza != null
                                      && n.DataScadenza <= tra30gg
                                      && n.Stato == StatoNullaOsta.Valido);
                }

                // ── KPI MODULO 2 — RM ────────────────────────────────────
                if (moduliAttivi.Contains(ModuliHelper.MOD2))
                {
                    vm.TotaleRMAttive = await _db.Apparecchiature
                        .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva
                                      && a.Modulo == TipoModulo.RM);

                    vm.VerificheCQScaduteRM = await _db.RecordVerifiche
                        .Include(v => v.Apparecchiatura)
                        .CountAsync(v => v.ProssimaVerificaData != null
                                      && v.ProssimaVerificaData < oggi
                                      && v.Apparecchiatura.Modulo == TipoModulo.RM);
                }

                // ── KPI MODULO 4 — Lu177 ─────────────────────────────────
                if (moduliAttivi.Contains(ModuliHelper.MOD4))
                {
                    vm.PazientiLu177Attivi = await _db.PazientiLu177
                        .CountAsync(p => p.StatoPaziente == StatoPaziente.InTrattamento);
                }

                // ── ALERT CRITICI TOTALI ──────────────────────────────────
                vm.AlertCriticiTotali = vm.VerificheCQScadute
                                      + vm.VerificheCQScaduteRM
                                      + vm.NullaOstaInScadenza;

                // ── COMPLIANCE % (solo MOD1) ──────────────────────────────
                if (moduliAttivi.Contains(ModuliHelper.MOD1))
                {
                    var totApp = await _db.Apparecchiature
                        .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva
                                      && a.Modulo == TipoModulo.Radiologica);
                    if (totApp > 0)
                    {
                        var appInRegola = await _db.Apparecchiature
                            .CountAsync(a => a.Stato == StatoApparecchiatura.Attiva
                                          && a.Modulo == TipoModulo.Radiologica
                                          && a.StatoInail == StatoAdempimento.Registrato
                                          && a.StatoStrims == StatoAdempimento.Registrato);
                        vm.PercentualeCompliance =
                            Math.Round((double)appInRegola / totApp * 100, 1);
                    }
                }

                // ── SCADENZE IMMINENTI (filtrate per moduli attivi) ───────
                var verificheQuery = _db.RecordVerifiche
                    .Include(v => v.Apparecchiatura)
                    .Where(v => v.ProssimaVerificaData != null
                             && v.ProssimaVerificaData <= tra30gg
                             && v.ProssimaVerificaData >= oggi.AddDays(-7));

                // Filtra per moduli attivi
                if (moduliAttivi.Contains(ModuliHelper.MOD1) &&
                    moduliAttivi.Contains(ModuliHelper.MOD2))
                {
                    // mostra tutto
                }
                else if (moduliAttivi.Contains(ModuliHelper.MOD1))
                {
                    verificheQuery = verificheQuery.Where(
                        v => v.Apparecchiatura.Modulo == TipoModulo.Radiologica);
                }
                else if (moduliAttivi.Contains(ModuliHelper.MOD2))
                {
                    verificheQuery = verificheQuery.Where(
                        v => v.Apparecchiatura.Modulo == TipoModulo.RM);
                }
                else
                {
                    verificheQuery = verificheQuery.Where(v => false);
                }

                var verificheInScadenza = await verificheQuery
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

                // ── ULTIME VERIFICHE (filtrate per moduli) ────────────────
                var ultimeQuery = _db.RecordVerifiche
                    .Include(v => v.Apparecchiatura)
                    .OrderByDescending(v => v.DataInizio)
                    .AsQueryable();

                if (!moduliAttivi.Contains(ModuliHelper.MOD1))
                    ultimeQuery = ultimeQuery.Where(
                        v => v.Apparecchiatura.Modulo != TipoModulo.Radiologica);
                if (!moduliAttivi.Contains(ModuliHelper.MOD2))
                    ultimeQuery = ultimeQuery.Where(
                        v => v.Apparecchiatura.Modulo != TipoModulo.RM);

                vm.UltimeVerifiche = await ultimeQuery
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

                // ── GRAFICI — CQ per mese (filtrati per moduli) ───────────
                var inizioAnno    = new DateTime(oggi.Year, 1, 1);
                var verificheAnno = await _db.RecordVerifiche
                    .Include(v => v.Apparecchiatura)
                    .Where(v => v.DataInizio >= inizioAnno)
                    .ToListAsync();

                // Filtra per moduli attivi
                verificheAnno = verificheAnno
                    .Where(v =>
                        (moduliAttivi.Contains(ModuliHelper.MOD1) &&
                         v.Apparecchiatura.Modulo == TipoModulo.Radiologica) ||
                        (moduliAttivi.Contains(ModuliHelper.MOD2) &&
                         v.Apparecchiatura.Modulo == TipoModulo.RM))
                    .ToList();

                for (int m = 1; m <= oggi.Month; m++)
                {
                    var mese = new DateTime(oggi.Year, m, 1);
                    vm.MesiLabels.Add(mese.ToString("MMM"));
                    vm.CQSuperati.Add(verificheAnno.Count(
                        v => v.DataInizio.Month == m &&
                             v.Esito == EsitoVerifica.Superato));
                    vm.CQNonSuperati.Add(verificheAnno.Count(
                        v => v.DataInizio.Month == m &&
                             v.Esito == EsitoVerifica.NonSuperato));
                }

                // ── GRAFICI — Distribuzione ambiti (solo MOD1 + MOD2) ────
                if (moduliAttivi.Contains(ModuliHelper.MOD1) ||
                    moduliAttivi.Contains(ModuliHelper.MOD2))
                {
                    var apparecchiature = await _db.Apparecchiature
                        .Where(a => a.Stato == StatoApparecchiatura.Attiva)
                        .ToListAsync();

                    if (moduliAttivi.Contains(ModuliHelper.MOD1))
                    {
                        vm.CountRadiologia = apparecchiature.Count(
                            a => a.AmbitoIntervento == AmbitoIntervento.Radiologia);
                        vm.CountInterventistica = apparecchiature.Count(
                            a => a.AmbitoIntervento == AmbitoIntervento.RadiologiaInterventistica);
                        vm.CountMedicinaNucleare = apparecchiature.Count(
                            a => a.AmbitoIntervento == AmbitoIntervento.MedicinaNucleare);
                        vm.CountRadioterapia = apparecchiature.Count(
                            a => a.AmbitoIntervento == AmbitoIntervento.Radioterapia);
                    }

                    if (moduliAttivi.Contains(ModuliHelper.MOD2))
                        vm.CountRM = apparecchiature.Count(
                            a => a.Modulo == TipoModulo.RM);

                    var totGrafico = vm.CountRadiologia + vm.CountInterventistica +
                                    vm.CountMedicinaNucleare + vm.CountRadioterapia +
                                    vm.CountRM;
                    ViewData["MostraGraficoAmbiti"] = totGrafico > 0;
                }

                ViewData["AlertCount"] = vm.AlertCriticiTotali;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore caricamento dashboard.");
            }

            return View(vm);
        }

        // ── SCADENZE ─────────────────────────────────────────────────────
        public async Task<IActionResult> Scadenze(
            string? tipo,
            int? giorni,
            int pagina = 1)
        {
            ViewData["Title"] = "Scadenze e Alert";
            ViewData["BreadcrumbParent"] = "Dashboard";
            ViewData["BreadcrumbParentUrl"] = "/";

            // Filtro moduli anche per la pagina scadenze
            var user  = await _userManager.GetUserAsync(User);
            var ruoli = user != null
                ? (IList<string>)await _userManager.GetRolesAsync(user)
                : new List<string>();
            var moduliAttivi = ModuliHelper.GetModuliEffettivi(
                user?.ModuliAbilitati, ruoli);
            ViewData["ModuliAttivi"] = moduliAttivi;

            const int perPagina = 30;
            var oggi   = DateTime.Today;
            var limite = oggi.AddDays(giorni ?? 60);

            var query = _db.RecordVerifiche
                .Include(v => v.Apparecchiatura)
                .Include(v => v.Protocollo)
                .Where(v => v.ProssimaVerificaData.HasValue &&
                            v.ProssimaVerificaData <= limite)
                .AsQueryable();

            // Filtra per moduli attivi
            if (!moduliAttivi.Contains(ModuliHelper.MOD1))
                query = query.Where(v => v.Apparecchiatura.Modulo != TipoModulo.Radiologica);
            if (!moduliAttivi.Contains(ModuliHelper.MOD2))
                query = query.Where(v => v.Apparecchiatura.Modulo != TipoModulo.RM);

            if (tipo == "scadute")
                query = query.Where(v => v.ProssimaVerificaData < oggi);

            var totale = await query.CountAsync();
            var items  = await query
                .OrderBy(v => v.ProssimaVerificaData)
                .Skip((pagina - 1) * perPagina)
                .Take(perPagina)
                .ToListAsync();

            var noScadenza = new List<NullaOsta>();
            if (moduliAttivi.Contains(ModuliHelper.MOD1))
            {
                noScadenza = await _db.NullaOsta
                    .Include(n => n.Apparecchiatura)
                    .Where(n => n.DataScadenza.HasValue &&
                                n.DataScadenza <= limite &&
                                n.Stato == StatoNullaOsta.Valido)
                    .OrderBy(n => n.DataScadenza)
                    .ToListAsync();
            }

            ViewData["Oggi"]             = oggi;
            ViewData["Limite"]           = limite;
            ViewData["GiorniFiltro"]     = giorni ?? 60;
            ViewData["TipoFiltro"]       = tipo;
            ViewData["PaginaCorrente"]   = pagina;
            ViewData["TotalePagine"]     = (int)Math.Ceiling((double)totale / perPagina);
            ViewData["TotaleRisultati"]  = totale;
            ViewData["NoScadenza"]       = noScadenza;
            ViewData["AlertCount"]       = items.Count(v => v.ProssimaVerificaData < oggi)
                                         + noScadenza.Count;

            return View(items);
        }
    }
}