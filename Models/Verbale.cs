using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class Verbale
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data Sopralluogo")]
        [DataType(DataType.Date)]
        public DateTime DataSopralluogo { get; set; } = DateTime.Today;

        [MaxLength(500)]
        [Display(Name = "Partecipanti")]
        public string? Partecipanti { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Oggetto")]
        public string Oggetto { get; set; } = string.Empty;

        [Required]
        [MaxLength(5000)]
        [Display(Name = "Rilievi e Osservazioni")]
        public string Rilievi { get; set; } = string.Empty;

        [MaxLength(2000)]
        [Display(Name = "Non Conformità Rilevate")]
        public string? NonConformita { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Azioni Correttive")]
        public string? AzioniCorrettive { get; set; }

        [Display(Name = "Scadenza Azioni Correttive")]
        [DataType(DataType.Date)]
        public DateTime? ScadenzaAzioni { get; set; }

        [MaxLength(20)]
        [Display(Name = "Stato")]
        public string Stato { get; set; } = "Aperto";

        [Display(Name = "Data Chiusura")]
        [DataType(DataType.Date)]
        public DateTime? DataChiusura { get; set; }

        [Display(Name = "Data Creazione")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        // ─── SEZIONE SPECIFICHE SOPRALLUOGO ─────────────────────────────

        // Campi generali
        [Display(Name = "Presenza dosimetro ambientale")]
        public bool? PresenzaDosimetroAmbientale { get; set; }

        [Display(Name = "Correttezza dosimetro ambientale (nome/periodo/posizionamento)")]
        public bool? CorrettezzaDosimetroAmbientale { get; set; }

        [Display(Name = "Presenza norme di radioprotezione D.Lgs. 101/20")]
        public bool? PresenzaNormeRadioprotezione { get; set; }

        [Display(Name = "Piantina")]
        public bool? Piantina { get; set; }

        // ─── SALA CONSOLLE ───────────────────────────────────────────────
        [Display(Name = "Segnaletica Consolle")]
        public bool? SegnaleticaConsolle { get; set; }

        [Display(Name = "Classificazione Consolle")]
        public bool? ClassificazioneConsolle { get; set; }

        [Display(Name = "Funzionamento Segnaletica Luminosa Consolle")]
        public bool? FunzionamentoSegnaleticaLuminosaConsolle { get; set; }

        [Display(Name = "Segnaletica donne in gravidanza (consolle)")]
        public bool? SegnaleticaGravidanzaConsolle { get; set; }

        [Display(Name = "Inter Lock porta/fascio funzionante Sala diagnostica")]
        public bool? InterLockConsolle { get; set; }

        // ─── SALA DIAGNOSTICA ────────────────────────────────────────────
        [Display(Name = "Segnaletica sala diagnostica")]
        public bool? SegnaleticaSalaDiagnostica { get; set; }

        [Display(Name = "Classificazione sala diagnostica")]
        public bool? ClassificazioneSalaDiagnostica { get; set; }

        [Display(Name = "Funzionamento Segnaletica Luminosa (sala diagnostica)")]
        public bool? FunzionamentoSegnaleticaLuminosaDiagnostica { get; set; }

        [Display(Name = "Segnaletica donne in gravidanza (sala diagnostica)")]
        public bool? SegnaleticaGravidanzaDiagnostica { get; set; }

        // ─── SALA PREPARAZIONE ───────────────────────────────────────────
        [Display(Name = "Segnaletica sala preparazione")]
        public bool? SegnaleticaSalaPreparazione { get; set; }

        [Display(Name = "Classificazione sala preparazione")]
        public bool? ClassificazioneSalaPreparazione { get; set; }

        [Display(Name = "Funzionamento Segnaletica Luminosa sala preparazione")]
        public bool? FunzionamentoSegnaleticaLuminosaPreparazione { get; set; }

        [Display(Name = "Segnaletica donne in gravidanza (sala preparazione)")]
        public bool? SegnaleticaGravidanzaPreparazione { get; set; }

        [Display(Name = "Inter Lock porta/fascio funzionante sala preparazione")]
        public bool? InterLockPreparazione { get; set; }

        // ─── PORTATILI RX - ARCO A C ─────────────────────────────────────
        [Display(Name = "Segnaletica rischio presenza di radiazioni")]
        public bool? SegnaleticaRischioRadiazioni { get; set; }

        [Display(Name = "Segnaletica rischio donne in gravidanza")]
        public bool? SegnaleticaRischioGravidanza { get; set; }

        [Display(Name = "Presenza Norme Radioprotezione D.Lgs 101/20")]
        public bool? PresenzaNormePortatili { get; set; }

        // ─── FK ──────────────────────────────────────────────────────────
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        // Navigation
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}