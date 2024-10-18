using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddGroup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "GroupId",
                table: "Users",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0)
                .Annotation("Relational:ColumnOrder", 15);

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeQuiz",
                table: "Questions",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 19);

            migrationBuilder.AlterColumn<string>(
                name: "ReasonId",
                table: "Notifications",
                type: "citext",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)1,
                columns: new[] { "CreateUser", "UpdateUser" },
                values: new object[] { 1L, 1L });

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)2,
                columns: new[] { "CreateUser", "UpdateUser" },
                values: new object[] { 1L, 1L });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Eşit Ağırlık", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Yabancı Dil", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Hepsi", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Orta Okul", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Lise", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L }
                });

            migrationBuilder.InsertData(
                table: "LessonGroups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "GroupId", "IsActive", "LessonId", "UpdateDate", "UpdateUser" },
                values: new object[] { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedecb"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)12, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)3);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)5);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)6);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)7);

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedecb"));

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExcludeQuiz",
                table: "Questions");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReasonId",
                table: "Notifications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)1,
                columns: new[] { "CreateUser", "UpdateUser" },
                values: new object[] { 2L, 2L });

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: (byte)2,
                columns: new[] { "CreateUser", "UpdateUser" },
                values: new object[] { 2L, 2L });
        }
    }
}
