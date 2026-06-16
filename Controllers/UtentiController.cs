using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Helpers;
using RadiologiaAppNew.Models;
using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Controllers
{
    [Authorize(Roles = "ADMIN_ORG,SUPER_ADMIN")]
    public class UtentiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UtentiController> _logger;

        private static readonly string[] RuoliSistema = {
            "ADMIN_ORG","EFM","EDR","RIR","SFM",
            "MA","ES","MR","RQ","OPERATORE","READER"
        };

        public UtentiController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UtentiController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger      = logger;
        }

        // ─── INDEX ───────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Gestione Utenti";
            ViewData["BreadcrumbParent"] = "Amministrazione";

            var utenti = await _userManager.Users
                .OrderBy(u => u.Cognome)
                .ThenBy(u => u.Nome)
                .ToListAsync();

            var vm = new List<UtenteRowVm>();
            foreach (var u in utenti)
            {
                var ruoli = await _userManager.GetRolesAsync(u);
                var moduliEffettivi = ModuliHelper.GetModuliEffettivi(
                    u.ModuliAbilitati, ruoli);

                vm.Add(new UtenteRowVm
                {
                    Id             = u.Id,
                    Nome           = u.Nome,
                    Cognome        = u.Cognome,
                    Email          = u.Email ?? "",
                    Attivo         = u.Attivo,
                    UltimoAccesso  = u.UltimoAccesso,
                    Ruoli          = ruoli.ToList(),
                    ModuliEffettivi = moduliEffettivi
                });
            }

            return View(vm);
        }

        // ─── CREATE GET ──────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuovo Utente";
            ViewData["BreadcrumbParent"] = "Gestione Utenti";
            ViewData["BreadcrumbParentUrl"] = "/Utenti";
            ViewBag.RuoliSistema  = RuoliSistema;
            ViewBag.TuttiIModuli  = ModuliHelper.TuttiIModuli;
            ViewBag.NomiModuli    = ModuliHelper.NomiModuli;
            ViewBag.IconeModuli   = ModuliHelper.IconeModuli;
            return View(new CreaUtenteVm());
        }

        // ─── CREATE POST ─────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreaUtenteVm model)
        {
            ViewBag.RuoliSistema = RuoliSistema;
            ViewBag.TuttiIModuli = ModuliHelper.TuttiIModuli;
            ViewBag.NomiModuli   = ModuliHelper.NomiModuli;
            ViewBag.IconeModuli  = ModuliHelper.IconeModuli;

            if (!ModelState.IsValid)
                return View(model);

            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email",
                    "Email già in uso da un altro account.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName        = model.Email,
                Email           = model.Email,
                Nome            = model.Nome,
                Cognome         = model.Cognome,
                TelefonoInterno = model.Telefono,
                EmailConfirmed  = true,
                Attivo          = true,
                CreatedAt       = DateTime.UtcNow
            };

            // Assegna moduli: se non selezionati esplicitamente,
            // calcola il default dai ruoli scelti
            if (model.ModuliSelezionati?.Any() == true)
            {
                user.SetModuli(model.ModuliSelezionati);
            }
            else
            {
                // Default da ruolo: non salviamo nulla (null = usa default dinamico)
                user.ModuliAbilitati = null;
            }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(model);
            }

            if (model.RuoliSelezionati?.Any() == true)
            {
                foreach (var r in model.RuoliSelezionati)
                    await _userManager.AddToRoleAsync(user, r);
            }

            _logger.LogInformation(
                "Utente {Email} creato da {Admin}.",
                model.Email, User.Identity?.Name);

            TempData["Success"] =
                $"Utente {model.Nome} {model.Cognome} creato con successo.";
            return RedirectToAction(nameof(Index));
        }

        // ─── EDIT GET ────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var ruoliUtente  = await _userManager.GetRolesAsync(user);
            var defaultModuli = ModuliHelper.GetDefaultModuliPerRuoli(ruoliUtente);

            // I moduli selezionati nella UI sono quelli espliciti (se presenti),
            // altrimenti mostriamo i default pre-spuntati
            var moduliDaMostrare = user.GetModuli().Any()
                ? user.GetModuli()
                : defaultModuli;

            ViewData["Title"] = $"Modifica — {user.NomeCompleto}";
            ViewData["BreadcrumbParent"] = "Gestione Utenti";
            ViewData["BreadcrumbParentUrl"] = "/Utenti";
            ViewBag.RuoliSistema       = RuoliSistema;
            ViewBag.TuttiIModuli       = ModuliHelper.TuttiIModuli;
            ViewBag.NomiModuli         = ModuliHelper.NomiModuli;
            ViewBag.IconeModuli        = ModuliHelper.IconeModuli;
            ViewBag.DefaultModuli      = defaultModuli; // per JS hint
            ViewBag.ModuliSonoDefault  = !user.GetModuli().Any(); // true = non personalizzati

            return View(new ModificaUtenteVm
            {
                Id               = user.Id,
                Nome             = user.Nome,
                Cognome          = user.Cognome,
                Email            = user.Email ?? "",
                Telefono         = user.TelefonoInterno,
                Attivo           = user.Attivo,
                RuoliSelezionati = ruoliUtente.ToList(),
                ModuliSelezionati = moduliDaMostrare,
                UsaDefaultModuli  = !user.GetModuli().Any()
            });
        }

        // ─── EDIT POST ───────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ModificaUtenteVm model)
        {
            ViewBag.RuoliSistema = RuoliSistema;
            ViewBag.TuttiIModuli = ModuliHelper.TuttiIModuli;
            ViewBag.NomiModuli   = ModuliHelper.NomiModuli;
            ViewBag.IconeModuli  = ModuliHelper.IconeModuli;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Nome            = model.Nome;
            user.Cognome         = model.Cognome;
            user.TelefonoInterno = model.Telefono;
            user.Attivo          = model.Attivo;

            // Gestione moduli
            if (model.UsaDefaultModuli)
            {
                // Ripristina default da ruolo (toglie personalizzazione)
                user.ModuliAbilitati = null;
            }
            else if (model.ModuliSelezionati?.Any() == true)
            {
                user.SetModuli(model.ModuliSelezionati);
            }
            else
            {
                user.ModuliAbilitati = null;
            }

            await _userManager.UpdateAsync(user);

            // Aggiorna ruoli
            var ruoliAttuali = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, ruoliAttuali);

            if (model.RuoliSelezionati?.Any() == true)
            {
                foreach (var r in model.RuoliSelezionati)
                    await _userManager.AddToRoleAsync(user, r);
            }

            // Reset password se richiesto
            if (!string.IsNullOrEmpty(model.NuovaPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var res   = await _userManager.ResetPasswordAsync(
                    user, token, model.NuovaPassword);
                if (!res.Succeeded)
                {
                    foreach (var e in res.Errors)
                        ModelState.AddModelError("", e.Description);
                    return View(model);
                }
            }

            TempData["Success"] = "Utente aggiornato con successo.";
            return RedirectToAction(nameof(Index));
        }

        // ─── API — DEFAULT MODULI PER RUOLI (chiamata AJAX dalla UI) ─────
        [HttpPost]
        public IActionResult GetDefaultModuli([FromBody] List<string> ruoli)
        {
            var moduli = ModuliHelper.GetDefaultModuliPerRuoli(ruoli ?? new List<string>());
            return Json(moduli);
        }

        // ─── TOGGLE ATTIVO ───────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAttivo(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentId = _userManager.GetUserId(User);
            if (user.Id == currentId)
            {
                TempData["Error"] =
                    "Non puoi disabilitare il tuo stesso account.";
                return RedirectToAction(nameof(Index));
            }

            user.Attivo = !user.Attivo;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = user.Attivo
                ? $"Account {user.Email} riabilitato."
                : $"Account {user.Email} disabilitato.";

            return RedirectToAction(nameof(Index));
        }

        // ─── DETAIL ──────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var ruoli = await _userManager.GetRolesAsync(user);
            var moduliEffettivi = ModuliHelper.GetModuliEffettivi(
                user.ModuliAbilitati, ruoli);

            ViewData["Title"] = $"{user.NomeCompleto} — Profilo";
            ViewData["BreadcrumbParent"] = "Gestione Utenti";
            ViewData["BreadcrumbParentUrl"] = "/Utenti";

            return View(new UtenteDetailVm
            {
                Id              = user.Id,
                Nome            = user.Nome,
                Cognome         = user.Cognome,
                Email           = user.Email ?? "",
                Telefono        = user.TelefonoInterno,
                Attivo          = user.Attivo,
                UltimoAccesso   = user.UltimoAccesso,
                CreatedAt       = user.CreatedAt,
                Ruoli           = ruoli.ToList(),
                ModuliEffettivi = moduliEffettivi,
                ModuliPersonalizzati = user.GetModuli().Any()
            });
        }

        // ─── DELETE ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var currentId = _userManager.GetUserId(User);
            if (id == currentId)
            {
                TempData["Error"] =
                    "Non puoi eliminare il tuo stesso account.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var nomeUtente = user.NomeCompleto;
            var result     = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                TempData["Success"] = $"Utente {nomeUtente} eliminato definitivamente.";
            else
                TempData["Error"] = "Errore durante l'eliminazione dell'utente.";

            return RedirectToAction(nameof(Index));
        }
    }

    // ─── VIEW MODELS ─────────────────────────────────────────────────────

    public class UtenteRowVm
    {
        public string Id             { get; set; } = "";
        public string Nome           { get; set; } = "";
        public string Cognome        { get; set; } = "";
        public string Email          { get; set; } = "";
        public bool   Attivo         { get; set; }
        public DateTime? UltimoAccesso { get; set; }
        public List<string> Ruoli    { get; set; } = new();
        public List<string> ModuliEffettivi { get; set; } = new();
    }

    public class CreaUtenteVm
    {
        [Required] public string Nome     { get; set; } = "";
        [Required] public string Cognome  { get; set; } = "";
        [Required][EmailAddress] public string Email { get; set; } = "";
        public string? Telefono           { get; set; }
        [Required][MinLength(8)] public string Password { get; set; } = "";
        public List<string> RuoliSelezionati  { get; set; } = new();
        public List<string> ModuliSelezionati { get; set; } = new();
    }

    public class ModificaUtenteVm
    {
        public string Id              { get; set; } = "";
        [Required] public string Nome    { get; set; } = "";
        [Required] public string Cognome { get; set; } = "";
        public string Email           { get; set; } = "";
        public string? Telefono       { get; set; }
        public bool   Attivo          { get; set; } = true;
        public string? NuovaPassword  { get; set; }
        public List<string> RuoliSelezionati  { get; set; } = new();
        public List<string> ModuliSelezionati { get; set; } = new();

        /// <summary>
        /// Se true, i moduli vengono calcolati automaticamente dal ruolo (default).
        /// Se false, si usano ModuliSelezionati.
        /// </summary>
        public bool UsaDefaultModuli { get; set; } = true;
    }

    public class UtenteDetailVm
    {
        public string    Id              { get; set; } = "";
        public string    Nome            { get; set; } = "";
        public string    Cognome         { get; set; } = "";
        public string    Email           { get; set; } = "";
        public string?   Telefono        { get; set; }
        public bool      Attivo          { get; set; }
        public DateTime? UltimoAccesso   { get; set; }
        public DateTime  CreatedAt       { get; set; }
        public List<string> Ruoli        { get; set; } = new();
        public List<string> ModuliEffettivi      { get; set; } = new();
        public bool         ModuliPersonalizzati { get; set; }
    }
}