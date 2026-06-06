using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class AggiuntaAnagraficaTecnica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Costruttori",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Paese = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SitoWeb = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costruttori", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocietaManutenzione",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    NumeroAssistenza = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    NumeroReperibilita = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EmailAssistenza = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    GlobalService = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    SitoWeb = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocietaManutenzione", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelliApparecchiatura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Tipologia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Attivo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CostrutoreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelliApparecchiatura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelliApparecchiatura_Costruttori_CostrutoreId",
                        column: x => x.CostrutoreId,
                        principalTable: "Costruttori",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Costruttori",
                columns: new[] { "Id", "Attivo", "Nome", "Note", "Paese", "SitoWeb" },
                values: new object[,]
                {
                    { 1, true, "Siemens Healthineers", null, "Germania", null },
                    { 2, true, "GE HealthCare", null, "USA", null },
                    { 3, true, "Philips Healthcare", null, "Paesi Bassi", null },
                    { 4, true, "Canon Medical Systems", null, "Giappone", null },
                    { 5, true, "Hologic", null, "USA", null },
                    { 6, true, "Varex Imaging", null, "USA", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModelliApparecchiatura_CostrutoreId",
                table: "ModelliApparecchiatura",
                column: "CostrutoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelliApparecchiatura");

            migrationBuilder.DropTable(
                name: "SocietaManutenzione");

            migrationBuilder.DropTable(
                name: "Costruttori");
        }
    }
}
