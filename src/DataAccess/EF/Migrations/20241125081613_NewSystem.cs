using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class NewSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RPackageSchools");

            migrationBuilder.DropColumn(
                name: "AddtionalCredit",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenceEndDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PackageCredit",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LicenseEndDate",
                table: "Schools");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PackageUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "QuestionCredit",
                table: "PackageUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionCredit",
                table: "PackageUsers");

            migrationBuilder.AddColumn<int>(
                name: "AddtionalCredit",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 17);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenceEndDate",
                table: "Users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 20);

            migrationBuilder.AddColumn<int>(
                name: "PackageCredit",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 16);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenseEndDate",
                table: "Schools",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PackageUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2024, 12, 23, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "RPackageSchools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPackageSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RPackageSchools_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RPackageSchools_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "RPackageSchools",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "PackageId", "SchoolId", "UpdateDate", "UpdateUser" },
                values: new object[] { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (short)5, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L });

            migrationBuilder.UpdateData(
                table: "Schools",
                keyColumn: "Id",
                keyValue: 1,
                column: "LicenseEndDate",
                value: new DateTime(2050, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20L,
                column: "LicenceEndDate",
                value: new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_RPackageSchools_1",
                table: "RPackageSchools",
                columns: new[] { "PackageId", "SchoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RPackageSchools_SchoolId",
                table: "RPackageSchools",
                column: "SchoolId");
        }
    }
}
