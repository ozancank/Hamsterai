using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionCreditInPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Payments");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Payments",
                type: "uuid",
                nullable: false)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<int>(
                name: "QuestionCredit",
                table: "Packages",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 22);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Packages",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)1)
                .Annotation("Relational:ColumnOrder", 21);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionCredit",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Packages");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Payments",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}