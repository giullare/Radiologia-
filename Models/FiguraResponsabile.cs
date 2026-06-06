using System.ComponentModel.DataAnnotations;
using RadiologiaAppNew.Enums;

namespace RadiologiaAppNew.Models
{
    public class FiguraResponsabile
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ruolo")]
        public RuoloResponsabile Ruolo { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [MaxLength(30)]
        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }

        [MaxLength(1)]
        [Display(Name = "Grado Abilitazione EDR")]
        public string? GradoAbilitazione { get; set; }

        [Display(Name = "Valido Dal")]
        [DataType(DataType.Date)]
        public DateTime ValidoDal { get; set; } = DateTime.Today;

        [Display(Name = "Valido Al")]
        [DataType(DataType.Date)]
        public DateTime? ValidoAl { get; set; }

        // FK
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;
    }
}