using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPublishers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (short)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Hız Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "3D Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Vip Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Okyanus Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Paraf Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Yanıt Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Acil Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Miray Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Palme Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)10, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Muba Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Arı Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)12, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Nitelik Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)13, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Fenomen Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)14, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Ankara Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)15, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Adrenalin Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)16, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Paylaşım Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)17, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Ogm Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)18, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Bilgi Sarmal Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)19, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Çap Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)20, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Aktif Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)21, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Aydın Yayınları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (short)22, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Biotik Yayıları", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)1);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)2);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)3);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)4);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)5);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)6);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)7);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)8);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)9);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)10);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)11);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)12);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)13);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)14);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)15);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)16);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)17);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)18);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)19);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)20);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)21);

            migrationBuilder.DeleteData(
                table: "Publishers",
                keyColumn: "Id",
                keyValue: (short)22);
        }
    }
}
