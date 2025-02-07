using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddSuccessRate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CorrectCount",
                table: "BookQuizUsers",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 10);

            migrationBuilder.AddColumn<byte>(
                name: "EmptyCount",
                table: "BookQuizUsers",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 12);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "BookQuizUsers",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<double>(
                name: "SuccessRate",
                table: "BookQuizUsers",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0)
                .Annotation("Relational:ColumnOrder", 13);

            migrationBuilder.AddColumn<byte>(
                name: "WrongCount",
                table: "BookQuizUsers",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 11);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectCount",
                table: "BookQuizUsers");

            migrationBuilder.DropColumn(
                name: "EmptyCount",
                table: "BookQuizUsers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BookQuizUsers");

            migrationBuilder.DropColumn(
                name: "SuccessRate",
                table: "BookQuizUsers");

            migrationBuilder.DropColumn(
                name: "WrongCount",
                table: "BookQuizUsers");
        }
    }
}
