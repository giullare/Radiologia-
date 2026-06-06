using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "UtentiToken");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "UtentiRuoli");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "UtentiLogin");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "UtentiClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "Ruoli");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "RuoliClaims");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "UtentiRuoli",
                newName: "IX_UtentiRuoli_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "UtentiLogin",
                newName: "IX_UtentiLogin_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "UtentiClaims",
                newName: "IX_UtentiClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "RuoliClaims",
                newName: "IX_RuoliClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtentiToken",
                table: "UtentiToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtentiRuoli",
                table: "UtentiRuoli",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtentiLogin",
                table: "UtentiLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtentiClaims",
                table: "UtentiClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ruoli",
                table: "Ruoli",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RuoliClaims",
                table: "RuoliClaims",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PazientiLu177",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CodicePaziente = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Cognome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataNascita = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sesso = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    CodiceFiscale = table.Column<string>(type: "TEXT", maxLength: 16, nullable: true),
                    NumeroNosologico = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MedicoInviante = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    RepartoInviante = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DiagnosiPrincipale = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Indicazione = table.Column<string>(type: "TEXT", nullable: false),
                    DataPrimaVisita = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StatoPaziente = table.Column<int>(type: "INTEGER", nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    AltezzaCm = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    EgfrMlMin = table.Column<decimal>(type: "decimal(6,1)", nullable: true),
                    FunzionalitaEpatica = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Controindicazioni = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ProtocolloTerapeutico = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NCicliPianificati = table.Column<int>(type: "INTEGER", nullable: true),
                    AttivitaPerCicloGbq = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IntervallSettimane = table.Column<int>(type: "INTEGER", nullable: true),
                    DataInizioTrattamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PazientiLu177", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProtocolliVerifica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codice = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    AmbitiApplicabilita = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TipologieApplicabilita = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Revisione = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DataEntrataVigore = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PeriodicitaMesi = table.Column<int>(type: "INTEGER", nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtocolliVerifica", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reparti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Responsabile = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reparti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Indirizzo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utenti",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cognome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TelefonoInterno = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    UltimoAccesso = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CicliTrattamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroCiclo = table.Column<int>(type: "INTEGER", nullable: false),
                    DataSomministrazione = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OraSomministrazione = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    AttivitaSomministrataGbq = table.Column<decimal>(type: "decimal(6,3)", nullable: false),
                    AttivitaResiduaSiringaGbq = table.Column<decimal>(type: "decimal(6,3)", nullable: false),
                    LottoRadiofarmaco = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FornitoreRadiofarmaco = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PesoPazienteCicloKg = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    InfermiereSomministratore = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    NoteCliniche = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    EffettiAvversi = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    EsitoCiclo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MotivoModifica = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    PazienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CicliTrattamento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CicliTrattamento_PazientiLu177_PazienteId",
                        column: x => x.PazienteId,
                        principalTable: "PazientiLu177",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Immobili",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    SitoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Immobili", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Immobili_Siti_SitoId",
                        column: x => x.SitoId,
                        principalTable: "Siti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatiEmatologici",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataPrelievo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Timing = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    WbcX10_9L = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    HgbGdl = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    PltX10_9L = table.Column<decimal>(type: "decimal(7,1)", nullable: true),
                    NeutrofiliX10_9L = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    CreatininaMgDl = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    EgfrMlMin = table.Column<decimal>(type: "decimal(6,1)", nullable: true),
                    PsaNgMl = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    CromograninaA = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TossicitaEmatologica = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    PazienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    CicloId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatiEmatologici", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DatiEmatologici_CicliTrattamento_CicloId",
                        column: x => x.CicloId,
                        principalTable: "CicliTrattamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DatiEmatologici_PazientiLu177_PazienteId",
                        column: x => x.PazienteId,
                        principalTable: "PazientiLu177",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Piani",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Numero = table.Column<int>(type: "INTEGER", nullable: true),
                    ImmobileId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Piani", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Piani_Immobili_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobili",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Codice = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PianoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locali", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locali_Piani_PianoId",
                        column: x => x.PianoId,
                        principalTable: "Piani",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Apparecchiature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codice = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Modulo = table.Column<int>(type: "INTEGER", nullable: false),
                    AmbitoIntervento = table.Column<int>(type: "INTEGER", nullable: true),
                    Tipologia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Modello = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Costruttore = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Matricola = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CorrenteMaxMa = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TensioneMaxKvolt = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    EnergiaMaxKev = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    IntensitaCampoTesla = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TipoMagnete = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    LocaleId = table.Column<int>(type: "INTEGER", nullable: true),
                    RepartoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Caposala = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SocietaManutenzione = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TecnicoRiferimento = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    NumeroAssistenza = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    GlobalService = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    EmailAssistenza = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    LanCollegata = table.Column<bool>(type: "INTEGER", nullable: false),
                    MedsquareInstallato = table.Column<bool>(type: "INTEGER", nullable: false),
                    Stato = table.Column<int>(type: "INTEGER", nullable: false),
                    DataAccettazione = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataCessazione = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MotivoCessazione = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SapId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SiapDescrizione = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    StatoInail = table.Column<int>(type: "INTEGER", nullable: false),
                    DataRegistrazioneInail = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NumeroPraticaInail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    StatoStrims = table.Column<int>(type: "INTEGER", nullable: false),
                    StrimsIdApparecchiatura = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DataRegistrazioneStrims = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StrimsNpCaricata = table.Column<bool>(type: "INTEGER", nullable: false),
                    StrimsNcCaricata = table.Column<bool>(type: "INTEGER", nullable: false),
                    DescrizioneZoneClassificate = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apparecchiature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apparecchiature_Locali_LocaleId",
                        column: x => x.LocaleId,
                        principalTable: "Locali",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Apparecchiature_Reparti_RepartoId",
                        column: x => x.RepartoId,
                        principalTable: "Reparti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FigureResponsabili",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ruolo = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cognome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    GradoAbilitazione = table.Column<string>(type: "TEXT", maxLength: 1, nullable: true),
                    ValidoDal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidoAl = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FigureResponsabili", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FigureResponsabili_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotichePratica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroProtocolloPec = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataNotifica = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnteDestinatario = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    InviatoRspp = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataInvioRspp = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotichePratica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotichePratica_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NullaOsta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataRilascio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnteRilascio = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataScadenza = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Stato = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NullaOsta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NullaOsta_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordVerifiche",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInizio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFine = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Esito = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Anno = table.Column<int>(type: "INTEGER", nullable: true),
                    Semestre = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    ProssimaVerificaData = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InfoGuasto = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    TipoGuasto = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    BenestareQualitaTecnicaData = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BenestareQualitaTecnicaBy = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    BenestareCliniciData = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BenestareClinicoBy = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TipoInterventoManutenzione = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TecnicoManutentore = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SocietaManutenzione = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    DataInterventoManutenzione = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProtocolloId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordVerifiche", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordVerifiche_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordVerifiche_ProtocolliVerifica_ProtocolloId",
                        column: x => x.ProtocolloId,
                        principalTable: "ProtocolliVerifica",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Verbali",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataSopralluogo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Partecipanti = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Oggetto = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Rilievi = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    NonConformita = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    AzioniCorrettive = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ScadenzaAzioni = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Stato = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DataChiusura = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verbali", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Verbali_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileAllegati",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeOriginale = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    NomeStorage = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    DimensioneBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Descrizione = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Versione = table.Column<int>(type: "INTEGER", nullable: false),
                    SostituisceFileId = table.Column<int>(type: "INTEGER", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UploadedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: true),
                    VerificaId = table.Column<int>(type: "INTEGER", nullable: true),
                    VerbaleId = table.Column<int>(type: "INTEGER", nullable: true),
                    PazienteId = table.Column<int>(type: "INTEGER", nullable: true),
                    CicloId = table.Column<int>(type: "INTEGER", nullable: true),
                    DatoEmatologicoId = table.Column<int>(type: "INTEGER", nullable: true),
                    NotificaPraticaId = table.Column<int>(type: "INTEGER", nullable: true),
                    NullaOstaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAllegati", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAllegati_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileAllegati_CicliTrattamento_CicloId",
                        column: x => x.CicloId,
                        principalTable: "CicliTrattamento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAllegati_DatiEmatologici_DatoEmatologicoId",
                        column: x => x.DatoEmatologicoId,
                        principalTable: "DatiEmatologici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAllegati_NotichePratica_NotificaPraticaId",
                        column: x => x.NotificaPraticaId,
                        principalTable: "NotichePratica",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAllegati_NullaOsta_NullaOstaId",
                        column: x => x.NullaOstaId,
                        principalTable: "NullaOsta",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAllegati_PazientiLu177_PazienteId",
                        column: x => x.PazienteId,
                        principalTable: "PazientiLu177",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAllegati_RecordVerifiche_VerificaId",
                        column: x => x.VerificaId,
                        principalTable: "RecordVerifiche",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FileAllegati_Verbali_VerbaleId",
                        column: x => x.VerbaleId,
                        principalTable: "Verbali",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "ProtocolliVerifica",
                columns: new[] { "Id", "AmbitiApplicabilita", "Attivo", "Codice", "CreatedAt", "DataEntrataVigore", "Descrizione", "PeriodicitaMesi", "Revisione", "Tipo", "TipologieApplicabilita" },
                values: new object[,]
                {
                    { 1, "Radiologia", true, "PA-RAD-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Protocollo Accettazione — Radiologia Diagnostica", null, "Rev. 1", 0, null },
                    { 2, "Radiologia", true, "PP-RAD-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Protocollo Periodico Annuale — Radiologia Diagnostica", 12, "Rev. 1", 1, null },
                    { 3, "Radiologia", true, "PA-TAC-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Protocollo Accettazione — TAC", null, "Rev. 1", 0, "TAC" },
                    { 4, "RM", true, "PP-RM-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Protocollo Periodico Semestrale — Risonanza Magnetica", 6, "Rev. 1", 1, null },
                    { 5, "Radiologia,RadiologiaInterventistica", true, "PV-EDR-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Prima Verifica EDR — Radiologia", null, "Rev. 1", 4, null },
                    { 6, "Radiologia", true, "LDR-RAD-001", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Verifica LDR Annuale — Radiologia", 12, "Rev. 1", 3, null }
                });

            migrationBuilder.InsertData(
                table: "Reparti",
                columns: new[] { "Id", "Email", "Nome", "Responsabile" },
                values: new object[,]
                {
                    { 1, null, "Radiologia", "Dr. Rossi" },
                    { 2, null, "Medicina Nucleare", "Dr. Bianchi" },
                    { 3, null, "Radioterapia", "Dr. Verdi" },
                    { 4, null, "Risonanza Magnetica", "Dr. Neri" }
                });

            migrationBuilder.InsertData(
                table: "Siti",
                columns: new[] { "Id", "Indirizzo", "Nome" },
                values: new object[] { 1, "Via della Salute, 1", "Presidio Ospedaliero Principale" });

            migrationBuilder.InsertData(
                table: "Immobili",
                columns: new[] { "Id", "Nome", "SitoId" },
                values: new object[,]
                {
                    { 1, "Edificio A — Diagnostica", 1 },
                    { 2, "Edificio B — Terapia", 1 }
                });

            migrationBuilder.InsertData(
                table: "Piani",
                columns: new[] { "Id", "ImmobileId", "Nome", "Numero" },
                values: new object[,]
                {
                    { 1, 1, "Piano Terra", 0 },
                    { 2, 1, "Piano 1", 1 },
                    { 3, 2, "Piano Terra", 0 }
                });

            migrationBuilder.InsertData(
                table: "Locali",
                columns: new[] { "Id", "Codice", "Nome", "PianoId" },
                values: new object[,]
                {
                    { 1, "TAC-01", "Sala TAC — 01", 1 },
                    { 2, "RX-02", "Sala RX — 02", 1 },
                    { 3, "RM-10", "Sala RM — 10", 2 },
                    { 4, "PET-01", "Sala PET-CT — 01", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apparecchiature_Codice",
                table: "Apparecchiature",
                column: "Codice",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Apparecchiature_LocaleId",
                table: "Apparecchiature",
                column: "LocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Apparecchiature_RepartoId",
                table: "Apparecchiature",
                column: "RepartoId");

            migrationBuilder.CreateIndex(
                name: "IX_CicliTrattamento_PazienteId",
                table: "CicliTrattamento",
                column: "PazienteId");

            migrationBuilder.CreateIndex(
                name: "IX_DatiEmatologici_CicloId",
                table: "DatiEmatologici",
                column: "CicloId");

            migrationBuilder.CreateIndex(
                name: "IX_DatiEmatologici_PazienteId",
                table: "DatiEmatologici",
                column: "PazienteId");

            migrationBuilder.CreateIndex(
                name: "IX_FigureResponsabili_ApparecchiaturaId",
                table: "FigureResponsabili",
                column: "ApparecchiaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_ApparecchiaturaId",
                table: "FileAllegati",
                column: "ApparecchiaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_CicloId",
                table: "FileAllegati",
                column: "CicloId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_DatoEmatologicoId",
                table: "FileAllegati",
                column: "DatoEmatologicoId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_NotificaPraticaId",
                table: "FileAllegati",
                column: "NotificaPraticaId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_NullaOstaId",
                table: "FileAllegati",
                column: "NullaOstaId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_PazienteId",
                table: "FileAllegati",
                column: "PazienteId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_VerbaleId",
                table: "FileAllegati",
                column: "VerbaleId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_VerificaId",
                table: "FileAllegati",
                column: "VerificaId");

            migrationBuilder.CreateIndex(
                name: "IX_Immobili_SitoId",
                table: "Immobili",
                column: "SitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Locali_PianoId",
                table: "Locali",
                column: "PianoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotichePratica_ApparecchiaturaId",
                table: "NotichePratica",
                column: "ApparecchiaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_NullaOsta_ApparecchiaturaId",
                table: "NullaOsta",
                column: "ApparecchiaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_PazientiLu177_CodicePaziente",
                table: "PazientiLu177",
                column: "CodicePaziente",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Piani_ImmobileId",
                table: "Piani",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordVerifiche_ApparecchiaturaId",
                table: "RecordVerifiche",
                column: "ApparecchiaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordVerifiche_ProtocolloId",
                table: "RecordVerifiche",
                column: "ProtocolloId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Utenti",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Utenti",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Verbali_ApparecchiaturaId",
                table: "Verbali",
                column: "ApparecchiaturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuoliClaims_Ruoli_RoleId",
                table: "RuoliClaims",
                column: "RoleId",
                principalTable: "Ruoli",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtentiClaims_Utenti_UserId",
                table: "UtentiClaims",
                column: "UserId",
                principalTable: "Utenti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtentiLogin_Utenti_UserId",
                table: "UtentiLogin",
                column: "UserId",
                principalTable: "Utenti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtentiRuoli_Ruoli_RoleId",
                table: "UtentiRuoli",
                column: "RoleId",
                principalTable: "Ruoli",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtentiRuoli_Utenti_UserId",
                table: "UtentiRuoli",
                column: "UserId",
                principalTable: "Utenti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtentiToken_Utenti_UserId",
                table: "UtentiToken",
                column: "UserId",
                principalTable: "Utenti",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuoliClaims_Ruoli_RoleId",
                table: "RuoliClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UtentiClaims_Utenti_UserId",
                table: "UtentiClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UtentiLogin_Utenti_UserId",
                table: "UtentiLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UtentiRuoli_Ruoli_RoleId",
                table: "UtentiRuoli");

            migrationBuilder.DropForeignKey(
                name: "FK_UtentiRuoli_Utenti_UserId",
                table: "UtentiRuoli");

            migrationBuilder.DropForeignKey(
                name: "FK_UtentiToken_Utenti_UserId",
                table: "UtentiToken");

            migrationBuilder.DropTable(
                name: "FigureResponsabili");

            migrationBuilder.DropTable(
                name: "FileAllegati");

            migrationBuilder.DropTable(
                name: "Utenti");

            migrationBuilder.DropTable(
                name: "DatiEmatologici");

            migrationBuilder.DropTable(
                name: "NotichePratica");

            migrationBuilder.DropTable(
                name: "NullaOsta");

            migrationBuilder.DropTable(
                name: "RecordVerifiche");

            migrationBuilder.DropTable(
                name: "Verbali");

            migrationBuilder.DropTable(
                name: "CicliTrattamento");

            migrationBuilder.DropTable(
                name: "ProtocolliVerifica");

            migrationBuilder.DropTable(
                name: "Apparecchiature");

            migrationBuilder.DropTable(
                name: "PazientiLu177");

            migrationBuilder.DropTable(
                name: "Locali");

            migrationBuilder.DropTable(
                name: "Reparti");

            migrationBuilder.DropTable(
                name: "Piani");

            migrationBuilder.DropTable(
                name: "Immobili");

            migrationBuilder.DropTable(
                name: "Siti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtentiToken",
                table: "UtentiToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtentiRuoli",
                table: "UtentiRuoli");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtentiLogin",
                table: "UtentiLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtentiClaims",
                table: "UtentiClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RuoliClaims",
                table: "RuoliClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ruoli",
                table: "Ruoli");

            migrationBuilder.RenameTable(
                name: "UtentiToken",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "UtentiRuoli",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "UtentiLogin",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "UtentiClaims",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "RuoliClaims",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "Ruoli",
                newName: "AspNetRoles");

            migrationBuilder.RenameIndex(
                name: "IX_UtentiRuoli_RoleId",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UtentiLogin_UserId",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UtentiClaims_UserId",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RuoliClaims_RoleId",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
