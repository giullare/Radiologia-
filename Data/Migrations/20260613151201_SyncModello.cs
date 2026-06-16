using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModello : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apparecchiature_Dipartimenti_DipartimentoId",
                table: "Apparecchiature");

            migrationBuilder.DropTable(
                name: "Dipartimenti");

            migrationBuilder.DropIndex(
                name: "IX_Apparecchiature_DipartimentoId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "ClassificazioneConsolle",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "ClassificazioneSalaDiagnostica",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "ClassificazioneSalaPreparazione",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "CorrettezzaDosimetroAmbientale",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "FunzionamentoSegnaleticaLuminosaConsolle",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "FunzionamentoSegnaleticaLuminosaDiagnostica",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "FunzionamentoSegnaleticaLuminosaPreparazione",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "InterLockConsolle",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "InterLockPreparazione",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "Piantina",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "PresenzaDosimetroAmbientale",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "PresenzaNormePortatili",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "PresenzaNormeRadioprotezione",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaConsolle",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaGravidanzaConsolle",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaGravidanzaDiagnostica",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaGravidanzaPreparazione",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaRischioGravidanza",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaRischioRadiazioni",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaSalaDiagnostica",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "SegnaleticaSalaPreparazione",
                table: "Verbali");

            migrationBuilder.DropColumn(
                name: "DipartimentoId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreDipartimento",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreDipartimentoEmail",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreDipartimentoEmailPec",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreDipartimentoTelefono",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreStruttura",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreStrutturaEmail",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreStrutturaEmailPec",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "DirettoreStrutturaTelefono",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "EdrEmail",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "EdrEmailPec",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "EdrNome",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "EdrTelefono",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "InailRicevutaCessioneFile",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "InailRicevutaRegistrazioneFile",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PiantinaZoneClassificateFile",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PrepostoEmail",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PrepostoEmailPec",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PrepostoTelefono",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "RirEmail",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "RirEmailPec",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "RirNome",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "RirTelefono",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "SfmEmail",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "SfmEmailPec",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "SfmNome",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "SfmTelefono",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "StrimsRicevutaCessioneFile",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "StrimsRicevutaRegistrazioneFile",
                table: "Apparecchiature");
        }
    }
}
