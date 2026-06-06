using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.ViewModels.Account
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(30)]
        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [Display(Name = "Ruoli assegnati")]
        public List<string> Ruoli { get; set; } = new();

        [Display(Name = "Ultimo accesso")]
        public DateTime? UltimoAccesso { get; set; }
    }
}