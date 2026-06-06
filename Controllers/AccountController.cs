using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RadiologiaAppNew.Models;
using RadiologiaAppNew.ViewModels.Account;

namespace RadiologiaAppNew.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // ─── LOGIN ───────────────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.Attivo)
            {
                ModelState.AddModelError(string.Empty,
                    "Credenziali non valide o account disabilitato.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RicordamiAccesso,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Aggiorna ultimo accesso
                user.UltimoAccesso = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Utente {Email} ha effettuato l'accesso.", model.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Account {Email} bloccato.", model.Email);
                ModelState.AddModelError(string.Empty,
                    "Account temporaneamente bloccato per troppi tentativi. Riprova tra 30 minuti.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email o password non corretti.");
            return View(model);
        }

        // ─── LOGOUT ──────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Utente ha effettuato il logout.");
            return RedirectToAction("Login");
        }

        // ─── ACCESS DENIED ───────────────────────────────────────────────
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ─── PROFILO ─────────────────────────────────────────────────────
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var ruoli = await _userManager.GetRolesAsync(user);

            var vm = new ProfileViewModel
            {
                Nome        = user.Nome,
                Cognome     = user.Cognome,
                Email       = user.Email ?? string.Empty,
                Telefono    = user.TelefonoInterno,
                Ruoli       = ruoli.ToList(),
                UltimoAccesso = user.UltimoAccesso
            };

            ViewData["Title"] = "Il mio profilo";
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.Nome             = model.Nome;
            user.Cognome          = model.Cognome;
            user.TelefonoInterno  = model.Telefono;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profilo aggiornato con successo.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ─── CAMBIA PASSWORD ─────────────────────────────────────────────
        [HttpGet]
        [Authorize]
        public IActionResult CambiaPassword()
        {
            ViewData["Title"] = "Cambia Password";
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiaPassword(CambiaPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                user, model.PasswordAttuale, model.NuovaPassword);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Password cambiata con successo.";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
    }
}