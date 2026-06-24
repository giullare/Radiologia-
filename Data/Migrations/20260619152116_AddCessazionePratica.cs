using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCessazionePratica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CessazionePraticaId",
                table: "FileAllegati",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CessazioniPratica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroProtocolloPec = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    DataCessazione = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnteDestinatario = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CessazioniPratica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CessazioniPratica_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_CessazionePraticaId",
                table: "FileAllegati",
                column: "CessazionePraticaId");

            migrationBuilder.CreateIndex(
                name: "IX_CessazioniPratica_ApparecchiaturaId",
                table: "CessazioniPratica",
                column: "ApparecchiaturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAllegati_CessazioniPratica_CessazionePraticaId",
                table: "FileAllegati",
                column: "CessazionePraticaId",
                principalTable: "CessazioniPratica",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAllegati_CessazioniPratica_CessazionePraticaId",
                table: "FileAllegati");

            migrationBuilder.DropTable(
                name: "CessazioniPratica");

            migrationBuilder.DropIndex(
                name: "IX_FileAllegati_CessazionePraticaId",
                table: "FileAllegati");

            migrationBuilder.DropColumn(
                name: "CessazionePraticaId",
                table: "FileAllegati");
        }
    }
}
