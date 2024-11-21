using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeworkUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HomeworkUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "citext", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    HomeworkId = table.Column<string>(type: "citext", nullable: false),
                    AnswerPath = table.Column<string>(type: "citext", nullable: true),
                    Status = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeworkUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeworkUsers_Homeworks_HomeworkId",
                        column: x => x.HomeworkId,
                        principalTable: "Homeworks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeworkUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkUsers_HomeworkId",
                table: "HomeworkUsers",
                column: "HomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeworkUsers_UserId",
                table: "HomeworkUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeworkUsers");

            migrationBuilder.RenameIndex(
                name: "IX_Lesson_Name",
                table: "Lessons",
                newName: "IX_OperationClaims_Name");
        }
    }
}
