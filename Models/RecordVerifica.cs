using System.ComponentModel.DataAnnotations;
using RadiologiaAppNew.Enums;

namespace RadiologiaAppNew.Models
{
    public class RecordVerifica
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tipo Verifica")]
        public TipoProtocollo Tipo { get; set; }

        [Required]
        [Display(Name = "Data Inizio Verifica")]
        [DataType(DataType.Date)]
        public DateTime DataInizio { get; set; } = DateTime.Today;

        [Display(Name = "Data Fine Verifica")]
        [DataType(DataType.Date)]
        public DateTime? DataFine { get; set; }

        [Required]
        [Display(Name = "Esito")]
        public EsitoVerifica Esito { get; set; } = EsitoVerifica.InCorso;

        [MaxLength(2000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        [Display(Name = "Anno di Riferimento")]
        public int? Anno { get; set; }

        [MaxLength(10)]
        [Display(Name = "Semestre")]
        public string? Semestre { get; set; }

        [Display(Name = "Data Prossima Verifica")]
        [DataType(DataType.Date)]
        public DateTime? ProssimaVerificaData { get; set; }

        // Info guasto
        [MaxLength(1000)]
        [Display(Name = "Info Guasto")]
        public string? InfoGuasto { get; set; }

        [MaxLength(30)]
        [Display(Name = "Tipo Guasto")]
        public string? TipoGuasto { get; set; }

        // Benestare (per accettazione)
        [Display(Name = "Data Benestare Qualità Tecnica")]
        [DataType(DataType.Date)]
        public DateTime? BenestareQualitaTecnicaData { get; set; }

        [MaxLength(255)]
        [Display(Name = "Benestare Qualità Tecnica — Firmato da")]
        public string? BenestareQualitaTecnicaBy { get; set; }

        [Display(Name = "Data Benestare Clinico")]
        [DataType(DataType.Date)]
        public DateTime? BenestareCliniciData { get; set; }

        [MaxLength(255)]
        [Display(Name = "Benestare Clinico — Firmato da")]
        public string? BenestareClinicoBy { get; set; }

        // Manutenzione
        [MaxLength(500)]
        [Display(Name = "Tipo Intervento Manutenzione")]
        public string? TipoInterventoManutenzione { get; set; }

        [MaxLength(255)]
        [Display(Name = "Tecnico Manutentore")]
        public string? TecnicoManutentore { get; set; }

        [MaxLength(255)]
        [Display(Name = "Società Manutenzione")]
        public string? SocietaManutenzione { get; set; }

        [Display(Name = "Data Intervento Manutenzione")]
        [DataType(DataType.Date)]
        public DateTime? DataInterventoManutenzione { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // FK
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        public int ProtocolloId { get; set; }
        public ProtocolloVerifica Protocollo { get; set; } = null!;

        // Navigation
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}