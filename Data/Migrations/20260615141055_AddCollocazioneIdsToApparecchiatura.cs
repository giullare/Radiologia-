using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCollocazioneIdsToApparecchiatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ImmobileId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PianoId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SitoId",
                table: "Apparecchiature",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImmobileId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "PianoId",
                table: "Apparecchiature");

            migrationBuilder.DropColumn(
                name: "SitoId",
                table: "Apparecchiature");
        }
    }
}
