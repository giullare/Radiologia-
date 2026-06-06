using System.ComponentModel.DataAnnotations;
using RadiologiaAppNew.Enums;


namespace RadiologiaAppNew.Models
{
    public class NullaOsta
    {
        
        public int Id { get; set; }
        [Required]
        [Display(Name = "Tipo Nulla Osta")]
        public TipoNullaOsta Tipo { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Numero Nulla Osta")]
        public string Numero { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Data Rilascio")]
        [DataType(DataType.Date)]
        public DateTime DataRilascio { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(255)]
        [Display(Name = "Ente Rilascio")]
        public string EnteRilascio { get; set; } = string.Empty;

        [Display(Name = "Data Scadenza")]
        [DataType(DataType.Date)]
        public DateTime? DataScadenza { get; set; }

        [Required]
        [Display(Name = "Stato")]
        public StatoNullaOsta Stato { get; set; } = StatoNullaOsta.Valido;

        [MaxLength(1000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        // Navigation
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}