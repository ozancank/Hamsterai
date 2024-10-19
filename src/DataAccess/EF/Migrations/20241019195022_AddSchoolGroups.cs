using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    GroupId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolGroups_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "SchoolGroups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "GroupId", "IsActive", "SchoolId", "UpdateDate", "UpdateUser" },
                values: new object[] { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, (byte)5, true, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolGroups_1",
                table: "SchoolGroups",
                columns: new[] { "GroupId", "SchoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolGroups_SchoolId",
                table: "SchoolGroups",
                column: "SchoolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolGroups");
        }
    }
}
