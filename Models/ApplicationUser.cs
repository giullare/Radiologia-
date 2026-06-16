using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        [MaxLength(30)]
        [Display(Name = "Telefono Interno")]
        public string? TelefonoInterno { get; set; }

        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        [Display(Name = "Data Ultimo Accesso")]
        public DateTime? UltimoAccesso { get; set; }

        [Display(Name = "Data Registrazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Moduli abilitati per questo utente, salvati come stringa CSV.
        /// Valori possibili: MOD1, MOD2, MOD3, MOD4
        /// Esempio: "MOD1,MOD2,MOD4"
        /// Null o vuoto = nessun modulo abilitato esplicitamente (usa default da ruolo).
        /// </summary>
        [MaxLength(50)]
        public string? ModuliAbilitati { get; set; }

        // Nome completo calcolato
        public string NomeCompleto => $"{Nome} {Cognome}".Trim();

        // Helper per leggere/scrivere come lista
        public List<string> GetModuli()
        {
            if (string.IsNullOrWhiteSpace(ModuliAbilitati))
                return new List<string>();
            return ModuliAbilitati
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim())
                .ToList();
        }

        public void SetModuli(IEnumerable<string> moduli)
        {
            var lista = moduli
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct()
                .OrderBy(m => m)
                .ToList();
            ModuliAbilitati = lista.Any() ? string.Join(",", lista) : null;
        }

        public bool HasModulo(string modulo) =>
            GetModuli().Contains(modulo, StringComparer.OrdinalIgnoreCase);
    }
}