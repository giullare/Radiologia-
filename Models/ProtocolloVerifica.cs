using System.ComponentModel.DataAnnotations;
using RadiologiaAppNew.Enums;

namespace RadiologiaAppNew.Models
{
    public class ProtocolloVerifica
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il codice è obbligatorio")]
        [MaxLength(50)]
        [Display(Name = "Codice Protocollo")]
        public string Codice { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [MaxLength(500)]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo Protocollo")]
        public TipoProtocollo Tipo { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ambiti Applicabilità")]
        public string? AmbitiApplicabilita { get; set; }

        [MaxLength(500)]
        [Display(Name = "Tipologie Applicabilità")]
        public string? TipologieApplicabilita { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Revisione")]
        public string Revisione { get; set; } = "Rev. 1";

        [Required]
        [Display(Name = "Data Entrata in Vigore")]
        [DataType(DataType.Date)]
        public DateTime DataEntrataVigore { get; set; } = DateTime.Today;

        [Display(Name = "Periodicità (mesi)")]
        public int? PeriodicitaMesi { get; set; }

        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<RecordVerifica> RecordVerifiche { get; set; } = new List<RecordVerifica>();
    }
}