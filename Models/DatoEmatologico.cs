using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RadiologiaAppNew.Models
{
    public class DatoEmatologico
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data Prelievo")]
        [DataType(DataType.Date)]
        public DateTime DataPrelievo { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(30)]
        [Display(Name = "Timing")]
        public string Timing { get; set; } = string.Empty;

        [Column(TypeName = "decimal(6,2)")]
        [Display(Name = "WBC (×10⁹/L)")]
        public decimal? WbcX10_9L { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Hgb (g/dL)")]
        public decimal? HgbGdl { get; set; }

        [Column(TypeName = "decimal(7,1)")]
        [Display(Name = "PLT (×10⁹/L)")]
        public decimal? PltX10_9L { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        [Display(Name = "Neutrofili (×10⁹/L)")]
        public decimal? NeutrofiliX10_9L { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        [Display(Name = "Creatinina (mg/dL)")]
        public decimal? CreatininaMgDl { get; set; }

        [Column(TypeName = "decimal(6,1)")]
        [Display(Name = "eGFR (mL/min)")]
        public decimal? EgfrMlMin { get; set; }

        [Column(TypeName = "decimal(10,3)")]
        [Display(Name = "PSA (ng/mL)")]
        public decimal? PsaNgMl { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Cromogranina A")]
        public decimal? CromograninaA { get; set; }

        [MaxLength(2)]
        [Display(Name = "Tossicità Ematologica (CTCAE)")]
        public string? TossicitaEmatologica { get; set; }

        [MaxLength(1000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // FK
        public int PazienteId { get; set; }
        public PazienteLu177 Paziente { get; set; } = null!;

        public int? CicloId { get; set; }
        public CicloTrattamento? Ciclo { get; set; }

        // Navigation
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}