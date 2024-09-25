using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false),
                    TimeSecond = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    CorrectCount = table.Column<byte>(type: "smallint", nullable: false),
                    WrongCount = table.Column<byte>(type: "smallint", nullable: false),
                    EmptyCount = table.Column<byte>(type: "smallint", nullable: false),
                    SuccessRate = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SortNo = table.Column<string>(type: "smallint", nullable: false),
                    QuizId = table.Column<string>(type: "citext", nullable: false),
                    Question = table.Column<string>(type: "citext", nullable: false),
                    QuestionPictureFileName = table.Column<string>(type: "citext", nullable: false),
                    QuestionPictureExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    Answer = table.Column<string>(type: "citext", nullable: false),
                    AnswerPictureFileName = table.Column<string>(type: "citext", nullable: false),
                    AnswerPictureExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    RightOption = table.Column<char>(type: "character(1)", maxLength: 1, nullable: false),
                    AnswerOption = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true),
                    OptionCount = table.Column<byte>(type: "smallint", nullable: false),
                    GainId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Gains_GainId",
                        column: x => x.GainId,
                        principalTable: "Gains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_GainId",
                table: "QuizQuestions",
                column: "GainId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizId",
                table: "QuizQuestions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_LessonId",
                table: "Quizzes",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UserId",
                table: "Quizzes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "Quizzes");
        }
    }
}
