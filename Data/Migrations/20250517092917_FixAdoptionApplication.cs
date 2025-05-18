using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawfectHome.Migrations
{
    /// <inheritdoc />
    public partial class FixAdoptionApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplictions_Pets_PetId",
                table: "AdoptionApplictions");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplictions_Users_UserId",
                table: "AdoptionApplictions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdoptionApplictions",
                table: "AdoptionApplictions");

            migrationBuilder.RenameTable(
                name: "AdoptionApplictions",
                newName: "AdoptionApplications");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionApplictions_UserId",
                table: "AdoptionApplications",
                newName: "IX_AdoptionApplications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionApplictions_PetId",
                table: "AdoptionApplications",
                newName: "IX_AdoptionApplications_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdoptionApplications",
                table: "AdoptionApplications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplications_Pets_PetId",
                table: "AdoptionApplications",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplications_Users_UserId",
                table: "AdoptionApplications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplications_Pets_PetId",
                table: "AdoptionApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionApplications_Users_UserId",
                table: "AdoptionApplications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdoptionApplications",
                table: "AdoptionApplications");

            migrationBuilder.RenameTable(
                name: "AdoptionApplications",
                newName: "AdoptionApplictions");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionApplications_UserId",
                table: "AdoptionApplictions",
                newName: "IX_AdoptionApplictions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AdoptionApplications_PetId",
                table: "AdoptionApplictions",
                newName: "IX_AdoptionApplictions_PetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdoptionApplictions",
                table: "AdoptionApplictions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplictions_Pets_PetId",
                table: "AdoptionApplictions",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionApplictions_Users_UserId",
                table: "AdoptionApplictions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
