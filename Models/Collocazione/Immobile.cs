using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models.Collocazione
{
    public class Immobile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome dell'immobile è obbligatorio")]
        [MaxLength(255)]
        [Display(Name = "Nome Immobile / Edificio")]
        public string Nome { get; set; } = string.Empty;

        // FK
        public int SitoId { get; set; }
        public Sito Sito { get; set; } = null!;

        // Navigation
        public ICollection<Piano> Piani { get; set; } = new List<Piano>();
    }
}