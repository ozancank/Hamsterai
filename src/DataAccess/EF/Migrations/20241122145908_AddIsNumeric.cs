using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddIsNumeric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Lessons",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 9);            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Lessons");
        }
    }
}
