using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddErrorDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorDescription",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 24);

            migrationBuilder.AddColumn<string>(
                name: "ErrorDescription",
                table: "Questions",
                type: "citext",
                nullable: true,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 22);

            migrationBuilder.AddColumn<bool>(
                name: "IsWebVisible",
                table: "Groups",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AddColumn<byte>(
                name: "SortNo",
                table: "Groups",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorDescription",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "ErrorDescription",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsWebVisible",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SortNo",
                table: "Groups");
        }
    }
}
