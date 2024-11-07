using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("2762f9cc-a007-4f0d-96a3-67d7af0a3259"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("282a33b3-4871-450a-9364-ea4a09473663"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("33f62067-25d2-429a-afe5-4ed9c4d4a0e0"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("509514c9-b66e-4f10-8307-cac9eb2ed1b4"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("568081ad-b78c-448a-974b-cc4dfddc6f84"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("74ea982a-ef8b-4048-8f0c-71e498b45876"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("88fbf700-c46c-4cff-83fb-c4cde25add5d"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("8e2b978b-9672-41c6-a030-0a6b40fb69bf"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("95427f3c-1228-4a8a-baa8-84ee156dd46f"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("97d4d695-bcd4-4766-a388-dc3d8396689d"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("9d48b435-6699-4abd-9a94-86d98ea4869d"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("aaf09372-334e-4227-9272-9c1e85cdfbda"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("aced1f4d-138a-4909-bae2-0e28eb30412f"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("bb64a9a1-60c3-4258-a23c-7fa5a34a7ffa"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("c92dd1f5-b3e9-4280-bd47-e56da8544ba7"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("ca0c28f0-354e-4e2d-a87c-5adbdb07076d"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("cc964ed9-ad9c-4260-9062-0083021743f5"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("e3d47f7d-baf9-43b1-9a44-ca636c3f0d9a"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("e492b61a-7503-45a4-9163-986cb048415c"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("f8f18ff6-045b-4d50-83af-3819fe9e674a"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2024, 11, 1, 10, 41, 26, 89, DateTimeKind.Local).AddTicks(4306));

            migrationBuilder.AlterColumn<byte>(
                name: "PaymentRenewalPeriod",
                table: "Packages",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "None");

            migrationBuilder.InsertData(
                table: "PackageUsers",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "PackageId", "UpdateDate", "UpdateUser", "UserId" },
                values: new object[,]
                {
                    { new Guid("00000001-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 1L },
                    { new Guid("00000002-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 2L },
                    { new Guid("00000003-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 3L },
                    { new Guid("00000004-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 4L },
                    { new Guid("00000005-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 5L },
                    { new Guid("00000006-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 6L },
                    { new Guid("00000007-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 7L },
                    { new Guid("00000008-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 8L },
                    { new Guid("00000009-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 9L },
                    { new Guid("0000000a-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 10L },
                    { new Guid("0000000b-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 11L },
                    { new Guid("0000000c-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 12L },
                    { new Guid("0000000d-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 13L },
                    { new Guid("0000000e-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 14L },
                    { new Guid("0000000f-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 15L },
                    { new Guid("00000012-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 18L },
                    { new Guid("00000013-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 19L },
                    { new Guid("00000014-0000-0000-0000-000000000000"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 20L }
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000001-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000002-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000003-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000004-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000005-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000006-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000007-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000008-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000009-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000a-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000b-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000c-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000d-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000e-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("0000000f-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000010-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000011-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000012-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000013-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "PackageUsers",
                keyColumn: "Id",
                keyValue: new Guid("00000014-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Payments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2024, 11, 1, 10, 41, 26, 89, DateTimeKind.Local).AddTicks(4306),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentRenewalPeriod",
                table: "Packages",
                type: "text",
                nullable: false,
                defaultValue: "None",
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldDefaultValue: (byte)0);

            migrationBuilder.InsertData(
                table: "PackageUsers",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "PackageId", "UpdateDate", "UpdateUser", "UserId" },
                values: new object[,]
                {
                    { new Guid("2762f9cc-a007-4f0d-96a3-67d7af0a3259"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 6L },
                    { new Guid("282a33b3-4871-450a-9364-ea4a09473663"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 12L },
                    { new Guid("33f62067-25d2-429a-afe5-4ed9c4d4a0e0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 15L },
                    { new Guid("509514c9-b66e-4f10-8307-cac9eb2ed1b4"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 3L },
                    { new Guid("568081ad-b78c-448a-974b-cc4dfddc6f84"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 10L },
                    { new Guid("74ea982a-ef8b-4048-8f0c-71e498b45876"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 8L },
                    { new Guid("88fbf700-c46c-4cff-83fb-c4cde25add5d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 14L },
                    { new Guid("8e2b978b-9672-41c6-a030-0a6b40fb69bf"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 11L },
                    { new Guid("95427f3c-1228-4a8a-baa8-84ee156dd46f"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 1L },
                    { new Guid("97d4d695-bcd4-4766-a388-dc3d8396689d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 19L },
                    { new Guid("9d48b435-6699-4abd-9a94-86d98ea4869d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 4L },
                    { new Guid("aced1f4d-138a-4909-bae2-0e28eb30412f"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 18L },
                    { new Guid("c92dd1f5-b3e9-4280-bd47-e56da8544ba7"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 13L },
                    { new Guid("ca0c28f0-354e-4e2d-a87c-5adbdb07076d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 20L },
                    { new Guid("cc964ed9-ad9c-4260-9062-0083021743f5"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 7L },
                    { new Guid("e3d47f7d-baf9-43b1-9a44-ca636c3f0d9a"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 2L },
                    { new Guid("e492b61a-7503-45a4-9163-986cb048415c"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 9L },
                    { new Guid("f8f18ff6-045b-4d50-83af-3819fe9e674a"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 5L }
                });

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)1,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)2,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)3,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)4,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)5,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)6,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "Packages",
                keyColumn: "Id",
                keyValue: (byte)7,
                column: "PaymentRenewalPeriod",
                value: "None");

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"),
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"),
                column: "PackageId",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"),
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"),
                column: "PackageId",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"),
                column: "PackageId",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"),
                column: "PackageId",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"),
                column: "PackageId",
                value: (byte)2);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"),
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"),
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"),
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"),
                column: "PackageId",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "RPackageLessons",
                keyColumn: "Id",
                keyValue: new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedecb"),
                column: "PackageId",
                value: (byte)1);
        }
    }
}
