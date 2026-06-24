using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaVerificaBenestare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrimaVerificaBenestareId",
                table: "FileAllegati",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrimeVerificheBenestare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataVerifica = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EnteVerificatore = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ApparecchiaturaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimeVerificheBenestare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrimeVerificheBenestare_Apparecchiature_ApparecchiaturaId",
                        column: x => x.ApparecchiaturaId,
                        principalTable: "Apparecchiature",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAllegati_PrimaVerificaBenestareId",
                table: "FileAllegati",
                column: "PrimaVerificaBenestareId");

            migrationBuilder.CreateIndex(
                name: "IX_PrimeVerificheBenestare_ApparecchiaturaId",
                table: "PrimeVerificheBenestare",
                column: "ApparecchiaturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAllegati_PrimeVerificheBenestare_PrimaVerificaBenestareId",
                table: "FileAllegati",
                column: "PrimaVerificaBenestareId",
                principalTable: "PrimeVerificheBenestare",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAllegati_PrimeVerificheBenestare_PrimaVerificaBenestareId",
                table: "FileAllegati");

            migrationBuilder.DropTable(
                name: "PrimeVerificheBenestare");

            migrationBuilder.DropIndex(
                name: "IX_FileAllegati_PrimaVerificaBenestareId",
                table: "FileAllegati");

            migrationBuilder.DropColumn(
                name: "PrimaVerificaBenestareId",
                table: "FileAllegati");
        }
    }
}
