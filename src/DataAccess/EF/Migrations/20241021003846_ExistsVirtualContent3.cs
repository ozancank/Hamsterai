using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class ExistsVirtualContent3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExistsVirtualContent",
                table: "SimilarQuestions",
                newName: "ExistsVisualContent");

            migrationBuilder.RenameColumn(
                name: "ExistsVirtualContent",
                table: "Questions",
                newName: "ExistsVisualContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExistsVisualContent",
                table: "SimilarQuestions",
                newName: "ExistsVirtualContent");

            migrationBuilder.RenameColumn(
                name: "ExistsVisualContent",
                table: "Questions",
                newName: "ExistsVirtualContent");
        }
    }
}
