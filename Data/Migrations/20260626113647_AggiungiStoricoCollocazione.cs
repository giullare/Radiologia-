using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AggiungiStoricoCollocazione : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataInizioProvvisoria",
                table: "Apparecchiature",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImmobileProvvisorioId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocaleProvvisorioId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PianoProvvisorioId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SitoProvvisorioId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoricoCollocazioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    SitoId = table.Column<int>(type: "INTEGER", nullable: true),
                    ImmobileId = table.Column<int>(type: "INTEGER", nullable: true),
                    PianoId = table.Column<int>(type: "INTEGER", nullable: true),
                    LocaleId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataInizio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataFine = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoricoCollocazioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoricoCollocazioni_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoricoCollocazioni_Immobili_ImmobileId",
                        column: x => x.ImmobileId,
                        principalTable: "Immobili",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoricoCollocazioni_Locali_LocaleId",
                        column: x => x.LocaleId,
                        principalTable: "Locali",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoricoCollocazioni_Piani_PianoId",
                        column: x => x.PianoId,
                        principalTable: "Piani",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StoricoCollocazioni_Siti_SitoId",
                        column: x => x.SitoId,
                        principalTable: "Siti",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apparecchiature_LocaleProvvisorioId",
                table: "Apparecchiature",
                column: "LocaleProvvisorioId");

            migrationBuilder.CreateIndex(
                name: "IX_StoricoCollocazioni_ApparecchiaturaId",
                table: "StoricoCollocazioni",
                column: "ApparecchiaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_StoricoCollocazioni_ImmobileId",
                table: "StoricoCollocazioni",
                column: "ImmobileId");

            migrationBuilder.CreateIndex(
                name: "IX_StoricoCollocazioni_LocaleId",
                table: "StoricoCollocazioni",
                column: "LocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_StoricoCollocazioni_PianoId",
                table: "StoricoCollocazioni",
                column: "PianoId");

            migrationBuilder.CreateIndex(
                name: "IX_StoricoCollocazioni_SitoId",
                table: "StoricoCollocazioni",
                column: "SitoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apparecchiature_Locali_LocaleProvvisorioId",
                table: "Apparecchiature",
                column: "LocaleProvvisorioId",
                principalTable: "Locali",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apparecchiature_Locali_LocaleProvvisorioId",
                table: "Apparecchiature");

            migrationBuilder.DropTable(
                name: "StoricoCollocazioni");

            migrationBuilder.DropIndex(
                name: "IX_Apparecchiature_LocaleProvvisorioId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DataInizioProvvisoria",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "ImmobileProvvisorioId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "LocaleProvvisorioId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PianoProvvisorioId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "SitoProvvisorioId",
                table: "Apparecchiature");
        }
    }
}
