using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;
using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Controllers
{
    [Authorize]
    public class Lu177Controller : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<Lu177Controller> _logger;

        public Lu177Controller(
            ApplicationDbContext db,
            ILogger<Lu177Controller> logger)
        {
            _db = db;
            _logger = logger;
        }

        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index(
            string? stato,
            string? indicazione,
            string? search,
            int pagina = 1)
        {
            ViewData["Title"] = "Pazienti Terapia Lu177";
            ViewData["BreadcrumbParent"] = "Modulo 4";

            const int perPagina = 20;

            var query = _db.PazientiLu177
                .Include(p => p.CicliTrattamento)
                .AsQueryable();

            if (!string.IsNullOrEmpty(stato) &&
                Enum.TryParse<StatoPaziente>(stato, out var statoEnum))
                query = query.Where(p => p.StatoPaziente == statoEnum);

            if (!string.IsNullOrEmpty(indicazione))
                query = query.Where(p => p.Indicazione == indicazione);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    p.Cognome.Contains(search) ||
                    p.Nome.Contains(search) ||
                    p.CodicePaziente.Contains(search));

            var totale = await query.CountAsync();
            var items  = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pagina - 1) * perPagina)
                .Take(perPagina)
                .ToListAsync();

            ViewData["StatoFiltro"]       = stato;
            ViewData["IndicazioneFiltro"] = indicazione;
            ViewData["SearchText"]        = search;
            ViewData["PaginaCorrente"]    = pagina;
            ViewData["TotalePagine"]      =
                (int)Math.Ceiling((double)totale / perPagina);
            ViewData["TotaleRisultati"]   = totale;

            return View(items);
        }

        // ─── DETAIL ──────────────────────────────────────────────────────
        public async Task<IActionResult> Detail(int id)
        {
            var paziente = await _db.PazientiLu177
                .Include(p => p.CicliTrattamento)
                    .ThenInclude(c => c.DatiEmatologici)
                .Include(p => p.DatiEmatologici)
                .Include(p => p.FileAllegati)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paziente == null) return NotFound();

            ViewData["Title"] =
                $"{paziente.Cognome} {paziente.Nome} — Lu177";
            ViewData["BreadcrumbParent"] = "Pazienti Lu177";
            ViewData["BreadcrumbParentUrl"] = "/Lu177";

            return View(paziente);
        }

        // ─── CREATE GET ──────────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuovo Paziente Lu177";
            ViewData["BreadcrumbParent"] = "Pazienti Lu177";
            ViewData["BreadcrumbParentUrl"] = "/Lu177";
            return View(new PazienteLu177
            {
                DataNascita = DateTime.Today.AddYears(-60),
                Indicazione = "PSMA_LU177"
            });
        }

        // ─── CREATE POST ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> Create(PazienteLu177 model)
        {
            ModelState.Remove("CicliTrattamento");
            ModelState.Remove("DatiEmatologici");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
                return View(model);

            if (await _db.PazientiLu177
                    .AnyAsync(p => p.CodicePaziente == model.CodicePaziente))
            {
                ModelState.AddModelError("CodicePaziente",
                    "Codice paziente già esistente.");
                return View(model);
            }

            model.CreatedAt        = DateTime.UtcNow;
            model.CreatedByUserId  = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _db.PazientiLu177.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] =
                $"Paziente {model.Cognome} {model.Nome} registrato.";
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }

        // ─── NUOVO CICLO GET ─────────────────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> NuovoCiclo(int id)
        {
            var paz = await _db.PazientiLu177
                .Include(p => p.CicliTrattamento)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (paz == null) return NotFound();

            ViewData["Title"] = "Nuovo Ciclo Lu177";
            ViewData["BreadcrumbParent"] =
                $"{paz.Cognome} {paz.Nome}";
            ViewData["BreadcrumbParentUrl"] = $"/Lu177/Detail/{id}";

            var numeroCiclo = paz.CicliTrattamento.Count + 1;

            ViewBag.Paziente     = paz;
            ViewBag.NumeroCiclo  = numeroCiclo;
            return View(new CicloTrattamento
            {
                PazienteId            = id,
                NumeroCiclo           = numeroCiclo,
                DataSomministrazione  = DateTime.Today,
                OraSomministrazione   = TimeSpan.FromHours(9),
                PesoPazienteCicloKg   = paz.PesoKg ?? 0,
                EsitoCiclo            = "Completato"
            });
        }

        // ─── NUOVO CICLO POST ────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> NuovoCiclo(CicloTrattamento model)
        {
            ModelState.Remove("Paziente");
            ModelState.Remove("FileAllegati");
            ModelState.Remove("DatiEmatologici");

            if (!ModelState.IsValid)
            {
                var paz2 = await _db.PazientiLu177
                    .FindAsync(model.PazienteId);
                ViewBag.Paziente    = paz2;
                ViewBag.NumeroCiclo = model.NumeroCiclo;
                return View(model);
            }

            model.CreatedAt       = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _db.CicliTrattamento.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] =
                $"Ciclo {model.NumeroCiclo} registrato con successo.";
            return RedirectToAction(nameof(Detail),
                new { id = model.PazienteId });
        }

        // ─── NUOVO DATO EMATOLOGICO GET ──────────────────────────────────
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> NuovoEmatologico(int id)
        {
            var paz = await _db.PazientiLu177
                .Include(p => p.CicliTrattamento)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (paz == null) return NotFound();

            ViewData["Title"] = "Nuovo Dato Ematologico";
            ViewBag.Paziente  = paz;
            return View(new DatoEmatologico
            {
                PazienteId    = id,
                DataPrelievo  = DateTime.Today
            });
        }

        // ─── NUOVO DATO EMATOLOGICO POST ─────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMIN_ORG,EFM,EDR")]
        public async Task<IActionResult> NuovoEmatologico(
            DatoEmatologico model)
        {
            ModelState.Remove("Paziente");
            ModelState.Remove("Ciclo");
            ModelState.Remove("FileAllegati");

            if (!ModelState.IsValid)
            {
                ViewBag.Paziente = await _db.PazientiLu177
                    .FindAsync(model.PazienteId);
                return View(model);
            }

            model.CreatedAt       = DateTime.UtcNow;
            model.CreatedByUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            _db.DatiEmatologici.Add(model);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Dato ematologico registrato.";
            return RedirectToAction(nameof(Detail),
                new { id = model.PazienteId });
        }

        // ─── STATISTICHE ─────────────────────────────────────────────────
        public async Task<IActionResult> Statistiche()
        {
            ViewData["Title"] = "Statistiche Lu177";
            ViewData["BreadcrumbParent"] = "Pazienti Lu177";
            ViewData["BreadcrumbParentUrl"] = "/Lu177";

            var pazienti = await _db.PazientiLu177
                .Include(p => p.CicliTrattamento)
                .Include(p => p.DatiEmatologici)
                .ToListAsync();

            var vm = new Lu177StatisticheVm
            {
                TotalePazienti = pazienti.Count,
                InTrattamento  = pazienti.Count(p =>
                    p.StatoPaziente == StatoPaziente.InTrattamento),
                Conclusi       = pazienti.Count(p =>
                    p.StatoPaziente == StatoPaziente.Concluso),
                CountPsma      = pazienti.Count(p =>
                    p.Indicazione == "PSMA_LU177"),
                CountDotatate  = pazienti.Count(p =>
                    p.Indicazione == "DOTATATE_LU177"),
                TotaleCicli    = pazienti
                    .Sum(p => p.CicliTrattamento.Count),
                MediaCicliPerPaziente = pazienti.Any()
                    ? Math.Round(pazienti
                        .Average(p => p.CicliTrattamento.Count), 1)
                    : 0,
                DistribuzioneTossicita = new Dictionary<string,int>
                {
                    ["G0"] = pazienti.SelectMany(p => p.DatiEmatologici)
                        .Count(d => d.TossicitaEmatologica == "G0"),
                    ["G1"] = pazienti.SelectMany(p => p.DatiEmatologici)
                        .Count(d => d.TossicitaEmatologica == "G1"),
                    ["G2"] = pazienti.SelectMany(p => p.DatiEmatologici)
                        .Count(d => d.TossicitaEmatologica == "G2"),
                    ["G3"] = pazienti.SelectMany(p => p.DatiEmatologici)
                        .Count(d => d.TossicitaEmatologica == "G3"),
                    ["G4"] = pazienti.SelectMany(p => p.DatiEmatologici)
                        .Count(d => d.TossicitaEmatologica == "G4")
                }
            };

            return View(vm);
        }
        // ─── EXPORT CSV ──────────────────────────────────────────────────
        [HttpGet]
public async Task<IActionResult> ExportCsv()
{
    var pazienti = await _db.PazientiLu177
        .Include(p => p.CicliTrattamento)
        .Include(p => p.DatiEmatologici)
        .OrderBy(p => p.Cognome)
        .ToListAsync();

    var csv = new System.Text.StringBuilder();
    csv.AppendLine(
        "CodicePaziente;Cognome;Nome;DataNascita;Sesso;" +
        "Indicazione;Stato;NCicli;UltimoCiclo;" +
        "AttivitaTotaleGBq;MedicoInviante");

    foreach (var p in pazienti)
    {
        var ultimoCiclo = p.CicliTrattamento
            .OrderByDescending(c => c.DataSomministrazione)
            .FirstOrDefault();
        var attivitaTot = p.CicliTrattamento
            .Sum(c => c.AttivitaSomministrataGbq
                      - c.AttivitaResiduaSiringaGbq);

        csv.AppendLine(string.Join(";",
            Q(p.CodicePaziente),
            Q(p.Cognome),
            Q(p.Nome),
            Q(p.DataNascita.ToString("dd/MM/yyyy")),
            Q(p.Sesso),
            Q(p.Indicazione),
            Q(p.StatoPaziente.ToString()),
            Q(p.CicliTrattamento.Count.ToString()),
            Q(ultimoCiclo?.DataSomministrazione
                .ToString("dd/MM/yyyy") ?? ""),
            Q(attivitaTot.ToString("F3")),
            Q(p.MedicoInviante)
        ));
    }

    var bytes    = System.Text.Encoding.UTF8.GetPreamble()
        .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
        .ToArray();
    var fileName =
        $"pazienti_lu177_{DateTime.Today:yyyyMMdd}.csv";

    return File(bytes, "text/csv", fileName);

    static string Q(string s) =>
        $"\"{s.Replace("\"", "\"\"")}\"";
}
    }

    public class Lu177StatisticheVm
    {
        public int TotalePazienti   { get; set; }
        public int InTrattamento    { get; set; }
        public int Conclusi         { get; set; }
        public int CountPsma        { get; set; }
        public int CountDotatate    { get; set; }
        public int TotaleCicli      { get; set; }
        public double MediaCicliPerPaziente { get; set; }
        public Dictionary<string,int> DistribuzioneTossicita { get; set; }
            = new();
    }
}