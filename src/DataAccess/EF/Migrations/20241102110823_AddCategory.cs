using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AddColumn<byte>(
                name: "CategoryId",
                table: "Packages",
                type: "smallint",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 20);

            migrationBuilder.CreateTable(
                name: "PackageCategories",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    SortNo = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    IsWebVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ParentId = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageCategories", x => x.Id);
                });                        

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CategoryId",
                table: "Packages",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageCategories_Name_ParentId",
                table: "PackageCategories",
                columns: new[] { "Name", "ParentId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_PackageCategories_CategoryId",
                table: "Packages",
                column: "CategoryId",
                principalTable: "PackageCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_PackageCategories_CategoryId",
                table: "Packages");

            migrationBuilder.DropTable(
                name: "PackageCategories");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CategoryId",
                table: "Packages");

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)4);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)5);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)6);

            migrationBuilder.DeleteData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (short)7);

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Packages");

            migrationBuilder.UpdateData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "ClassRooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000012-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000013-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000014-0000-0000-0000-000000000000"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "CreateDate", "CreateUser", "Description", "IsActive", "Name", "OldAmount", "PictureUrl", "Slug", "TaxOldAmount", "UnitOldPrice", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Sözel", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Sayısal", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Eşit Ağırlık", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Yabancı Dil", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Hepsi", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Orta Okul", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Lise", null, null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L }
                });

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedecb"),
                column: "PackageId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "RPackageSchools",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),
                column: "PackageId",
                value: (byte)5);
        }
    }
}
