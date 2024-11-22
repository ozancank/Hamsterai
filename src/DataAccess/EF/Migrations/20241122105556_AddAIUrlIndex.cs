using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddAIUrlIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "AIUrlIndex",
                table: "Lessons",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIUrlIndex",
                table: "Lessons");
        }
    }
}
