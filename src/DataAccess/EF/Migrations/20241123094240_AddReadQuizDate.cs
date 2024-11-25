using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddReadQuizDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AIIP",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 50,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 26);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadDate",
                table: "SimilarQuestions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 27);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendQuizDate",
                table: "SimilarQuestions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 28);

            migrationBuilder.AlterColumn<string>(
                name: "AIIP",
                table: "Questions",
                type: "citext",
                maxLength: 50,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 23);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadDate",
                table: "Questions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 24);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendQuizDate",
                table: "Questions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 25);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "PackageUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000012-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000013-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000014-0000-0000-0000-000000000000"),
                column: "EndDate",
                value: new DateTime(9999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadDate",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "SendQuizDate",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "ReadDate",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SendQuizDate",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "PackageUsers");

            migrationBuilder.AlterColumn<string>(
                name: "AIIP",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "")
                .OldAnnotation("Relational:ColumnOrder", 26);

            migrationBuilder.AlterColumn<string>(
                name: "AIIP",
                table: "Questions",
                type: "citext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "")
                .OldAnnotation("Relational:ColumnOrder", 23);
        }
    }
}
