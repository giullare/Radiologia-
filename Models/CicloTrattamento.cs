using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadiologiaAppNew.Models
{
    public class CicloTrattamento
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Numero Ciclo")]
        public int NumeroCiclo { get; set; }

        [Required]
        [Display(Name = "Data Somministrazione")]
        [DataType(DataType.Date)]
        public DateTime DataSomministrazione { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Ora Somministrazione")]
        [DataType(DataType.Time)]
        public TimeSpan OraSomministrazione { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,3)")]
        [Display(Name = "Attività Somministrata (GBq)")]
        public decimal AttivitaSomministrataGbq { get; set; }

        [Required]
        [Column(TypeName = "decimal(6,3)")]
        [Display(Name = "Attività Residua Siringa (GBq)")]
        public decimal AttivitaResiduaSiringaGbq { get; set; }

        [Column(TypeName = "decimal(6,3)")]
        [Display(Name = "Attività Effettiva (GBq)")]
        public decimal AttivitaEffettivaGbq =>
            AttivitaSomministrataGbq - AttivitaResiduaSiringaGbq;

        [Required]
        [MaxLength(100)]
        [Display(Name = "Lotto Radiofarmaco")]
        public string LottoRadiofarmaco { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Display(Name = "Fornitore Radiofarmaco")]
        public string FornitoreRadiofarmaco { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(5,1)")]
        [Display(Name = "Peso Paziente al Ciclo (kg)")]
        public decimal PesoPazienteCicloKg { get; set; }

        [MaxLength(255)]
        [Display(Name = "Infermiere Somministratore")]
        public string? InfermiereSomministratore { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Note Cliniche")]
        public string? NoteCliniche { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Effetti Avversi")]
        public string? EffettiAvversi { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Esito Ciclo")]
        public string EsitoCiclo { get; set; } = "Completato";

        [MaxLength(500)]
        [Display(Name = "Motivo Modifica / Sospensione")]
        public string? MotivoModifica { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // FK
        public int PazienteId { get; set; }
        public PazienteLu177 Paziente { get; set; } = null!;

        // Navigation
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
        public ICollection<DatoEmatologico> DatiEmatologici { get; set; } = new List<DatoEmatologico>();
    }
}