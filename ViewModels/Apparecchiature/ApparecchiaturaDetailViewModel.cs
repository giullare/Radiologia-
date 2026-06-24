using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.ViewModels.Apparecchiature
{
    public class ApparecchiaturaDetailViewModel
    {
        public Apparecchiatura Apparecchiatura { get; set; } = null!;

        // Figure responsabili per ruolo
        public List<FiguraResponsabile> FigureEFM { get; set; } = new();
        public List<FiguraResponsabile> FigureEDR { get; set; } = new();
        public List<FiguraResponsabile> FigureRIR { get; set; } = new();
        public List<FiguraResponsabile> FigureMA  { get; set; } = new();

        // Verifiche
        public List<RecordVerifica> VerificheCQ     { get; set; } = new();
        public List<RecordVerifica> VerificheEDR    { get; set; } = new();
        public RecordVerifica? UltimaAccettazione   { get; set; }

        // Adempimenti
        public List<NotificaPratica> NotichePratica { get; set; } = new();
        public List<CessazionePratica> CessazioniPratica { get; set; } = new();
        public List<PrimaVerificaBenestare> PrimeVerificheBenestare { get; set; } = new();

        public List<NullaOsta> NullaOsta            { get; set; } = new();
        public List<Verbale> Verbali                { get; set; } = new();

        // Documenti
        public List<FileAllegato> FileAllegati      { get; set; } = new();

        // Compliance items
        public List<ComplianceItemVm> ItemsCompliance { get; set; } = new();

        // Tab attiva
        public string TabAttiva { get; set; } = "anagrafica";
    }

    public class ComplianceItemVm
    {
        public string Label   { get; set; } = string.Empty;
        public string Stato   { get; set; } = "missing"; // ok, warning, danger, missing, todo
        public string Dettaglio { get; set; } = string.Empty;
        public string? Url    { get; set; }
        public string Icona   { get; set; } = "bi-circle";
    }
}