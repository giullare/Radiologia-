using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models.Collocazione
{
    public class Reparto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome del reparto è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Nome Reparto")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(255)]
        [Display(Name = "Responsabile / Caposala")]
        public string? Responsabile { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email Reparto")]
        public string? Email { get; set; }

        // Navigation
        public ICollection<Apparecchiatura> Apparecchiature { get; set; } = new List<Apparecchiatura>();
    }
}