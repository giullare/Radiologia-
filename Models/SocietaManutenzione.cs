using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class SocietaManutenzione
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Ragione Sociale")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(50)]
        [Display(Name = "Numero Assistenza Tecnica")]
        public string? NumeroAssistenza { get; set; }

        [MaxLength(50)]
        [Display(Name = "Numero Reperibilità")]
        public string? NumeroReperibilita { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "Email Assistenza")]
        public string? EmailAssistenza { get; set; }

        [MaxLength(255)]
        [Display(Name = "Contratto / Global Service")]
        public string? GlobalService { get; set; }

        [MaxLength(255)]
        [Display(Name = "Sito Web")]
        public string? SitoWeb { get; set; }

        [MaxLength(500)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        public bool Attivo { get; set; } = true;
    }
}