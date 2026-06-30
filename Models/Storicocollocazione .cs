using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models.Collocazione;

namespace RadiologiaAppNew.Models
{
    /// <summary>
    /// Riga di storico per ogni movimento di collocazione (ufficiale o provvisoria)
    /// di un'apparecchiatura. Una riga con DataFine nulla è "attiva".
    /// </summary>
    public class StoricoCollocazione
    {
        public int Id { get; set; }

        [Required]
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura? Apparecchiatura { get; set; }

        [Required]
        [Display(Name = "Tipo Collocazione")]
        public TipoCollocazione Tipo { get; set; } = TipoCollocazione.Ufficiale;

        // ─── Collocazione (Sito → Edificio → Piano → Locale) ──────────────
        [Display(Name = "Sito / Presidio")]
        public int? SitoId { get; set; }
        public Sito? Sito { get; set; }

        [Display(Name = "Edificio")]
        public int? ImmobileId { get; set; }
        public Immobile? Immobile { get; set; }

        [Display(Name = "Piano")]
        public int? PianoId { get; set; }
        public Piano? Piano { get; set; }

        [Display(Name = "Locale / Stanza")]
        public int? LocaleId { get; set; }
        public Locale? Locale { get; set; }

        // ─── Periodo di validità ───────────────────────────────────────────
        [Required]
        [Display(Name = "Data Inizio")]
        [DataType(DataType.Date)]
        public DateTime DataInizio { get; set; } = DateTime.UtcNow;

        /// <summary>Nulla se la collocazione è ancora attiva.</summary>
        [Display(Name = "Data Fine")]
        [DataType(DataType.Date)]
        public DateTime? DataFine { get; set; }

        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        // ─── Tracciamento ───────────────────────────────────────────────────
        [Display(Name = "Registrato il")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }
    }
}