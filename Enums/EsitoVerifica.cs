using System.ComponentModel.DataAnnotations;

namespace RadiologiaAppNew.Enums
{
    public enum EsitoVerifica
    {
        [Display(Name = "In Corso")]
        InCorso,  

        [Display(Name = "Positivo")]
        Positivo,  

        [Display(Name = "Positivo con riserva")]
        PositivoConRiserva,  

        [Display(Name = "Negativo")]
        Negativo
    }
}