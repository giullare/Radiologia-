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
        public TipoModulo Modulo { get; set; } = TipoModulo.Radiologica;

        [Display(Name = "Ambito")]
        public AmbitoIntervento? AmbitoIntervento { get; set; }

        [Required(ErrorMessage = "La tipologia è obbligatoria")]
        [MaxLength(100)]
        [Display(Name = "Tipologia")]
        public string Tipologia { get; set; } = string.Empty;

        // ─── BENE AZIENDALE (dopo tipologia come richiesto) ──────────────
        [MaxLength(100)]
        [Display(Name = "Codice Cespite (SAP)")]
        public string? SapId { get; set; }

        [MaxLength(500)]
        [Display(Name = "Codice Terminale (SIA)")]
        public string? SiapDescrizione { get; set; }

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

        // ─── FLAGS TECNICI ───────────────────────────────────────────────
        [Display(Name = "Collegata in Rete (LAN)")]
        public bool LanCollegata { get; set; } = false;

        [Display(Name = "Software di monitoraggio dose installato")]
        public bool MedsquareInstallato { get; set; } = false;

        // ─── COLLOCAZIONE ────────────────────────────────────────────────
        [Display(Name = "Locale")]
        public int? LocaleId { get; set; }
        public Locale? Locale { get; set; }
       
[Required(ErrorMessage = "Sito / Presidio è obbligatorio")]
[Range(1, int.MaxValue, ErrorMessage = "Seleziona un sito valido")]
public int SitoId { get; set; }

[Required(ErrorMessage = "Edificio è obbligatorio")]
[Range(1, int.MaxValue, ErrorMessage = "Seleziona un edificio valido")]
public int ImmobileId { get; set; }

[Required(ErrorMessage = "Il piano è obbligatorio")]
[Range(1, int.MaxValue, ErrorMessage = "Seleziona un piano valido")]
public int PianoId { get; set; }


        // ─── COLLOCAZIONE PROVVISORIA ────────────────────────────────────
        // Popolata quando l'apparecchiatura viene temporaneamente spostata
        // rispetto alla collocazione ufficiale sopra. Nullable: assente se
        // non c'è alcun movimento provvisorio attivo.
        [Display(Name = "Sito Provvisorio")]
        public int? SitoProvvisorioId { get; set; }

        [Display(Name = "Edificio Provvisorio")]
        public int? ImmobileProvvisorioId { get; set; }

        [Display(Name = "Piano Provvisorio")]
        public int? PianoProvvisorioId { get; set; }

        [Display(Name = "Locale Provvisorio")]
        public int? LocaleProvvisorioId { get; set; }
        public Locale? LocaleProvvisorio { get; set; }

        [Display(Name = "Data Inizio Collocazione Provvisoria")]
        [DataType(DataType.Date)]
        public DateTime? DataInizioProvvisoria { get; set; }

        [Display(Name = "Struttura")]
        public int? RepartoId { get; set; }
        public Reparto? Reparto { get; set; }

        /// <summary>Dipartimento (FK → tabella Dipartimenti)</summary>
        [Display(Name = "Dipartimento")]
        public int? DipartimentoId { get; set; }
        public Dipartimento? Dipartimento { get; set; }

        // ─── RESPONSABILI ────────────────────────────────────────────────
        // Direttore Dipartimento
        [MaxLength(255)]
        [Display(Name = "Direttore Dipartimento")]
        public string? DirettoreDipartimento { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email Direttore Dipartimento")]
        public string? DirettoreDipartimentoEmail { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "PEC Direttore Dipartimento")]
        public string? DirettoreDipartimentoEmailPec { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telefono Direttore Dipartimento")]
        public string? DirettoreDipartimentoTelefono { get; set; }

        // Direttore Struttura
        [MaxLength(255)]
        [Display(Name = "Direttore Struttura")]
        public string? DirettoreStruttura { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email Direttore Struttura")]
        public string? DirettoreStrutturaEmail { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "PEC Direttore Struttura")]
        public string? DirettoreStrutturaEmailPec { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telefono Direttore Struttura")]
        public string? DirettoreStrutturaTelefono { get; set; }

        // Preposto D.Lgs 101/20 (ex Caposala)
        [MaxLength(255)]
        [Display(Name = "Preposto D. Lgs 101/20")]
        public string? Caposala { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email Preposto")]
        public string? PrepostoEmail { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "PEC Preposto")]
        public string? PrepostoEmailPec { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telefono Preposto")]
        public string? PrepostoTelefono { get; set; }

        // Responsabile Impianto Radiologico (RIR)
        [MaxLength(255)]
        [Display(Name = "Responsabile Impianto Radiologico (RIR)")]
        public string? RirNome { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email RIR")]
        public string? RirEmail { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "PEC RIR")]
        public string? RirEmailPec { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telefono RIR")]
        public string? RirTelefono { get; set; }

        // Specialista in Fisica Medica (SFM)
        [MaxLength(255)]
        [Display(Name = "Specialista in Fisica Medica (SFM)")]
        public string? SfmNome { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email SFM")]
        public string? SfmEmail { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "PEC SFM")]
        public string? SfmEmailPec { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telefono SFM")]
        public string? SfmTelefono { get; set; }

        // Esperto di Radioprotezione (EdR)
        [MaxLength(255)]
        [Display(Name = "Esperto di Radioprotezione (EdR)")]
        public string? EdrNome { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email EdR")]
        public string? EdrEmail { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "PEC EdR")]
        public string? EdrEmailPec { get; set; }

        [MaxLength(50)]
        [Display(Name = "Telefono EdR")]
        public string? EdrTelefono { get; set; }

        // ─── PLANIMETRIA E ZONE ──────────────────────────────────────────
        [MaxLength(1000)]
        [Display(Name = "Descrizione Zone Classificate")]
        public string? DescrizioneZoneClassificate { get; set; }

        /// <summary>Nome file piantina zone classificate (salvato in wwwroot/uploads/)</summary>
        [MaxLength(500)]
        [Display(Name = "Piantina Zone Classificate")]
        public string? PiantinaZoneClassificateFile { get; set; }

        /// <summary>Nome originale del file piantina, per visualizzazione in UI</summary>
        [MaxLength(255)]
        public string? PiantinaZoneClassificateNomeOriginale { get; set; }

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

        // ─── ADEMPIMENTI INAIL ───────────────────────────────────────────
        [Display(Name = "Stato Registrazione INAIL")]
        public StatoAdempimento StatoInail { get; set; } = StatoAdempimento.DaRegistrare;

        [Display(Name = "Data Registrazione INAIL")]
        [DataType(DataType.Date)]
        public DateTime? DataRegistrazioneInail { get; set; }

        [MaxLength(100)]
        [Display(Name = "Numero Pratica INAIL")]
        public string? NumeroPraticaInail { get; set; }

        /// <summary>File ricevuta registrazione INAIL</summary>
        [MaxLength(500)]
        [Display(Name = "Ricevuta Registrazione INAIL")]
        public string? InailRicevutaRegistrazioneFile { get; set; }

        /// <summary>Nome originale del file ricevuta registrazione INAIL, per visualizzazione in UI</summary>
        [MaxLength(255)]
        public string? InailRicevutaRegistrazioneNomeOriginale { get; set; }

        /// <summary>File ricevuta cessazione INAIL</summary>
        [MaxLength(500)]
        [Display(Name = "Ricevuta Cessazione INAIL")]
        public string? InailRicevutaCessioneFile { get; set; }

        /// <summary>Nome originale del file ricevuta cessazione INAIL, per visualizzazione in UI</summary>
        [MaxLength(255)]
        public string? InailRicevutaCessioneNomeOriginale { get; set; }

        // ─── ADEMPIMENTI STRIMS ──────────────────────────────────────────
        [Display(Name = "Stato Registrazione STRIMS")]
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

        /// <summary>File ricevuta registrazione STRIMS</summary>
        [MaxLength(500)]
        [Display(Name = "Ricevuta Registrazione STRIMS")]
        public string? StrimsRicevutaRegistrazioneFile { get; set; }

        /// <summary>Nome originale del file ricevuta registrazione STRIMS, per visualizzazione in UI</summary>
        [MaxLength(255)]
        public string? StrimsRicevutaRegistrazioneNomeOriginale { get; set; }

        /// <summary>File ricevuta cessazione STRIMS</summary>
        [MaxLength(500)]
        [Display(Name = "Ricevuta Cessazione STRIMS")]
        public string? StrimsRicevutaCessioneFile { get; set; }

        /// <summary>Nome originale del file ricevuta cessazione STRIMS, per visualizzazione in UI</summary>
        [MaxLength(255)]
        public string? StrimsRicevutaCessioneNomeOriginale { get; set; }

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
        public ICollection<CessazionePratica> CessazioniPratica { get; set; } = new List<CessazionePratica>();
        public ICollection<PrimaVerificaBenestare> PrimeVerificheBenestare { get; set; } = new List<PrimaVerificaBenestare>();
        public ICollection<NullaOsta> NullaOsta { get; set; } = new List<NullaOsta>();
        public ICollection<Verbale> Verbali { get; set; } = new List<Verbale>();
        public ICollection<StoricoCollocazione> StoricoCollocazioni { get; set; } = new List<StoricoCollocazione>();
    }
}