using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HLSMP.Migrations
{
    /// <inheritdoc />
    public partial class AddVillageTatimaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "VillageTatimas",
                columns: table => new
                {
                    VillageCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dist_Code = table.Column<int>(type: "int", nullable: false),
                    Teh_Code = table.Column<int>(type: "int", nullable: false),
                    TotalTatima = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<int>(type: "int", nullable: false),
                    Pending = table.Column<int>(type: "int", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VillageTatimas", x => x.VillageCode);
                });

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropTable(
                name: "VillageTatimas");

            
        }
    }
}
