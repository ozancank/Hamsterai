﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)1,
                column: "Name",
                value: "Türkçe");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)4,
                column: "Name",
                value: "Fen Bilimleri");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)5,
                column: "Name",
                value: "Fizik");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)6,
                column: "Name",
                value: "Kimya");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)7,
                column: "Name",
                value: "Biyoloji");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)8,
                column: "Name",
                value: "Edebiyat");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)9,
                column: "Name",
                value: "Tarih");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)10,
                column: "Name",
                value: "Coğrafya");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)11,
                column: "Name",
                value: "Felsefe");

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "SortNo", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)12, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Din Kültürü", (byte)12, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)1);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)4);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)5);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)6);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)7);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)8);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)9);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)10);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)11);

            migrationBuilder.DeleteData(
                table: "Lessons",
                keyColumn: "Id",
                keyValue: (byte)12);

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "SortNo", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Türk Dili ve Edebiyatı", (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Fizik", (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Kimya", (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Biyoloji", (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Geometri", (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Tarih", (byte)8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Coğrafya", (byte)9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)10, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Felsefe", (byte)10, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Din Kültürü ve Ahlak Bilgisi", (byte)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });
        }
    }
}