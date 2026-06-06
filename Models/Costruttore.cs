using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class Costruttore
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Nome Costruttore / Produttore")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Paese di Origine")]
        public string? Paese { get; set; }

        [MaxLength(255)]
        [Display(Name = "Sito Web")]
        public string? SitoWeb { get; set; }

        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        public bool Attivo { get; set; } = true;

        // Navigation
        public ICollection<ModelloApparecchiatura> Modelli { get; set; }
            = new List<ModelloApparecchiatura>();
    }

    public class ModelloApparecchiatura
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome modello è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Nome Modello")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Tipologia")]
        public string? Tipologia { get; set; }

        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        public bool Attivo { get; set; } = true;

        // FK
        public int CostrutoreId { get; set; }
        public Costruttore Costruttore { get; set; } = null!;
    }
}