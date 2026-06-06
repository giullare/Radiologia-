using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Models;
using RadiologiaAppNew.Models.Collocazione;

namespace RadiologiaAppNew.Controllers
{
    [Authorize(Roles = "ADMIN_ORG")]
    public class ConfigurazioneController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ConfigurazioneController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index(string tab = "struttura")
        {
            ViewData["Title"] = "Configurazione";
            ViewData["BreadcrumbParent"] = "Amministrazione";
            ViewData["TabAttivo"] = tab;
            var vm = new ConfigurazioneVm
            {
                Siti = await _db.Siti
                    .Include(s => s.Immobili)
                        .ThenInclude(i => i.Piani)
                            .ThenInclude(p => p.Locali)
                    .OrderBy(s => s.Nome).ToListAsync(),
                Reparti    = await _db.Reparti
                    .OrderBy(r => r.Nome).ToListAsync(),
                Protocolli = await _db.ProtocolliVerifica
                    .OrderBy(p => p.Tipo)
                    .ThenBy(p => p.Codice).ToListAsync(),
                Costruttori = await _db.Costruttori
                    .Include(c => c.Modelli)
                    .OrderBy(c => c.Nome).ToListAsync(),
                SocietaManutenzione = await _db.SocietaManutenzione
                    .OrderBy(s => s.Nome).ToListAsync()
            };

            return View(vm);
        }

        // ═══════════════════════════════════════════════════════════
        // STRUTTURA FISICA
        // ═══════════════════════════════════════════════════════════

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoSito(
            string nome, string? indirizzo , string tab = "struttura")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.Siti.Add(new Sito
                    { Nome = nome, Indirizzo = indirizzo });
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Sito «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoImmobile(
            int sitoId, string nome, string tab = "struttura")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.Immobili.Add(new Immobile
                    { SitoId = sitoId, Nome = nome });
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Edificio «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoPiano(
            int immobileId, string nome, int? numero, string tab = "struttura")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.Piani.Add(new Piano
                {
                    ImmobileId = immobileId,
                    Nome       = nome,
                    Numero     = numero
                });
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Piano «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoLocale(
            int pianoId, string nome, string? codice, string tab = "struttura")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.Locali.Add(new Locale
                {
                    PianoId = pianoId,
                    Nome    = nome,
                    Codice  = codice
                });
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Locale «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        
        

        

    
    // ─── ELIMINA PRESIDIO ────────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaSito(
    int id, string tab = "struttura")
{
    var sito = await _db.Siti
        .Include(s => s.Immobili)
            .ThenInclude(i => i.Piani)
                .ThenInclude(p => p.Locali)
        .FirstOrDefaultAsync(s => s.Id == id);

    if (sito == null)
        return RedirectToAction(nameof(Index), new { tab });

    // Controlla se qualche locale è usato da apparecchiature
    var localiIds = sito.Immobili
        .SelectMany(i => i.Piani)
        .SelectMany(p => p.Locali)
        .Select(l => l.Id)
        .ToList();

    if (localiIds.Any())
    {
        var haApp = await _db.Apparecchiature
            .AnyAsync(a => a.LocaleId.HasValue &&
                           localiIds.Contains(a.LocaleId.Value));
        if (haApp)
        {
            TempData["Error"] =
                $"Impossibile eliminare il presidio «{sito.Nome}»: " +
                "uno o più locali sono associati ad apparecchiature. " +
                "Riassegna prima le apparecchiature ad altro locale.";
            return RedirectToAction(nameof(Index), new { tab });
        }
    }

    // Azzera LocaleId nelle apparecchiature che puntano
    // a locali di questo sito (per sicurezza)
    if (localiIds.Any())
    {
        var appDaAggiornare = await _db.Apparecchiature
            .Where(a => a.LocaleId.HasValue &&
                        localiIds.Contains(a.LocaleId.Value))
            .ToListAsync();
        foreach (var a in appDaAggiornare)
            a.LocaleId = null;
    }

    // Cascade delete gestita dal DB dopo il fix migration
    _db.Siti.Remove(sito);
    await _db.SaveChangesAsync();

    TempData["Success"] =
        $"Presidio «{sito.Nome}» eliminato con tutti gli edifici, " +
        "piani e locali.";
    return RedirectToAction(nameof(Index), new { tab });
}

// ─── ELIMINA IMMOBILE ────────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaImmobile(
    int id, string tab = "struttura")
{
    var imm = await _db.Immobili
        .Include(x => x.Piani)
            .ThenInclude(p => p.Locali)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (imm == null)
        return RedirectToAction(nameof(Index), new { tab });

    var localiIds = imm.Piani
        .SelectMany(p => p.Locali)
        .Select(l => l.Id).ToList();

    if (localiIds.Any())
    {
        var haApp = await _db.Apparecchiature
            .AnyAsync(a => a.LocaleId.HasValue &&
                           localiIds.Contains(a.LocaleId.Value));
        if (haApp)
        {
            TempData["Error"] =
                $"Impossibile eliminare l'edificio «{imm.Nome}»: " +
                "contiene locali associati ad apparecchiature.";
            return RedirectToAction(nameof(Index), new { tab });
        }
    }

    _db.Immobili.Remove(imm);
    await _db.SaveChangesAsync();
    TempData["Success"] = $"Edificio «{imm.Nome}» eliminato.";
    return RedirectToAction(nameof(Index), new { tab });
}

// ─── ELIMINA COSTRUTTORE ─────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaCostruttore(
    int id, string tab = "costruttori")
{
    var c = await _db.Costruttori
        .Include(x => x.Modelli)
        .FirstOrDefaultAsync(x => x.Id == id);

    if (c == null)
        return RedirectToAction(nameof(Index), new { tab });

    // Controlla se il costruttore è usato in apparecchiature
    var haApp = await _db.Apparecchiature
        .AnyAsync(a => a.Costruttore == c.Nome);

    if (haApp)
    {
        TempData["Error"] =
            $"Impossibile eliminare «{c.Nome}»: " +
            "è associato ad una o più apparecchiature. " +
            "Disattivalo invece di eliminarlo.";
        return RedirectToAction(nameof(Index), new { tab });
    }

    _db.Costruttori.Remove(c);
    await _db.SaveChangesAsync();
    TempData["Success"] = $"Costruttore «{c.Nome}» eliminato.";
    return RedirectToAction(nameof(Index), new { tab });
}


// ─── MODIFICA SOCIETÀ ────────────────────────────────────────────
[HttpGet]
public async Task<IActionResult> ModificaSocieta(int id)
{
    var s = await _db.SocietaManutenzione.FindAsync(id);
    if (s == null) return NotFound();

    ViewData["Title"] = $"Modifica — {s.Nome}";
    return View(s);
}

[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> ModificaSocieta(
    SocietaManutenzione model, string tab = "societa")
{
    if (!ModelState.IsValid)
        return View(model);

    var existing = await _db.SocietaManutenzione
        .FindAsync(model.Id);
    if (existing == null) return NotFound();

    existing.Nome               = model.Nome;
    existing.NumeroAssistenza   = model.NumeroAssistenza;
    existing.NumeroReperibilita = model.NumeroReperibilita;
    existing.EmailAssistenza    = model.EmailAssistenza;
    existing.GlobalService      = model.GlobalService;
    existing.SitoWeb            = model.SitoWeb;
    existing.Note               = model.Note;

    await _db.SaveChangesAsync();
    TempData["Success"] = "Società aggiornata.";
    return RedirectToAction(nameof(Index), new { tab = "societa" });
}

// ─── ELIMINA SOCIETÀ ─────────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaSocieta(
    int id, string tab = "societa")
{
    var s = await _db.SocietaManutenzione.FindAsync(id);
    if (s == null)
        return RedirectToAction(nameof(Index), new { tab });

    // Controlla se usata in apparecchiature
    var haApp = await _db.Apparecchiature
        .AnyAsync(a => a.SocietaManutenzione == s.Nome);

    if (haApp)
    {
        TempData["Error"] =
            $"Impossibile eliminare «{s.Nome}»: " +
            "è associata ad una o più apparecchiature.";
        return RedirectToAction(nameof(Index), new { tab });
    }

    _db.SocietaManutenzione.Remove(s);
    await _db.SaveChangesAsync();
    TempData["Success"] = $"Società «{s.Nome}» eliminata.";
    return RedirectToAction(nameof(Index), new { tab });
}

// ─── ELIMINA PIANO ───────────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaPiano(
    int id, string tab = "struttura")
{
    var piano = await _db.Piani
        .Include(p => p.Locali)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (piano == null)
        return RedirectToAction(nameof(Index), new { tab });

    var localiIds = piano.Locali.Select(l => l.Id).ToList();

    if (localiIds.Any())
    {
        var haApp = await _db.Apparecchiature
            .AnyAsync(a => a.LocaleId.HasValue &&
                           localiIds.Contains(a.LocaleId.Value));
        if (haApp)
        {
            TempData["Error"] =
                $"Impossibile eliminare il piano «{piano.Nome}»: " +
                "contiene locali associati ad apparecchiature.";
            return RedirectToAction(nameof(Index), new { tab });
        }
    }

    _db.Piani.Remove(piano);
    await _db.SaveChangesAsync();
    TempData["Success"] = $"Piano «{piano.Nome}» eliminato.";
    return RedirectToAction(nameof(Index), new { tab });
}

// ─── ELIMINA LOCALE ──────────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaLocale(
    int id, string tab = "struttura")
{
    var locale = await _db.Locali.FindAsync(id);
    if (locale == null)
        return RedirectToAction(nameof(Index), new { tab });

    var haApp = await _db.Apparecchiature
        .AnyAsync(a => a.LocaleId == id);

    if (haApp)
    {
        TempData["Error"] =
            $"Impossibile eliminare il locale «{locale.Nome}»: " +
            "è associato ad una o più apparecchiature. " +
            "Riassegna prima le apparecchiature.";
        return RedirectToAction(nameof(Index), new { tab });
    }

    _db.Locali.Remove(locale);
    await _db.SaveChangesAsync();
    TempData["Success"] = $"Locale «{locale.Nome}» eliminato.";
    return RedirectToAction(nameof(Index), new { tab });
}

// ─── ELIMINA REPARTO ─────────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaReparto(
    int id, string tab = "reparti")
{
    var r = await _db.Reparti.FindAsync(id);
    if (r == null)
        return RedirectToAction(nameof(Index), new { tab });

    var haApp = await _db.Apparecchiature
        .AnyAsync(a => a.RepartoId == id);

    if (haApp)
    {
        TempData["Error"] =
            $"Impossibile eliminare il reparto «{r.Nome}»: " +
            "ha apparecchiature associate. " +
            "Riassegna prima le apparecchiature.";
        return RedirectToAction(nameof(Index), new { tab });
    }

    _db.Reparti.Remove(r);
    await _db.SaveChangesAsync();
    TempData["Success"] = $"Reparto «{r.Nome}» eliminato.";
    return RedirectToAction(nameof(Index), new { tab });
}


// ─── ELIMINA PROTOCOLLO ──────────────────────────────────────────
[HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> EliminaProtocollo(int id, string tab = "protocolli")
{
    var p = await _db.ProtocolliVerifica.FindAsync(id);
    if (p != null)
    {
        var usato = await _db.RecordVerifiche
            .AnyAsync(v => v.ProtocolloId == id);
        if (usato)
        {
            TempData["Error"] =
                "Impossibile eliminare: protocollo utilizzato " +
                "in verifiche esistenti. Disattivalo invece.";
            return RedirectToAction(nameof(Index),
                new { tab = "protocolli" });
        }
        _db.ProtocolliVerifica.Remove(p);
        await _db.SaveChangesAsync();
        TempData["Success"] = $"Protocollo «{p.Codice}» eliminato.";
    }
    return RedirectToAction(nameof(Index),
        new { tab = "protocolli" });
}

        // ═══════════════════════════════════════════════════════════
        // REPARTI
        // ═══════════════════════════════════════════════════════════

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoReparto(
            string nome, string? responsabile, string? email, string tab = "reparti")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.Reparti.Add(new Reparto
                {
                    Nome         = nome,
                    Responsabile = responsabile,
                    Email        = email
                });
                await _db.SaveChangesAsync();
                TempData["Success"] = $"Reparto «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        

        // ═══════════════════════════════════════════════════════════
        // COSTRUTTORI E MODELLI
        // ═══════════════════════════════════════════════════════════

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoCostruttore(
            string nome, string? paese, string? sitoWeb , string tab = "costruttori")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.Costruttori.Add(new Costruttore
                {
                    Nome    = nome,
                    Paese   = paese,
                    SitoWeb = sitoWeb,
                    Attivo  = true
                });
                await _db.SaveChangesAsync();
                TempData["Success"] =
                    $"Costruttore «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoModello(
            int costruttoreId, string nome, string? tipologia, string tab = "costruttori")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.ModelliApparecchiatura.Add(
                    new ModelloApparecchiatura
                    {
                        CostrutoreId = costruttoreId,
                        Nome         = nome,
                        Tipologia    = tipologia,
                        Attivo       = true
                    });
                await _db.SaveChangesAsync();
                TempData["Success"] =
                    $"Modello «{nome}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCostruttore(int id, string tab = "costruttori")
        {
            var c = await _db.Costruttori.FindAsync(id);
            if (c != null)
            {
                c.Attivo = !c.Attivo;
                await _db.SaveChangesAsync();
                TempData["Success"] = c.Attivo
                    ? "Costruttore attivato."
                    : "Costruttore disattivato.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminaModello(int id, string tab = "costruttori")
        {
            var m = await _db.ModelliApparecchiatura.FindAsync(id);
            if (m != null)
            {
                _db.ModelliApparecchiatura.Remove(m);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Modello eliminato.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        // ═══════════════════════════════════════════════════════════
        // SOCIETÀ DI MANUTENZIONE
        // ═══════════════════════════════════════════════════════════

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovaSocieta(
            string nome, string? numeroAssistenza,
            string? numeroReperibilita, string? emailAssistenza,
            string? globalService, string? sitoWeb, string tab = "societa")
        {
            if (!string.IsNullOrWhiteSpace(nome))
            {
                _db.SocietaManutenzione.Add(new SocietaManutenzione
                {
                    Nome                = nome,
                    NumeroAssistenza    = numeroAssistenza,
                    NumeroReperibilita  = numeroReperibilita,
                    EmailAssistenza     = emailAssistenza,
                    GlobalService       = globalService,
                    SitoWeb             = sitoWeb,
                    Attivo              = true
                });
                await _db.SaveChangesAsync();
                TempData["Success"] =
                    $"Società «{nome}» aggiunta.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        

        // ═══════════════════════════════════════════════════════════
        // PROTOCOLLI
        // ═══════════════════════════════════════════════════════════

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> NuovoProtocollo(
            string codice, string descrizione,
            string tipo, int? periodicitaMesi, string tab = "protocolli")
        {
            if (!string.IsNullOrWhiteSpace(codice) &&
                !string.IsNullOrWhiteSpace(descrizione) &&
                Enum.TryParse<RadiologiaAppNew.Enums.TipoProtocollo>(
                    tipo, out var tipoEnum))
            {
                _db.ProtocolliVerifica.Add(new ProtocolloVerifica
                {
                    Codice            = codice,
                    Descrizione       = descrizione,
                    Tipo              = tipoEnum,
                    PeriodicitaMesi   = periodicitaMesi,
                    Revisione         = "Rev. 1",
                    DataEntrataVigore = DateTime.Today,
                    Attivo            = true,
                    CreatedAt         = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
                TempData["Success"] =
                    $"Protocollo «{codice}» aggiunto.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProtocollo(int id, string tab = "protocolli")
        {
            var p = await _db.ProtocolliVerifica.FindAsync(id);
            if (p != null)
            {
                p.Attivo = !p.Attivo;
                await _db.SaveChangesAsync();
                TempData["Success"] = p.Attivo
                    ? "Protocollo attivato."
                    : "Protocollo disattivato.";
            }
            return RedirectToAction(nameof(Index), new { tab });
        }

        // ─── API AJAX — modelli per costruttore ──────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetModelliPerCostruttore(
            int costruttoreId)
        {
            var modelli = await _db.ModelliApparecchiatura
                .Where(m => m.CostrutoreId == costruttoreId &&
                            m.Attivo)
                .OrderBy(m => m.Nome)
                .Select(m => new { m.Id, m.Nome, m.Tipologia })
                .ToListAsync();
            return Json(modelli);
        }
    }

    public class ConfigurazioneVm
    {
        public List<Sito>               Siti       { get; set; } = new();
        public List<Reparto>            Reparti    { get; set; } = new();
        public List<ProtocolloVerifica> Protocolli { get; set; } = new();
        public List<Costruttore>        Costruttori { get; set; } = new();
        public List<SocietaManutenzione> SocietaManutenzione
            { get; set; } = new();
    }

}