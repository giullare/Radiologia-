using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models.Collocazione
{
    public class Sito
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome del sito è obbligatorio")]
        [MaxLength(255, ErrorMessage = "Massimo 255 caratteri")]
        [Display(Name = "Nome Sito / Presidio")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Indirizzo")]
        public string? Indirizzo { get; set; }

        // Navigation
        public ICollection<Immobile> Immobili { get; set; } = new List<Immobile>();
    }
}