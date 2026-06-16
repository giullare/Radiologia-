using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class Dipartimento
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        [Display(Name = "Nome Dipartimento")]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Descrizione")]
        public string? Descrizione { get; set; }

        [Display(Name = "Attivo")]
        public bool Attivo { get; set; } = true;

        // Navigation
        public ICollection<Apparecchiatura> Apparecchiature { get; set; } = new List<Apparecchiatura>();
    }
}