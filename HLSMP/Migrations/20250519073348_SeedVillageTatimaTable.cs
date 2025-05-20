using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HLSMP.Migrations
{
    /// <inheritdoc />
    public partial class SeedVillageTatimaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VillageTatimas",
                columns: new[] { "VillageCode", "Completed", "Dist_Code", "Pending", "StatusCode", "Teh_Code", "TotalTatima" },
                values: new object[,]
                {
                    { 1, 30, 1, 20, 2, 1, 50 },
                    { 32, 30, 1, 30, 3, 6, 60 },
                    { 1931, 30, 2, 40, 4, 7, 70 },
                    { 1982, 30, 2, 40, 5, 11, 70 },
                    { 2110, 60, 2, 0, 7, 10, 60 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 1931);

            migrationBuilder.DeleteData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 1982);

            migrationBuilder.DeleteData(
                table: "VillageTatimas",
                keyColumn: "VillageCode",
                keyValue: 2110);
        }
    }
}
