using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HLSMP.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVillageTatimatbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsWorkDone",
                table: "VillageTatimas",
                type: "nvarchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VillageStageCode",
                table: "VillageTatimas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkDate",
                table: "VillageTatimas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWorkDone",
                table: "VillageTatimas");

            migrationBuilder.DropColumn(
                name: "VillageStageCode",
                table: "VillageTatimas");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "VillageTatimas");
        }
    }
}
