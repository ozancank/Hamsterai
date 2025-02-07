using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "UK_OperationClaims_Name",
                table: "OperationClaims");

            migrationBuilder.DropUniqueConstraint(
                name: "UK_Lessons_Name",
                table: "Lessons");

            migrationBuilder.CreateIndex(
                name: "IX_OperationClaims_Name",
                table: "OperationClaims",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lesson_Name",
                table: "Lessons",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OperationClaims_Name",
                table: "OperationClaims");

            migrationBuilder.DropIndex(
                name: "IX_OperationClaims_Name",
                table: "Lessons");

            migrationBuilder.AddUniqueConstraint(
                name: "UK_OperationClaims_Name",
                table: "OperationClaims",
                column: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "UK_Lessons_Name",
                table: "Lessons",
                column: "Name");
        }
    }
}
