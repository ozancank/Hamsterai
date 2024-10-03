using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddUserForTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "CreateDate", "Email", "IsActive", "MustPasswordChange", "Name", "PasswordHash", "PasswordSalt", "Phone", "ProfileUrl", "SchoolId", "Surname", "Type", "UserName" },
                values: new object[,]
                {
                    { 18L, null, new DateTime(2024, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "kazimyildirimeng1@gmail.com", true, false, "Kazım", new byte[] { 186, 31, 96, 48, 95, 78, 158, 166, 85, 180, 187, 134, 246, 224, 115, 214, 111, 80, 51, 192, 138, 129, 200, 8, 1, 31, 183, 138, 110, 232, 23, 68, 117, 97, 147, 141, 33, 52, 101, 79, 162, 46, 153, 107, 207, 73, 111, 234, 192, 119, 232, 192, 21, 66, 179, 216, 87, 72, 216, 181, 95, 59, 109, 162 }, new byte[] { 174, 222, 90, 210, 27, 13, 26, 160, 176, 57, 91, 39, 224, 32, 135, 218, 59, 222, 74, 61, 12, 41, 215, 48, 59, 181, 35, 162, 42, 142, 223, 232, 224, 172, 216, 100, 255, 252, 82, 87, 138, 99, 90, 181, 169, 189, 219, 44, 46, 161, 190, 185, 145, 56, 27, 69, 79, 138, 117, 62, 193, 77, 101, 124, 35, 4, 133, 97, 27, 239, 210, 160, 152, 223, 205, 92, 141, 5, 252, 162, 186, 38, 248, 210, 252, 119, 53, 66, 33, 157, 253, 74, 164, 131, 117, 233, 172, 99, 167, 200, 54, 59, 162, 8, 126, 247, 95, 97, 143, 181, 226, 132, 117, 168, 71, 54, 38, 229, 15, 196, 150, 93, 138, 167, 89, 254, 27, 124 }, "54136952281", null, null, "Yıldırım", (byte)1, "kazim1" },
                    { 19L, null, new DateTime(2024, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "kazimyildirimeng2@gmail.com", true, false, "Kazım", new byte[] { 186, 31, 96, 48, 95, 78, 158, 166, 85, 180, 187, 134, 246, 224, 115, 214, 111, 80, 51, 192, 138, 129, 200, 8, 1, 31, 183, 138, 110, 232, 23, 68, 117, 97, 147, 141, 33, 52, 101, 79, 162, 46, 153, 107, 207, 73, 111, 234, 192, 119, 232, 192, 21, 66, 179, 216, 87, 72, 216, 181, 95, 59, 109, 162 }, new byte[] { 174, 222, 90, 210, 27, 13, 26, 160, 176, 57, 91, 39, 224, 32, 135, 218, 59, 222, 74, 61, 12, 41, 215, 48, 59, 181, 35, 162, 42, 142, 223, 232, 224, 172, 216, 100, 255, 252, 82, 87, 138, 99, 90, 181, 169, 189, 219, 44, 46, 161, 190, 185, 145, 56, 27, 69, 79, 138, 117, 62, 193, 77, 101, 124, 35, 4, 133, 97, 27, 239, 210, 160, 152, 223, 205, 92, 141, 5, 252, 162, 186, 38, 248, 210, 252, 119, 53, 66, 33, 157, 253, 74, 164, 131, 117, 233, 172, 99, 167, 200, 54, 59, 162, 8, 126, 247, 95, 97, 143, 181, 226, 132, 117, 168, 71, 54, 38, 229, 15, 196, 150, 93, 138, 167, 89, 254, 27, 124 }, "54136952282", null, null, "Yıldırım", (byte)1, "kazim2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19L);
        }
    }
}
