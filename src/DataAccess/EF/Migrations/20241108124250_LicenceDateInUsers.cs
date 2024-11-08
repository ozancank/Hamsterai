using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class LicenceDateInUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LicenceEndDate",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 20);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Users",
                type: "citext",
                maxLength: 11,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 19);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20L,
                columns: new[] { "LicenceEndDate", "TaxNumber" },
                values: new object[] { new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenceEndDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Users");
        }
    }
}
