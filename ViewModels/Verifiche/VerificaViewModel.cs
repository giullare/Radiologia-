using RadiologiaAppNew.Enums;
using RadiologiaAppNew.Models;

namespace RadiologiaAppNew.ViewModels.Verifiche
{
    public class VerificaListViewModel
    {
        public List<VerificaRowViewModel> Verifiche { get; set; } = new();

        // Filtri
        public int?    ApparecchiaturaIdFiltro { get; set; }
        public string? TipoFiltro    { get; set; }
        public string? EsitoFiltro   { get; set; }
        public int?    AnnoFiltro    { get; set; }
        public string? SearchText    { get; set; }

        // Paginazione
        public int PaginaCorrente   { get; set; } = 1;
        public int TotalePagine     { get; set; }
        public int TotaleRisultati  { get; set; }

        // Dati per filtri
        public List<Apparecchiatura> Apparecchiature  { get; set; } = new();
        public List<int>             AnniDisponibili  { get; set; } = new();

        public bool HaFiltriAttivi =>
            ApparecchiaturaIdFiltro.HasValue ||
            !string.IsNullOrEmpty(TipoFiltro)  ||
            !string.IsNullOrEmpty(EsitoFiltro) ||
            AnnoFiltro.HasValue                ||
            !string.IsNullOrEmpty(SearchText);
    }

    public class VerificaRowViewModel
    {
        public int    Id               { get; set; }
        public string Apparecchiatura  { get; set; } = string.Empty;
        public int    ApparecchiaturaId { get; set; }
        public string Protocollo       { get; set; } = string.Empty;
        public TipoProtocollo    Tipo   { get; set; }
        public DateTime          DataInizio { get; set; }
        public DateTime?         DataFine   { get; set; }
        public EsitoVerifica     Esito      { get; set; }
        public int?              Anno       { get; set; }
        public string?           Semestre   { get; set; }
        public DateTime?         ProssimaVerificaData { get; set; }
        public int               NumeroAllegati       { get; set; }
        public bool              HaGuasto => !string.IsNullOrEmpty(InfoGuasto);
        public string?           InfoGuasto { get; set; }
    }

    public class VerificaCreateViewModel
    {
        public int    ApparecchiaturaId          { get; set; }
        public string ApparecchiaturaDescrizione { get; set; } = string.Empty;
        public string ApparecchiaturaCodice      { get; set; } = string.Empty;

        // ── Tipo e Protocollo ────────────────────────────────────────────
        /// <summary>
        /// Valori UI: Accettazione | FunzionamentoPeriodico |
        /// FunzionamentoPostManutenzione | LDR
        /// (mappato a TipoProtocollo nell'enum esistente)
        /// </summary>
        public string TipoUI { get; set; } = "FunzionamentoPeriodico";

        public int     ProtocolloId { get; set; }
        public TipoProtocollo Tipo  { get; set; } = TipoProtocollo.Periodico;

        // ── Date ed Esito ────────────────────────────────────────────────
        public DateTime  DataInizio           { get; set; } = DateTime.Today;
        // DataFine RIMOSSA come richiesto

        /// <summary>
        /// Valori UI: InCorso | Positivo | PositivoConRiserva | Negativo
        /// </summary>
        public string EsitoUI { get; set; } = "InCorso";
        public EsitoVerifica Esito { get; set; } = EsitoVerifica.InCorso;

        public int?      Anno                { get; set; } = DateTime.Today.Year;
        public string?   Semestre            { get; set; }
        public DateTime? ProssimaVerificaData { get; set; }

        // ── Periodicità ──────────────────────────────────────────────────
        /// <summary>
        /// Valori: Mensile | Trimestrale | Semestrale | Annuale | Biennale | Altro
        /// </summary>
        public string? Periodicita         { get; set; }
        /// <summary>
        /// Mesi specifici se Periodicita = Altro
        /// </summary>
        public int?    PeriodicitaAltroMesi { get; set; }

        // ── Note ─────────────────────────────────────────────────────────
        public string? Note { get; set; }

        // ── Guasto ───────────────────────────────────────────────────────
        public string? InfoGuasto  { get; set; }
        public string? TipoGuasto  { get; set; }

        // ── Benestare (per Accettazione) ─────────────────────────────────
        public DateTime? BenestareQualitaTecnicaData { get; set; }
        public string?   BenestareQualitaTecnicaBy   { get; set; }
        public DateTime? BenestareCliniciData         { get; set; }
        public string?   BenestareClinicoBy           { get; set; }

        // ── Manutenzione ─────────────────────────────────────────────────
        public string?   TipoInterventoManutenzione  { get; set; }
        public string?   TecnicoManutentore           { get; set; }
        public string?   SocietaManutenzione          { get; set; }
        public DateTime? DataInterventoManutenzione   { get; set; }

        // ── Dati per il form ─────────────────────────────────────────────
        public List<ProtocolloVerifica> ProtocolliDisponibili { get; set; } = new();
    }

    public class VerificaDetailViewModel
    {
        public RecordVerifica  Verifica                 { get; set; } = null!;
        public string          ApparecchiaturaDescrizione { get; set; } = string.Empty;
        public List<FileAllegato> FileAllegati          { get; set; } = new();
        public ProtocolloVerifica? Protocollo           { get; set; }
        public int?            GiorniAllaScadenza       { get; set; }
    }

    // ── Mapping helper (usato nel controller) ────────────────────────────
    public static class TipoUIHelper
    {
        /// <summary>Mappa il valore UI al TipoProtocollo dell'enum</summary>
        public static TipoProtocollo ToTipoProtocollo(string tipoUI) => tipoUI switch
        {
            "Accettazione"                => TipoProtocollo.Accettazione,
            "FunzionamentoPeriodico"      => TipoProtocollo.Periodico,
            "FunzionamentoPostManutenzione" => TipoProtocollo.PostManutenzione,
            "LDR"                         => TipoProtocollo.Ldr,
            _                             => TipoProtocollo.Periodico
        };

        public static string ToTipoUI(TipoProtocollo tipo) => tipo switch
        {
            TipoProtocollo.Accettazione      => "Accettazione",
            TipoProtocollo.Periodico         => "FunzionamentoPeriodico",
            TipoProtocollo.PostManutenzione  => "FunzionamentoPostManutenzione",
            TipoProtocollo.Ldr               => "LDR",
            _                                => "FunzionamentoPeriodico"
        };

        /// <summary>Mappa il valore UI all'EsitoVerifica dell'enum</summary>
        public static EsitoVerifica ToEsitoVerifica(string esitoUI) => esitoUI switch
        {
            
                "InCorso"            => EsitoVerifica.InCorso,
                "Positivo"           => EsitoVerifica.Positivo,
                "PositivoConRiserva" => EsitoVerifica.PositivoConRiserva,
                "Negativo"           => EsitoVerifica.Negativo,
                _                    => EsitoVerifica.InCorso
  
        };

        public static string ToEsitoUI(EsitoVerifica esito) => esito switch
        {
            
                EsitoVerifica.InCorso            => "InCorso",
                EsitoVerifica.Positivo           => "Positivo",
                EsitoVerifica.PositivoConRiserva => "PositivoConRiserva",
                EsitoVerifica.Negativo           => "Negativo",
                _                                => "InCorso"

        };

        /// <summary>Mappa la periodicità UI ai mesi</summary>
        public static int? PeriodicitaToMesi(string? periodicita, int? altroMesi) => periodicita switch
        {
            "Mensile"     => 1,
            "Trimestrale" => 3,
            "Semestrale"  => 6,
            "Annuale"     => 12,
            "Biennale"    => 24,
            "Altro"       => altroMesi,
            _             => null
        };
    }
}