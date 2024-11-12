using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class NullablePhoneForAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_Teachers_Phone",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "UK_Students_Phone",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "citext",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Students",
                type: "citext",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 15);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Teachers",
                type: "citext",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Students",
                type: "citext",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UK_Teachers_Phone",
                table: "Teachers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Students_Phone",
                table: "Students",
                column: "Phone",
                unique: true);
        }
    }
}