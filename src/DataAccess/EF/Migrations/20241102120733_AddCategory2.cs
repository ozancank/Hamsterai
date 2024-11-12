using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PackageCategories",
                type: "citext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "PackageCategories",
                type: "citext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "PackageCategories",
                type: "citext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "PackageCategories");

            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "PackageCategories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "PackageCategories");
        }
    }
}