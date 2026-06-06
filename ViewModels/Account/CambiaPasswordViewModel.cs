using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.ViewModels.Account
{
    public class CambiaPasswordViewModel
    {
        [Required(ErrorMessage = "Inserisci la password attuale")]
        [DataType(DataType.Password)]
        [Display(Name = "Password Attuale")]
        public string PasswordAttuale { get; set; } = string.Empty;

        [Required(ErrorMessage = "Inserisci la nuova password")]
        [MinLength(8, ErrorMessage = "Minimo 8 caratteri")]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova Password")]
        public string NuovaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conferma la nuova password")]
        [DataType(DataType.Password)]
        [Compare("NuovaPassword", ErrorMessage = "Le password non coincidono")]
        [Display(Name = "Conferma Nuova Password")]
        public string ConfermaPassword { get; set; } = string.Empty;
    }
}