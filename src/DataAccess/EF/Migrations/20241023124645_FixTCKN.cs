using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixTCKN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TcNo",
                table: "Teachers",
                type: "citext",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "TcNo",
                table: "Students",
                type: "citext",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 11);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TcNo",
                table: "Teachers",
                type: "citext",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TcNo",
                table: "Students",
                type: "citext",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 11,
                oldNullable: true);
        }
    }
}
