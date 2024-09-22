using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonGroupData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LessonGroups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "LessonId", "IsActive", "GroupId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)3, true, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)4, true, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)5, true, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)6, true, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)7, true, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)8, true, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)9, true, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)10, true, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)11, true, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"));

            migrationBuilder.DeleteData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"));
        }
    }
}
