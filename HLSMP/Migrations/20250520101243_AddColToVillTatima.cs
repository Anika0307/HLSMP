using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HLSMP.Migrations
{
    /// <inheritdoc />
    public partial class AddColToVillTatima : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedDocument",
                table: "VillageTatimas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 1,
                column: "UploadedDocument",
                value: null);

            migrationBuilder.UpdateData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 32,
                column: "UploadedDocument",
                value: null);

            migrationBuilder.UpdateData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 1931,
                column: "UploadedDocument",
                value: null);

            migrationBuilder.UpdateData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 1982,
                column: "UploadedDocument",
                value: null);

            migrationBuilder.UpdateData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 2110,
                column: "UploadedDocument",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedDocument",
                table: "VillageTatimas");
        }
    }
}
