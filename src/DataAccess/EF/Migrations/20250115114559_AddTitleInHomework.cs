using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleInHomework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Homeworks",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 12);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Homeworks",
                type: "citext",
                maxLength: 200,
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.CreateIndex(
                name: "IX_Books_LessonId",
                table: "Books",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Lessons_LessonId",
                table: "Books",
                column: "LessonId",
                principalTable: "Lessons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Lessons_LessonId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_LessonId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Homeworks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Homeworks");
        }
    }
}
