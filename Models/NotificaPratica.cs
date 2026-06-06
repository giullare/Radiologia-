using System.ComponentModel.DataAnnotations;


namespace RadiologiaAppNew.Models
{
    public class NotificaPratica
    {
        
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Numero Protocollo PEC")]
        public string NumeroProtocolloPec { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Data Notifica")]
        [DataType(DataType.Date)]
        public DateTime DataNotifica { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(255)]
        [Display(Name = "Ente Destinatario")]
        public string EnteDestinatario { get; set; } = string.Empty;

        [Display(Name = "Documento Inviato all'RSPP")]
        public bool InviatoRspp { get; set; } = false;

        [Display(Name = "Data Invio RSPP")]
        [DataType(DataType.Date)]
        public DateTime? DataInvioRspp { get; set; }

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