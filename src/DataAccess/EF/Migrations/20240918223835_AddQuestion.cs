using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnswerPictureExtension",
                table: "Questions",
                type: "citext",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AddColumn<string>(
                name: "AnswerPictureFileName",
                table: "Questions",
                type: "citext",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 12);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerPictureExtension",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerPictureFileName",
                table: "Questions");
        }
    }
}
