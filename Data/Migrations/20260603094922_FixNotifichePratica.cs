using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RadiologiaAppNew.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixNotifichePratica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAllegati_NotichePratica_NotificaPraticaId",
                table: "FileAllegati");

            migrationBuilder.DropForeignKey(
                name: "FK_NotichePratica_Apparecchiature_ApparecchiaturaId",
                table: "NotichePratica");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotichePratica",
                table: "NotichePratica");

            migrationBuilder.RenameTable(
                name: "NotichePratica",
                newName: "NotifichePratica");

            migrationBuilder.RenameIndex(
                name: "IX_NotichePratica_ApparecchiaturaId",
                table: "NotifichePratica",
                newName: "IX_NotifichePratica_ApparecchiaturaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotifichePratica",
                table: "NotifichePratica",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAllegati_NotifichePratica_NotificaPraticaId",
                table: "FileAllegati",
                column: "NotificaPraticaId",
                principalTable: "NotifichePratica",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotifichePratica_Apparecchiature_ApparecchiaturaId",
                table: "NotifichePratica",
                column: "ApparecchiaturaId",
                principalTable: "Apparecchiature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAllegati_NotifichePratica_NotificaPraticaId",
                table: "FileAllegati");

            migrationBuilder.DropForeignKey(
                name: "FK_NotifichePratica_Apparecchiature_ApparecchiaturaId",
                table: "NotifichePratica");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NotifichePratica",
                table: "NotifichePratica");

            migrationBuilder.RenameTable(
                name: "NotifichePratica",
                newName: "NotichePratica");

            migrationBuilder.RenameIndex(
                name: "IX_NotifichePratica_ApparecchiaturaId",
                table: "NotichePratica",
                newName: "IX_NotichePratica_ApparecchiaturaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotichePratica",
                table: "NotichePratica",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileAllegati_NotichePratica_NotificaPraticaId",
                table: "FileAllegati",
                column: "NotificaPraticaId",
                principalTable: "NotichePratica",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotichePratica_Apparecchiature_ApparecchiaturaId",
                table: "NotichePratica",
                column: "ApparecchiaturaId",
                principalTable: "Apparecchiature",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
