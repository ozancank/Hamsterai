using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAIIP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIIP",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AIIP",
                table: "Questions",
                type: "citext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIIP",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "AIIP",
                table: "Questions");
        }
    }
}
