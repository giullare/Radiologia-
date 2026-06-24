using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Models
{
    public class PrimaVerificaBenestare
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Data Prima Verifica")]
        [DataType(DataType.Date)]
        public DateTime DataVerifica { get; set; } = DateTime.Today;

        [Required]
        [MaxLength(255)]
        [Display(Name = "Ente / Tecnico")]
        public string EnteVerificatore { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Note")]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int ApparecchiaturaId { get; set; }
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        // file
        public ICollection<FileAllegato> FileAllegati { get; set; } = new List<FileAllegato>();
    }
}