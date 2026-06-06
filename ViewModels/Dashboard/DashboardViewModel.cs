namespace RadiologiaAppNew.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // KPI principali
        public int TotaleApparecchiatureAttive { get; set; }
        public int ApparecchiatureInManutenzione { get; set; }
        public int VerificheCQScadute { get; set; }
        public int VerificheCQInScadenza30gg { get; set; }
        public int NullaOstaInScadenza { get; set; }
        public int AlertCriticiTotali { get; set; }
        public double PercentualeCompliance { get; set; }
        public int PazientiLu177Attivi { get; set; }

        // Scadenze imminenti
        public List<ScadenzaItem> ScadenzeImminenti { get; set; } = new();

        // Ultime verifiche
        public List<UltimaVerificaItem> UltimeVerifiche { get; set; } = new();

        // Dati grafici — CQ per mese
        public List<string> MesiLabels { get; set; } = new();
        public List<int> CQSuperati { get; set; } = new();
        public List<int> CQNonSuperati { get; set; } = new();

        // Dati grafici — distribuzione per ambito
        public int CountRadiologia { get; set; }
        public int CountInterventistica { get; set; }
        public int CountMedicinaNucleare { get; set; }
        public int CountRadioterapia { get; set; }
        public int CountRM { get; set; }
    }

    public class ScadenzaItem
    {
        public string Apparecchiatura { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public DateTime DataScadenza { get; set; }
        public string Priorita { get; set; } = "warning"; // ok, warning, danger
        public int ApparecchiaturaId { get; set; }
        public int GiorniMancanti =>
            (DataScadenza.Date - DateTime.Today).Days;
    }

    public class UltimaVerificaItem
    {
        public string Apparecchiatura { get; set; } = string.Empty;
        public string TipoVerifica { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Esito { get; set; } = string.Empty;
        public int ApparecchiaturaId { get; set; }
    }
}