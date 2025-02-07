using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddWebHookString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OcrMethod",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 25);

            migrationBuilder.AddColumn<string>(
                name: "WebHookString",
                table: "PaymentSipays",
                type: "citext",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 33);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OcrMethod",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "WebHookString",
                table: "PaymentSipays");
        }
    }
}
