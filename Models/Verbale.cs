using System.ComponentModel.DataAnnotations;


namespace RadiologiaAppNew.Models
{
    public class Verbale
    {
       
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data Sopralluogo")]
        [DataType(DataType.Date)]
        public DateTime DataSopralluogo { get; set; } = DateTime.Today;

        [MaxLength(500)]
        [Display(Name = "Partecipanti")]
        public string? Partecipanti { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Oggetto")]
        public string Oggetto { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        [Display(Name = "Rilievi e Osservazioni")]
        public string Rilievi { get; set; } = string.Empty;

        [MaxLength(2000)]
        [Display(Name = "Non Conformità Rilevate")]
        public string? NonConformita { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Azioni Correttive")]
        public string? AzioniCorrettive { get; set; }

        [Display(Name = "Scadenza Azioni Correttive")]
        [DataType(DataType.Date)]
        public DateTime? ScadenzaAzioni { get; set; }

        [MaxLength(20)]
        [Display(Name = "Stato")]
        public string Stato { get; set; } = "Aperto";

        [Display(Name = "Data Chiusura")]
        [DataType(DataType.Date)]
        public DateTime? DataChiusura { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // FK
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        // Navigation
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}