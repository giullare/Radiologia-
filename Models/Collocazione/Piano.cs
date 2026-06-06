using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models.Collocazione
{
    public class Piano
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome del piano è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Piano")]
        public string Nome { get; set; } = string.Empty;

        [Display(Name = "Numero Piano")]
        public int? Numero { get; set; }

        // FK
        public int ImmobileId { get; set; }
        public Immobile Immobile { get; set; } = null!;

        // Navigation
        public ICollection<Locale> Locali { get; set; } = new List<Locale>();
    }
}