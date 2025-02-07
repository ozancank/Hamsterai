using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddExitPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExitPassword",
                table: "Users",
                type: "citext",
                nullable: false,
                defaultValue: "GNRs5RnuzaVEvCHRi9ZC8g==")
                .Annotation("Relational:ColumnOrder", 22);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExitPassword",
                table: "Users");
        }
    }
}
