using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddGains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GainId",
                table: "SimilarQuestions",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 20);

            migrationBuilder.AddColumn<bool>(
                name: "SendForQuiz",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 18);

            migrationBuilder.AddColumn<int>(
                name: "TryCount",
                table: "SimilarQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 19);

            migrationBuilder.AddColumn<int>(
                name: "GainId",
                table: "Questions",
                type: "integer",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 17);

            migrationBuilder.CreateTable(
                name: "Gains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 200, nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gains_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimilarQuestions_GainId",
                table: "SimilarQuestions",
                column: "GainId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GainId",
                table: "Questions",
                column: "GainId");

            migrationBuilder.CreateIndex(
                name: "IX_Gains_1",
                table: "Gains",
                columns: new[] { "Name", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gains_LessonId",
                table: "Gains",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Gains_GainId",
                table: "Questions",
                column: "GainId",
                principalTable: "Gains",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarQuestions_Gains_GainId",
                table: "SimilarQuestions",
                column: "GainId",
                principalTable: "Gains",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Gains_GainId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_SimilarQuestions_Gains_GainId",
                table: "SimilarQuestions");

            migrationBuilder.DropTable(
                name: "Gains");

            migrationBuilder.DropIndex(
                name: "IX_SimilarQuestions_GainId",
                table: "SimilarQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_GainId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "GainId",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "SendForQuiz",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "TryCount",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "GainId",
                table: "Questions");
        }
    }
}
