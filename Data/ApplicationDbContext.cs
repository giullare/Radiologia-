using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RadiologiaAppNew.Models;
using RadiologiaAppNew.Models.Collocazione;
using RadiologiaAppNew.Enums;

namespace RadiologiaAppNew.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ─── DBSET ──────────────────────────────────────────────────────
        // Collocazione
        public DbSet<Sito> Siti { get; set; }
        public DbSet<Immobile> Immobili { get; set; }
        public DbSet<Piano> Piani { get; set; }
        public DbSet<Locale> Locali { get; set; }
        public DbSet<Reparto> Reparti { get; set; }
        public DbSet<Dipartimento> Dipartimenti { get; set; }  // NUOVO

        // Apparecchiature
        public DbSet<Apparecchiatura> Apparecchiature { get; set; }
        public DbSet<FiguraResponsabile> FigureResponsabili { get; set; }
        public DbSet<FileAllegato> FileAllegati { get; set; }
        public DbSet<ProtocolloVerifica> ProtocolliVerifica { get; set; }
        public DbSet<RecordVerifica> RecordVerifiche { get; set; }
        public DbSet<NotificaPratica> NotifichePratica { get; set; }
        public DbSet<NullaOsta> NullaOsta { get; set; }
        public DbSet<CessazionePratica> CessazioniPratica { get; set; }
        public DbSet<PrimaVerificaBenestare> PrimeVerificheBenestare { get; set; }
        public DbSet<Verbale> Verbali { get; set; }

        // Anagrafica tecnica
        public DbSet<Costruttore> Costruttori { get; set; }
        public DbSet<ModelloApparecchiatura> ModelliApparecchiatura { get; set; }
        public DbSet<SocietaManutenzione> SocietaManutenzione { get; set; }

        // Lu177
        public DbSet<PazienteLu177> PazientiLu177 { get; set; }
        public DbSet<CicloTrattamento> CicliTrattamento { get; set; }
        public DbSet<DatoEmatologico> DatiEmatologici { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ─── COSTRUTTORI ─────────────────────────────────────────────
            builder.Entity<Costruttore>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(255);
                e.HasMany(x => x.Modelli)
                    .WithOne(x => x.Costruttore)
                    .HasForeignKey(x => x.CostrutoreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ModelloApparecchiatura>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(255);
            });

            // ─── SOCIETÀ MANUTENZIONE ────────────────────────────────────
            builder.Entity<SocietaManutenzione>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Nome).IsRequired().HasMaxLength(255);
            });

            // Seed costruttori comuni
            builder.Entity<Costruttore>().HasData(
                new Costruttore { Id = 1, Nome = "Siemens Healthineers", Paese = "Germania", Attivo = true },
                new Costruttore { Id = 2, Nome = "GE HealthCare", Paese = "USA", Attivo = true },
                new Costruttore { Id = 3, Nome = "Philips Healthcare", Paese = "Paesi Bassi", Attivo = true },
                new Costruttore { Id = 4, Nome = "Canon Medical Systems", Paese = "Giappone", Attivo = true },
                new Costruttore { Id = 5, Nome = "Hologic", Paese = "USA", Attivo = true },
                new Costruttore { Id = 6, Nome = "Varex Imaging", Paese = "USA", Attivo = true }
            );

            // ─── DIPARTIMENTO ────────────────────────────────────────────  NUOVO
            builder.Entity<Dipartimento>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(255);
                e.HasMany(x => x.Apparecchiature)
                    .WithOne(x => x.Dipartimento)
                    .HasForeignKey(x => x.DipartimentoId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ─── COLLOCAZIONE ────────────────────────────────────────────
            builder.Entity<Sito>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(255);
                e.HasMany(x => x.Immobili)
                    .WithOne(x => x.Sito)
                    .HasForeignKey(x => x.SitoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Immobile>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasMany(x => x.Piani)
                    .WithOne(x => x.Immobile)
                    .HasForeignKey(x => x.ImmobileId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Piano>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasMany(x => x.Locali)
                    .WithOne(x => x.Piano)
                    .HasForeignKey(x => x.PianoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Locale>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasMany(x => x.Apparecchiature)
                    .WithOne(x => x.Locale)
                    .HasForeignKey(x => x.LocaleId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Reparto>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasMany(x => x.Apparecchiature)
                    .WithOne(x => x.Reparto)
                    .HasForeignKey(x => x.RepartoId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // ─── APPARECCHIATURA ─────────────────────────────────────────
            builder.Entity<Apparecchiatura>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Codice).IsUnique();
                e.Property(x => x.Codice).IsRequired().HasMaxLength(100);
                e.Property(x => x.Descrizione).IsRequired().HasMaxLength(255);
                e.Property(x => x.Tipologia).IsRequired().HasMaxLength(100);
                e.Property(x => x.Modello).IsRequired().HasMaxLength(255);
                e.Property(x => x.Costruttore).IsRequired().HasMaxLength(255);
                e.Property(x => x.Matricola).IsRequired().HasMaxLength(100);

                e.HasMany(x => x.FigureResponsabili)
                    .WithOne(x => x.Apparecchiatura)
                    .HasForeignKey(x => x.ApparecchiaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.RecordVerifiche)
                    .WithOne(x => x.Apparecchiatura)
                    .HasForeignKey(x => x.ApparecchiaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.FileAllegati)
                    .WithOne(x => x.Apparecchiatura)
                    .HasForeignKey(x => x.ApparecchiaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.NotifichePratica)
                    .WithOne(x => x.Apparecchiatura)
                    .HasForeignKey(x => x.ApparecchiaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.NullaOsta)
                    .WithOne(x => x.Apparecchiatura)
                    .HasForeignKey(x => x.ApparecchiaturaId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(x => x.Verbali)
                    .WithOne(x => x.Apparecchiatura)
                    .HasForeignKey(x => x.ApparecchiaturaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ─── FIGURE RESPONSABILI ─────────────────────────────────────
            builder.Entity<FiguraResponsabile>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Nome).IsRequired().HasMaxLength(100);
                e.Property(x => x.Cognome).IsRequired().HasMaxLength(100);
            });

            // ─── PROTOCOLLO VERIFICA ─────────────────────────────────────
            builder.Entity<ProtocolloVerifica>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Codice).IsRequired().HasMaxLength(50);
                e.HasMany(x => x.RecordVerifiche)
                    .WithOne(x => x.Protocollo)
                    .HasForeignKey(x => x.ProtocolloId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ─── RECORD VERIFICA ─────────────────────────────────────────
            builder.Entity<RecordVerifica>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasMany(x => x.FileAllegati)
                    .WithOne(x => x.Verifica)
                    .HasForeignKey(x => x.VerificaId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ─── NOTIFICA PRATICA ────────────────────────────────────────
            builder.Entity<NotificaPratica>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            // ─── NULLA OSTA ──────────────────────────────────────────────
            builder.Entity<NullaOsta>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            // ─── VERBALE ─────────────────────────────────────────────────
            builder.Entity<Verbale>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            // ─── FILE ALLEGATO ───────────────────────────────────────────
            builder.Entity<FileAllegato>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.NomeOriginale).IsRequired().HasMaxLength(500);
                e.Property(x => x.NomeStorage).IsRequired().HasMaxLength(500);
                e.Property(x => x.Categoria).IsRequired().HasMaxLength(60);
            });

            // ─── PAZIENTE LU177 ──────────────────────────────────────────
            builder.Entity<PazienteLu177>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.CodicePaziente).IsUnique();
                e.Property(x => x.CodicePaziente).IsRequired().HasMaxLength(50);
                e.HasMany(x => x.CicliTrattamento)
                    .WithOne(x => x.Paziente)
                    .HasForeignKey(x => x.PazienteId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.DatiEmatologici)
                    .WithOne(x => x.Paziente)
                    .HasForeignKey(x => x.PazienteId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasMany(x => x.FileAllegati)
                    .WithOne(x => x.Paziente)
                    .HasForeignKey(x => x.PazienteId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ─── CICLO TRATTAMENTO ───────────────────────────────────────
            builder.Entity<CicloTrattamento>(e =>
            {
                e.HasKey(x => x.Id);
                e.Ignore(x => x.AttivitaEffettivaGbq);
                e.HasMany(x => x.FileAllegati)
                    .WithOne(x => x.Ciclo)
                    .HasForeignKey(x => x.CicloId)
                    .OnDelete(DeleteBehavior.NoAction);
                e.HasMany(x => x.DatiEmatologici)
                    .WithOne(x => x.Ciclo)
                    .HasForeignKey(x => x.CicloId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // ─── DATO EMATOLOGICO ────────────────────────────────────────
            builder.Entity<DatoEmatologico>(e =>
            {
                e.HasKey(x => x.Id);
            });

            // ─── IDENTITY — rinomina tabelle in italiano ─────────────────
            builder.Entity<ApplicationUser>().ToTable("Utenti");
            builder.Entity<IdentityRole>().ToTable("Ruoli");
            builder.Entity<IdentityUserRole<string>>().ToTable("UtentiRuoli");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UtentiClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UtentiLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RuoliClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UtentiToken");

            // ─── SEED DATI INIZIALI ──────────────────────────────────────
            builder.Entity<ProtocolloVerifica>().HasData(
                new ProtocolloVerifica { Id = 1, Codice = "PA-RAD-001", Descrizione = "Protocollo Accettazione — Radiologia Diagnostica", Tipo = TipoProtocollo.Accettazione, AmbitiApplicabilita = "Radiologia", Revisione = "Rev. 1", DataEntrataVigore = new DateTime(2024, 1, 1), PeriodicitaMesi = null, Attivo = true, CreatedAt = new DateTime(2024, 1, 1) },
                new ProtocolloVerifica { Id = 2, Codice = "PP-RAD-001", Descrizione = "Protocollo Periodico Annuale — Radiologia Diagnostica", Tipo = TipoProtocollo.Periodico, AmbitiApplicabilita = "Radiologia", Revisione = "Rev. 1", DataEntrataVigore = new DateTime(2024, 1, 1), PeriodicitaMesi = 12, Attivo = true, CreatedAt = new DateTime(2024, 1, 1) },
                new ProtocolloVerifica { Id = 3, Codice = "PA-TAC-001", Descrizione = "Protocollo Accettazione — TAC", Tipo = TipoProtocollo.Accettazione, AmbitiApplicabilita = "Radiologia", TipologieApplicabilita = "TAC", Revisione = "Rev. 1", DataEntrataVigore = new DateTime(2024, 1, 1), PeriodicitaMesi = null, Attivo = true, CreatedAt = new DateTime(2024, 1, 1) },
                new ProtocolloVerifica { Id = 4, Codice = "PP-RM-001", Descrizione = "Protocollo Periodico Semestrale — Risonanza Magnetica", Tipo = TipoProtocollo.Periodico, AmbitiApplicabilita = "RM", Revisione = "Rev. 1", DataEntrataVigore = new DateTime(2024, 1, 1), PeriodicitaMesi = 6, Attivo = true, CreatedAt = new DateTime(2024, 1, 1) },
                new ProtocolloVerifica { Id = 5, Codice = "PV-EDR-001", Descrizione = "Prima Verifica EDR — Radiologia", Tipo = TipoProtocollo.PrimaVerificaEdr, AmbitiApplicabilita = "Radiologia,RadiologiaInterventistica", Revisione = "Rev. 1", DataEntrataVigore = new DateTime(2024, 1, 1), PeriodicitaMesi = null, Attivo = true, CreatedAt = new DateTime(2024, 1, 1) },
                new ProtocolloVerifica { Id = 6, Codice = "LDR-RAD-001", Descrizione = "Verifica LDR Annuale — Radiologia", Tipo = TipoProtocollo.Ldr, AmbitiApplicabilita = "Radiologia", Revisione = "Rev. 1", DataEntrataVigore = new DateTime(2024, 1, 1), PeriodicitaMesi = 12, Attivo = true, CreatedAt = new DateTime(2024, 1, 1) }
            );

            builder.Entity<Sito>().HasData(new Sito { Id = 1, Nome = "Presidio Ospedaliero Principale", Indirizzo = "Via della Salute, 1" });
            builder.Entity<Immobile>().HasData(
                new Immobile { Id = 1, SitoId = 1, Nome = "Edificio A — Diagnostica" },
                new Immobile { Id = 2, SitoId = 1, Nome = "Edificio B — Terapia" }
            );
            builder.Entity<Piano>().HasData(
                new Piano { Id = 1, ImmobileId = 1, Nome = "Piano Terra", Numero = 0 },
                new Piano { Id = 2, ImmobileId = 1, Nome = "Piano 1", Numero = 1 },
                new Piano { Id = 3, ImmobileId = 2, Nome = "Piano Terra", Numero = 0 }
            );
            builder.Entity<Locale>().HasData(
                new Locale { Id = 1, PianoId = 1, Nome = "Sala TAC — 01", Codice = "TAC-01" },
                new Locale { Id = 2, PianoId = 1, Nome = "Sala RX — 02", Codice = "RX-02" },
                new Locale { Id = 3, PianoId = 2, Nome = "Sala RM — 10", Codice = "RM-10" },
                new Locale { Id = 4, PianoId = 3, Nome = "Sala PET-CT — 01", Codice = "PET-01" }
            );
            builder.Entity<Reparto>().HasData(
                new Reparto { Id = 1, Nome = "Radiologia", Responsabile = "Dr. Rossi" },
                new Reparto { Id = 2, Nome = "Medicina Nucleare", Responsabile = "Dr. Bianchi" },
                new Reparto { Id = 3, Nome = "Radioterapia", Responsabile = "Dr. Verdi" },
                new Reparto { Id = 4, Nome = "Risonanza Magnetica", Responsabile = "Dr. Neri" }
            );
        }
    }
}
