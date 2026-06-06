using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.ViewModels.Apparecchiature
{
    public class ApparecchiaturaListViewModel
    {
        public List<ApparecchiaturaRowViewModel> Apparecchiature { get; set; } = new();

        // Filtri attivi
        public string? SearchText { get; set; }
        public string? StatoFiltro { get; set; }
        public string? AmbitoFiltro { get; set; }
        public int? RepartoFiltro { get; set; }

        // Paginazione
        public int PaginaCorrente { get; set; } = 1;
        public int TotalePagine { get; set; }
        public int TotaleRisultati { get; set; }
        public int ElementiPerPagina { get; set; } = 20;

        // Dati per i filtri
        public List<Models.Collocazione.Reparto> Reparti { get; set; } = new();

        public bool HaFiltriAttivi =>
            !string.IsNullOrEmpty(SearchText) ||
            !string.IsNullOrEmpty(StatoFiltro) ||
            !string.IsNullOrEmpty(AmbitoFiltro) ||
            RepartoFiltro.HasValue;
    }

    public class ApparecchiaturaRowViewModel
    {
        public int Id { get; set; }
        public string Codice { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Tipologia { get; set; } = string.Empty;
        public string Costruttore { get; set; } = string.Empty;
        public string Modello { get; set; } = string.Empty;
        public string? Reparto { get; set; }
        public string? Locale { get; set; }
        public StatoApparecchiatura Stato { get; set; }
        public TipoModulo Modulo { get; set; }
        public AmbitoIntervento? AmbitoIntervento { get; set; }
        public DateTime? DataAccettazione { get; set; }
        public DateTime? ProssimaVerifica { get; set; }
        public string ComplianceStato { get; set; } = "missing";
        public int NumeroVerifiche { get; set; }
    }
}