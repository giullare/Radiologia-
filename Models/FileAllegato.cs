using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class FileAllegato
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        [Display(Name = "Nome File Originale")]
        public string NomeOriginale { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Display(Name = "Nome Storage")]
        public string NomeStorage { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Tipo MIME")]
        public string? MimeType { get; set; }

        [Display(Name = "Dimensione (bytes)")]
        public long DimensioneBytes { get; set; }

        [MaxLength(60)]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Display(Name = "Versione")]
        public int Versione { get; set; } = 1;

        [Display(Name = "Sostituisce File ID")]
        public int? SostituisceFileId { get; set; }

        [Display(Name = "Data Upload")]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? UploadedByUserId { get; set; }

        // FK — relazioni polimorfiche (una sola valorizzata)
        public int? ApparecchiaturaId { get; set; }
        public Apparecchiatura? Apparecchiatura { get; set; }

        public int? VerificaId { get; set; }
        public RecordVerifica? Verifica { get; set; }

        public int? VerbaleId { get; set; }
        public Verbale? Verbale { get; set; }

        public int? PazienteId { get; set; }
        public PazienteLu177? Paziente { get; set; }

        public int? CicloId { get; set; }
        public CicloTrattamento? Ciclo { get; set; }
    }
}