using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Data;
using RadiologiaAppNew.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── DATABASE ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=radiologia.db"
    ));

// ─── IDENTITY ────────────────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ─── COOKIE ──────────────────────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

// ─── MVC ─────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ─── PIPELINE ────────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// ─── SEED DATABASE ────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();
        await SeedDatabase(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Errore durante il seed del database.");
    }
}

app.Run();

// ─── SEED METHOD ─────────────────────────────────────────────────────────────
static async Task SeedDatabase(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
    // Crea ruoli
    string[] ruoli = {
        "ADMIN_ORG", "EFM", "EDR", "RIR", "SFM",
        "MA", "ES", "MR", "RQ", "OPERATORE", "READER"
    };

    foreach (var ruolo in ruoli)
    {
        if (!await roleManager.RoleExistsAsync(ruolo))
            await roleManager.CreateAsync(new IdentityRole(ruolo));
    }

    // Crea utente admin di default
    const string adminEmail = "admin@radiologia.local";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nome = "Amministratore",
            Cognome = "Sistema",
            EmailConfirmed = true,
            Attivo = true
        };

        var result = await userManager.CreateAsync(admin, "Admin@1234");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "ADMIN_ORG");
            await userManager.AddToRoleAsync(admin, "EFM");
            await userManager.AddToRoleAsync(admin, "EDR");
        }
    }
}