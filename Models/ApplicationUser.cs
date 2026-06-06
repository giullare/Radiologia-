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

        // Nome completo calcolato
        public string NomeCompleto => $"{Nome} {Cognome}".Trim();
    }
}