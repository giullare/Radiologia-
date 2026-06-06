using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models.Collocazione;

namespace RadiologiaAppNew.Models
{
    public class Apparecchiatura
    {
        public int Id { get; set; }

        // ─── IDENTIFICAZIONE ────────────────────────────────────────────
        [Required(ErrorMessage = "Il codice è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Codice Interno")]
        public string Codice { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descrizione è obbligatoria")]
        [MaxLength(255)]
        [Display(Name = "Descrizione")]
        public string Descrizione { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il modulo è obbligatorio")]
        [Display(Name = "Modulo")]
        public TipoModulo Modulo { get; set; }

        [Display(Name = "Ambito di Intervento")]
        public AmbitoIntervento? AmbitoIntervento { get; set; }

        [Required(ErrorMessage = "La tipologia è obbligatoria")]
        [MaxLength(100)]
        [Display(Name = "Tipologia")]
        public string Tipologia { get; set; } = string.Empty;

        // ─── DATI TECNICI IDENTIFICATIVI ────────────────────────────────
        [Required(ErrorMessage = "Il modello è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Modello")]
        public string Modello { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il costruttore è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Costruttore / Produttore")]
        public string Costruttore { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matricola è obbligatoria")]
        [MaxLength(100)]
        [Display(Name = "Matricola")]
        public string Matricola { get; set; } = string.Empty;

        [MaxLength(100)]
        [Display(Name = "Numero Seriale")]
        public string? SerialNumber { get; set; }

        // ─── PARAMETRI RADIOLOGICI ───────────────────────────────────────
        [Display(Name = "Corrente Max (mA)")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? CorrenteMaxMa { get; set; }

        [Display(Name = "Tensione Max (kV)")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? TensioneMaxKvolt { get; set; }

        [Display(Name = "Energia Max (keV)")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? EnergiaMaxKev { get; set; }

        // Solo per RM
        [Display(Name = "Intensità Campo (Tesla)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? IntensitaCampoTesla { get; set; }

        [MaxLength(30)]
        [Display(Name = "Tipo Magnete")]
        public string? TipoMagnete { get; set; }

        // ─── COLLOCAZIONE ────────────────────────────────────────────────
        [Display(Name = "Locale")]
        public int? LocaleId { get; set; }
        public Locale? Locale { get; set; }

        [Display(Name = "Reparto")]
        public int? RepartoId { get; set; }
        public Reparto? Reparto { get; set; }

        [MaxLength(255)]
        [Display(Name = "Caposala / Referente Reparto")]
        public string? Caposala { get; set; }

        // ─── RIFERIMENTI ASSISTENZA ──────────────────────────────────────
        [MaxLength(255)]
        [Display(Name = "Società di Manutenzione")]
        public string? SocietaManutenzione { get; set; }

        [MaxLength(255)]
        [Display(Name = "Tecnico di Riferimento")]
        public string? TecnicoRiferimento { get; set; }

        [MaxLength(50)]
        [Display(Name = "Numero Assistenza Tecnica")]
        public string? NumeroAssistenza { get; set; }

        [MaxLength(255)]
        [Display(Name = "Global Service")]
        public string? GlobalService { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email Assistenza")]
        public string? EmailAssistenza { get; set; }

        // ─── FLAGS TECNICI ───────────────────────────────────────────────
        [Display(Name = "Collegata in Rete (LAN)")]
        public bool LanCollegata { get; set; } = false;

        [Display(Name = "Software MedSquare Installato")]
        public bool MedsquareInstallato { get; set; } = false;

        // ─── CICLO DI VITA ───────────────────────────────────────────────
        [Required]
        [Display(Name = "Stato")]
        public StatoApparecchiatura Stato { get; set; } = StatoApparecchiatura.InInstallazione;

        [Display(Name = "Data Accettazione / Collaudo")]
        [DataType(DataType.Date)]
        public DateTime? DataAccettazione { get; set; }

        [Display(Name = "Data Cessazione")]
        [DataType(DataType.Date)]
        public DateTime? DataCessazione { get; set; }

        [MaxLength(500)]
        [Display(Name = "Motivo Cessazione")]
        public string? MotivoCessazione { get; set; }

        // ─── BENE AZIENDALE ──────────────────────────────────────────────
        [MaxLength(100)]
        [Display(Name = "ID SAP")]
        public string? SapId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Descrizione SIAP")]
        public string? SiapDescrizione { get; set; }

        // ─── ADEMPIMENTI INAIL ───────────────────────────────────────────
        [Display(Name = "Stato INAIL")]
        public StatoAdempimento StatoInail { get; set; } = StatoAdempimento.DaRegistrare;

        [Display(Name = "Data Registrazione INAIL")]
        [DataType(DataType.Date)]
        public DateTime? DataRegistrazioneInail { get; set; }

        [MaxLength(100)]
        [Display(Name = "Numero Pratica INAIL")]
        public string? NumeroPraticaInail { get; set; }

        // ─── ADEMPIMENTI STRIMS ──────────────────────────────────────────
        [Display(Name = "Stato STRIMS")]
        public StatoAdempimento StatoStrims { get; set; } = StatoAdempimento.DaRegistrare;

        [MaxLength(100)]
        [Display(Name = "ID Apparecchiatura STRIMS")]
        public string? StrimsIdApparecchiatura { get; set; }

        [Display(Name = "Data Registrazione STRIMS")]
        [DataType(DataType.Date)]
        public DateTime? DataRegistrazioneStrims { get; set; }

        [Display(Name = "Notifica di Pratica Caricata su STRIMS")]
        public bool StrimsNpCaricata { get; set; } = false;

        [Display(Name = "Notifica di Cessazione Caricata su STRIMS")]
        public bool StrimsNcCaricata { get; set; } = false;

        // ─── PLANIMETRIA E ZONE ──────────────────────────────────────────
        [MaxLength(1000)]
        [Display(Name = "Descrizione Zone Classificate")]
        public string? DescrizioneZoneClassificate { get; set; }

        // ─── TIMESTAMPS ──────────────────────────────────────────────────
        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Ultima Modifica")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // ─── NAVIGATION PROPERTIES ───────────────────────────────────────
        public ICollection<FiguraResponsabile> FigureResponsabili { get; set; } = new List<FiguraResponsabile>();
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
        public ICollection<RecordVerifica> RecordVerifiche { get; set; } = new List<RecordVerifica>();
        public ICollection<NotificaPratica> NotifichePratica { get; set; } = new List<NotificaPratica>();
        public ICollection<NullaOsta> NullaOsta { get; set; } = new List<NullaOsta>();
        public ICollection<Verbale> Verbali { get; set; } = new List<Verbale>();
    }
}