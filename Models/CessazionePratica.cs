using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class CessazionePratica
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Numero Protocollo PEC")]
        public string NumeroProtocolloPec { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Data Cessazione")]
        [DataType(DataType.Date)]
        public DateTime DataCessazione { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(255)]
        [Display(Name = "Ente Destinatario")]
        public string EnteDestinatario { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        // FILE
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}