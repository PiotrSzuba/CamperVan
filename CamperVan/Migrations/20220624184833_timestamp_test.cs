using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamperVan.Migrations
{
    public partial class timestamp_test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Energie",
                table: "Energie");

            migrationBuilder.RenameTable(
                name: "Energie",
                newName: "Energy");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp2",
                table: "Water",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Energy",
                table: "Energy",
                column: "EnergyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Energy",
                table: "Energy");

            migrationBuilder.DropColumn(
                name: "Timestamp2",
                table: "Water");

            migrationBuilder.RenameTable(
                name: "Energy",
                newName: "Energie");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Energie",
                table: "Energie",
                column: "EnergyId");
        }
    }
}
