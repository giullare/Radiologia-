using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RadiologiaAppNew.Enums;

namespace RadiologiaAppNew.Models
{
    public class PazienteLu177
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il codice paziente è obbligatorio")]
        [MaxLength(50)]
        [Display(Name = "Codice Paziente")]
        public string CodicePaziente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "La data di nascita è obbligatoria")]
        [Display(Name = "Data di Nascita")]
        [DataType(DataType.Date)]
        public DateTime DataNascita { get; set; }

        [Required]
        [MaxLength(1)]
        [Display(Name = "Sesso")]
        public string Sesso { get; set; } = string.Empty;

        [MaxLength(16)]
        [Display(Name = "Codice Fiscale")]
        public string? CodiceFiscale { get; set; }

        [MaxLength(100)]
        [Display(Name = "Numero Nosologico")]
        public string? NumeroNosologico { get; set; }

        [Required(ErrorMessage = "Il medico inviante è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Medico Inviante")]
        public string MedicoInviante { get; set; } = string.Empty;

        [MaxLength(255)]
        [Display(Name = "Reparto Inviante")]
        public string? RepartoInviante { get; set; }

        [Required(ErrorMessage = "La diagnosi è obbligatoria")]
        [MaxLength(1000)]
        [Display(Name = "Diagnosi Principale")]
        public string DiagnosiPrincipale { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Indicazione al Trattamento")]
        public string Indicazione { get; set; } = "PSMA_LU177";

        [Display(Name = "Data Prima Visita")]
        [DataType(DataType.Date)]
        public DateTime? DataPrimaVisita { get; set; }

        [Required]
        [Display(Name = "Stato Paziente")]
        public StatoPaziente StatoPaziente { get; set; } = StatoPaziente.InTrattamento;

        // Dati clinici
        [Column(TypeName = "decimal(5,1)")]
        [Display(Name = "Peso (kg)")]
        public decimal? PesoKg { get; set; }

        [Column(TypeName = "decimal(5,1)")]
        [Display(Name = "Altezza (cm)")]
        public decimal? AltezzaCm { get; set; }

        [Column(TypeName = "decimal(6,1)")]
        [Display(Name = "eGFR (mL/min/1.73m²)")]
        public decimal? EgfrMlMin { get; set; }

        [MaxLength(500)]
        [Display(Name = "Note Funzionalità Epatica")]
        public string? FunzionalitaEpatica { get; set; }

        [MaxLength(500)]
        [Display(Name = "Controindicazioni")]
        public string? Controindicazioni { get; set; }

        // Piano terapeutico
        [MaxLength(100)]
        [Display(Name = "Protocollo Terapeutico")]
        public string? ProtocolloTerapeutico { get; set; }

        [Display(Name = "Numero Cicli Pianificati")]
        public int? NCicliPianificati { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Attività per Ciclo (GBq)")]
        public decimal? AttivitaPerCicloGbq { get; set; }

        [Display(Name = "Intervallo tra Cicli (settimane)")]
        public int? IntervallSettimane { get; set; }

        [Display(Name = "Data Inizio Trattamento")]
        [DataType(DataType.Date)]
        public DateTime? DataInizioTrattamento { get; set; }

        // Timestamps
        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // Navigation
        public ICollection<CicloTrattamento> CicliTrattamento { get; set; } = new List<CicloTrattamento>();
        public ICollection<DatoEmatologico> DatiEmatologici { get; set; } = new List<DatoEmatologico>();
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}