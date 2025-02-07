using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionPicture",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionPictureExtension",
                table: "SimilarQuestions");

            migrationBuilder.DropColumn(
                name: "QuestionPictureFileName",
                table: "SimilarQuestions");

            migrationBuilder.RenameColumn(
                name: "QuestionPictureBase64",
                table: "Questions",
                newName: "QuestionText");

            migrationBuilder.AlterColumn<int>(
                name: "TryCount",
                table: "SimilarQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0)
                .Annotation("Relational:ColumnOrder", 16)
                .OldAnnotation("Relational:ColumnOrder", 19);

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "SimilarQuestions",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint")
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendQuizDate",
                table: "SimilarQuestions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 25)
                .OldAnnotation("Relational:ColumnOrder", 28);

            migrationBuilder.AlterColumn<bool>(
                name: "SendForQuiz",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 18);

            migrationBuilder.AlterColumn<char>(
                name: "RightOption",
                table: "SimilarQuestions",
                type: "character(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(char),
                oldType: "character(1)",
                oldMaxLength: 1,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 18)
                .OldAnnotation("Relational:ColumnOrder", 21);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseQuestionFileName",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 8)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseQuestionExtension",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 10)
                .Annotation("Relational:ColumnOrder", 9)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseQuestion",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 7)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseAnswerFileName",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseAnswerExtension",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 10)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseAnswer",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadDate",
                table: "SimilarQuestions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 24)
                .OldAnnotation("Relational:ColumnOrder", 27);

            migrationBuilder.AlterColumn<string>(
                name: "OcrMethod",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true,
                oldDefaultValue: "")
                .Annotation("Relational:ColumnOrder", 22)
                .OldAnnotation("Relational:ColumnOrder", 25);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 17);

            migrationBuilder.AlterColumn<int>(
                name: "GainId",
                table: "SimilarQuestions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 17)
                .OldAnnotation("Relational:ColumnOrder", 20);

            migrationBuilder.AlterColumn<bool>(
                name: "ExistsVisualContent",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 20)
                .OldAnnotation("Relational:ColumnOrder", 23);

            migrationBuilder.AlterColumn<bool>(
                name: "ExcludeQuiz",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 19)
                .OldAnnotation("Relational:ColumnOrder", 22);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDescription",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true,
                oldDefaultValue: "")
                .Annotation("Relational:ColumnOrder", 21)
                .OldAnnotation("Relational:ColumnOrder", 24);

            migrationBuilder.AlterColumn<string>(
                name: "AIIP",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 50,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "")
                .Annotation("Relational:ColumnOrder", 23)
                .OldAnnotation("Relational:ColumnOrder", 26);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuestionText",
                table: "Questions",
                newName: "QuestionPictureBase64");

            migrationBuilder.AlterColumn<int>(
                name: "TryCount",
                table: "SimilarQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0)
                .Annotation("Relational:ColumnOrder", 19)
                .OldAnnotation("Relational:ColumnOrder", 16);

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "SimilarQuestions",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint")
                .Annotation("Relational:ColumnOrder", 16)
                .OldAnnotation("Relational:ColumnOrder", 13);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendQuizDate",
                table: "SimilarQuestions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 28)
                .OldAnnotation("Relational:ColumnOrder", 25);

            migrationBuilder.AlterColumn<bool>(
                name: "SendForQuiz",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 18)
                .OldAnnotation("Relational:ColumnOrder", 15);

            migrationBuilder.AlterColumn<char>(
                name: "RightOption",
                table: "SimilarQuestions",
                type: "character(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(char),
                oldType: "character(1)",
                oldMaxLength: 1,
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 21)
                .OldAnnotation("Relational:ColumnOrder", 18);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseQuestionFileName",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 11)
                .OldAnnotation("Relational:ColumnOrder", 8);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseQuestionExtension",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 10)
                .Annotation("Relational:ColumnOrder", 12)
                .OldAnnotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseQuestion",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 10)
                .OldAnnotation("Relational:ColumnOrder", 7);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseAnswerFileName",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 14)
                .OldAnnotation("Relational:ColumnOrder", 11);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseAnswerExtension",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 10)
                .Annotation("Relational:ColumnOrder", 15)
                .OldAnnotation("Relational:ColumnOrder", 12);

            migrationBuilder.AlterColumn<string>(
                name: "ResponseAnswer",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "citext")
                .Annotation("Relational:ColumnOrder", 13)
                .OldAnnotation("Relational:ColumnOrder", 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReadDate",
                table: "SimilarQuestions",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .Annotation("Relational:ColumnOrder", 27)
                .OldAnnotation("Relational:ColumnOrder", 24);

            migrationBuilder.AlterColumn<string>(
                name: "OcrMethod",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true,
                oldDefaultValue: "")
                .Annotation("Relational:ColumnOrder", 25)
                .OldAnnotation("Relational:ColumnOrder", 22);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 17)
                .OldAnnotation("Relational:ColumnOrder", 14);

            migrationBuilder.AlterColumn<int>(
                name: "GainId",
                table: "SimilarQuestions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true)
                .Annotation("Relational:ColumnOrder", 20)
                .OldAnnotation("Relational:ColumnOrder", 17);

            migrationBuilder.AlterColumn<bool>(
                name: "ExistsVisualContent",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 23)
                .OldAnnotation("Relational:ColumnOrder", 20);

            migrationBuilder.AlterColumn<bool>(
                name: "ExcludeQuiz",
                table: "SimilarQuestions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false)
                .Annotation("Relational:ColumnOrder", 22)
                .OldAnnotation("Relational:ColumnOrder", 19);

            migrationBuilder.AlterColumn<string>(
                name: "ErrorDescription",
                table: "SimilarQuestions",
                type: "citext",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldNullable: true,
                oldDefaultValue: "")
                .Annotation("Relational:ColumnOrder", 24)
                .OldAnnotation("Relational:ColumnOrder", 21);

            migrationBuilder.AlterColumn<string>(
                name: "AIIP",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 50,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "")
                .Annotation("Relational:ColumnOrder", 26)
                .OldAnnotation("Relational:ColumnOrder", 23);

            migrationBuilder.AddColumn<string>(
                name: "QuestionPicture",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 7);

            migrationBuilder.AddColumn<string>(
                name: "QuestionPictureExtension",
                table: "SimilarQuestions",
                type: "citext",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AddColumn<string>(
                name: "QuestionPictureFileName",
                table: "SimilarQuestions",
                type: "citext",
                nullable: false,
                defaultValue: "")
                .Annotation("Relational:ColumnOrder", 8);
        }
    }
}
