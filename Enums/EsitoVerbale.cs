using System.ComponentModel.DataAnnotations;
namespace RadiologiaAppNew.Models.Enums

{
    public enum EsitoVerbale
    {
       [Display(Name = "Positivo")]
        Positivo,  

        [Display(Name = "Positivo con riserva")]
        PositivoConRiserva,  

        [Display(Name = "Negativo")]
        Negativo
    }
}

