using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPostit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Postits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LessonId = table.Column<short>(type: "smallint", nullable: false),
                    Title = table.Column<string>(type: "citext", maxLength: 250, nullable: true, defaultValue: ""),
                    Description = table.Column<string>(type: "citext", nullable: true, defaultValue: ""),
                    Color = table.Column<string>(type: "citext", maxLength: 8, nullable: true, defaultValue: "F7DD00"),
                    SortNo = table.Column<short>(type: "smallint", nullable: false),
                    PictureFileName = table.Column<string>(type: "citext", nullable: true, defaultValue: "")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Postits_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Postits_Users_CreateUser",
                        column: x => x.CreateUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Postits_CreateUser",
                table: "Postits",
                column: "CreateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Postits_LessonId",
                table: "Postits",
                column: "LessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Postits");
        }
    }
}
