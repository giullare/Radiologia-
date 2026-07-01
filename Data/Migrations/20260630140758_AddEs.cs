using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Esito",
                table: "Verbali",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Esito",
                table: "Verbali");
        }
    }
}
