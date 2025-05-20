using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HLSMP.Migrations
{
    /// <inheritdoc />
    public partial class AddColToVT : Migration
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
