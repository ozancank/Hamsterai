using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionReadQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 17);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Questions",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.AddColumn<bool>(
                name: "SendForQuiz",
                table: "Questions",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 15);

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)1, (byte)3 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)2, (byte)4 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)2, (byte)5 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)2, (byte)6 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)2, (byte)7 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)1, (byte)8 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)1, (byte)9 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)1, (byte)10 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)1, (byte)11 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SendForQuiz",
                table: "Questions");

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)3, (byte)1 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)4, (byte)2 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)5, (byte)2 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)6, (byte)2 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)7, (byte)2 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)8, (byte)1 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)9, (byte)1 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)10, (byte)1 });

            migrationBuilder.UpdateData(
                table: "LessonGroups",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"),
                columns: new[] { "GroupId", "LessonId" },
                values: new object[] { (byte)11, (byte)1 });
        }
    }
}
