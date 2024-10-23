using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupInClassRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "GroupId",
                table: "ClassRooms",
                type: "smallint",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.UpdateData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "GroupId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "GroupId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "GroupId",
                value: (byte)1);

            migrationBuilder.CreateIndex(
                name: "IX_ClassRooms_GroupId",
                table: "ClassRooms",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRooms_Groups_GroupId",
                table: "ClassRooms",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRooms_Groups_GroupId",
                table: "ClassRooms");

            migrationBuilder.DropIndex(
                name: "IX_ClassRooms_GroupId",
                table: "ClassRooms");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "ClassRooms");
        }
    }
}
