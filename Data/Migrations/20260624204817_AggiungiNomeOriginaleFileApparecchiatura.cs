using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AggiungiNomeOriginaleFileApparecchiatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InailRicevutaCessioneNomeOriginale",
                table: "Apparecchiature",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InailRicevutaRegistrazioneNomeOriginale",
                table: "Apparecchiature",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PiantinaZoneClassificateNomeOriginale",
                table: "Apparecchiature",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrimsRicevutaCessioneNomeOriginale",
                table: "Apparecchiature",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrimsRicevutaRegistrazioneNomeOriginale",
                table: "Apparecchiature",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InailRicevutaCessioneNomeOriginale",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "InailRicevutaRegistrazioneNomeOriginale",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PiantinaZoneClassificateNomeOriginale",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "StrimsRicevutaCessioneNomeOriginale",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "StrimsRicevutaRegistrazioneNomeOriginale",
                table: "Apparecchiature");
        }
    }
}
