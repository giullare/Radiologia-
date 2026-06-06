using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models.Collocazione
{
    public class Locale
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome del locale è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Nome Locale / Stanza")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(50)]
        [Display(Name = "Codice Locale")]
        public string? Codice { get; set; }

        // FK
        public int PianoId { get; set; }
        public Piano Piano { get; set; } = null!;

        // Navigation
        public ICollection<Apparecchiatura> Apparecchiature { get; set; } = new List<Apparecchiatura>();
    }
}