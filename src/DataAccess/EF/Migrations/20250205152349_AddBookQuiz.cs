using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddBookQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookQuizzes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    LessonId = table.Column<short>(type: "smallint", nullable: false),
                    QuestionCount = table.Column<byte>(type: "smallint", nullable: false),
                    OptionCount = table.Column<byte>(type: "smallint", nullable: false),
                    Answers = table.Column<string>(type: "citext", nullable: false),
                    BookId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookQuizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookQuizzes_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookQuizzes_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookQuizUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    BookQuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Answers = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookQuizUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookQuizUsers_BookQuizzes_BookQuizId",
                        column: x => x.BookQuizId,
                        principalTable: "BookQuizzes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BookQuizUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookQuizUsers_BookQuizId",
                table: "BookQuizUsers",
                column: "BookQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_BookQuizUsers_UserId",
                table: "BookQuizUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookQuizzes_BookId",
                table: "BookQuizzes",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookQuizzes_LessonId",
                table: "BookQuizzes",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookQuizUsers");

            migrationBuilder.DropTable(
                name: "BookQuizzes");
        }
    }
}
