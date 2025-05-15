using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawfectHome.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePetEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Pets");

            migrationBuilder.RenameColumn(
                name: "IsNeutured",
                table: "Pets",
                newName: "IsNeutered");

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthdate",
                table: "Pets",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Birthdate",
                table: "Pets");

            migrationBuilder.RenameColumn(
                name: "IsNeutered",
                table: "Pets",
                newName: "IsNeutured");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
