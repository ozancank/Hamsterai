using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolIdInBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Name",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Books",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 11);

            migrationBuilder.CreateIndex(
                name: "IX_Books_SchoolId",
                table: "Books",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Schools_SchoolId",
                table: "Books",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Schools_SchoolId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_SchoolId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Books");

            migrationBuilder.CreateIndex(
                name: "IX_Books_Name",
                table: "Books",
                column: "Name",
                unique: true);
        }
    }
}
