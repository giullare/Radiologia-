using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.ViewModels.Verifiche
{
    public class VerificaListViewModel
    {
        public List<VerificaRowViewModel> Verifiche { get; set; } = new();

        // Filtri
        public int? ApparecchiaturaIdFiltro { get; set; }
        public string? TipoFiltro { get; set; }
        public string? EsitoFiltro { get; set; }
        public int? AnnoFiltro { get; set; }
        public string? SearchText { get; set; }

        // Paginazione
        public int PaginaCorrente { get; set; } = 1;
        public int TotalePagine { get; set; }
        public int TotaleRisultati { get; set; }

        // Dati per filtri
        public List<Apparecchiatura> Apparecchiature { get; set; } = new();
        public List<int> AnniDisponibili { get; set; } = new();

        public bool HaFiltriAttivi =>
            ApparecchiaturaIdFiltro.HasValue ||
            !string.IsNullOrEmpty(TipoFiltro) ||
            !string.IsNullOrEmpty(EsitoFiltro) ||
            AnnoFiltro.HasValue ||
            !string.IsNullOrEmpty(SearchText);
    }

    public class VerificaRowViewModel
    {
        public int Id { get; set; }
        public string Apparecchiatura { get; set; } = string.Empty;
        public int ApparecchiaturaId { get; set; }
        public string Protocollo { get; set; } = string.Empty;
        public TipoProtocollo Tipo { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        public EsitoVerifica Esito { get; set; }
        public int? Anno { get; set; }
        public string? Semestre { get; set; }
        public DateTime? ProssimaVerificaData { get; set; }
        public int NumeroAllegati { get; set; }
        public bool HaGuasto => !string.IsNullOrEmpty(InfoGuasto);
        public string? InfoGuasto { get; set; }
    }

    public class VerificaCreateViewModel
    {
        public int ApparecchiaturaId { get; set; }
        public string ApparecchiaturaDescrizione { get; set; } = string.Empty;
        public string ApparecchiaturaCodice { get; set; } = string.Empty;

        // Campi form
        public int ProtocolloId { get; set; }
        public TipoProtocollo Tipo { get; set; } = TipoProtocollo.Periodico;
        public DateTime DataInizio { get; set; } = DateTime.Today;
        public DateTime? DataFine { get; set; }
        public EsitoVerifica Esito { get; set; } = EsitoVerifica.InCorso;
        public string? Note { get; set; }
        public int? Anno { get; set; } = DateTime.Today.Year;
        public string? Semestre { get; set; }
        public DateTime? ProssimaVerificaData { get; set; }

        // Guasto
        public string? InfoGuasto { get; set; }
        public string? TipoGuasto { get; set; }

        // Benestare (per accettazione)
        public DateTime? BenestareQualitaTecnicaData { get; set; }
        public string? BenestareQualitaTecnicaBy { get; set; }
        public DateTime? BenestareCliniciData { get; set; }
        public string? BenestareClinicoBy { get; set; }

        // Manutenzione
        public string? TipoInterventoManutenzione { get; set; }
        public string? TecnicoManutentore { get; set; }
        public string? SocietaManutenzione { get; set; }
        public DateTime? DataInterventoManutenzione { get; set; }

        // Dati per il form
        public List<ProtocolloVerifica> ProtocolliDisponibili { get; set; } = new();
    }

    public class VerificaDetailViewModel
    {
        public RecordVerifica Verifica { get; set; } = null!;
        public string ApparecchiaturaDescrizione { get; set; } = string.Empty;
        public List<FileAllegato> FileAllegati { get; set; } = new();
        public ProtocolloVerifica? Protocollo { get; set; }
        public int? GiorniAllaScadenza { get; set; }
    }
}