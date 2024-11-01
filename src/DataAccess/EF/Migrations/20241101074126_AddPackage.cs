using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class AddPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRooms_Groups_GroupId",
                table: "ClassRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "LessonGroups");

            migrationBuilder.DropTable(
                name: "SchoolGroups");

            migrationBuilder.DropTable(
                name: "TeacherClassRooms");

            migrationBuilder.DropTable(
                name: "TeacherLessons");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Users_GroupId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "UK_Teachers_TcNo",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "UK_Students_TcNo",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TcNo",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "TcNo",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "QuestionCount",
                table: "Users",
                newName: "PackageCredit");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "ClassRooms",
                newName: "PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassRooms_GroupId",
                table: "ClassRooms",
                newName: "IX_ClassRooms_PackageId");

            migrationBuilder.AddColumn<int>(
                name: "AddtionalCredit",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Relational:ColumnOrder", 17);

            migrationBuilder.AddColumn<bool>(
                name: "AutomaticPayment",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false)
                .Annotation("Relational:ColumnOrder", 18);

            migrationBuilder.AlterColumn<int>(
                name: "GainId",
                table: "QuizQuestions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNo = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    QuestionCredit = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    SubTotal = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    DiscountAmount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxBase = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxAmount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    Amount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    SortNo = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    IsWebVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UnitPrice = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    UnitOldPrice = table.Column<double>(type: "double precision", nullable: true),
                    TaxRatio = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxAmount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxOldAmount = table.Column<double>(type: "double precision", nullable: true),
                    Amount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    OldAmount = table.Column<double>(type: "double precision", nullable: true),
                    PaymentRenewalPeriod = table.Column<string>(type: "text", nullable: false, defaultValue: "None"),
                    Description = table.Column<string>(type: "citext", maxLength: 500, nullable: true),
                    PictureUrl = table.Column<string>(type: "citext", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    PaymentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValue: new DateTime(2024, 11, 1, 10, 41, 26, 89, DateTimeKind.Local).AddTicks(4306)),
                    PaymentReason = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    ReasonId = table.Column<string>(type: "citext", maxLength: 50, nullable: true),
                    SipayMerchantKey = table.Column<string>(type: "citext", maxLength: 100, nullable: true),
                    SipayPlanCode = table.Column<string>(type: "citext", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RTeacherClassRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    ClassRoomId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RTeacherClassRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RTeacherClassRooms_ClassRooms_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RTeacherClassRooms_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RTeacherLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RTeacherLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RTeacherLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RTeacherLessons_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<byte>(type: "smallint", nullable: false),
                    QuestionCredit = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Quantity = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    UnitPrice = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    DiscountRatio = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    DiscountAmount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxBase = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxRatio = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    TaxAmount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    Amount = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PackageUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PackageId = table.Column<byte>(type: "smallint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RenewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageUsers_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PackageUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RPackageLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PackageId = table.Column<byte>(type: "smallint", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPackageLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RPackageLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RPackageLessons_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RPackageSchools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    PackageId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RPackageSchools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RPackageSchools_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RPackageSchools_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Packages",
                columns: new[] { "Id", "CreateDate", "CreateUser", "Description", "IsActive", "Name", "OldAmount", "PictureUrl", "TaxOldAmount", "UnitOldPrice", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Sözel", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Sayısal", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Eşit Ağırlık", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Yabancı Dil", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Hepsi", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Orta Okul", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, null, true, "Lise", null, null, null, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L }
                });

            migrationBuilder.InsertData(
                table: "RTeacherClassRooms",
                columns: new[] { "Id", "ClassRoomId", "CreateDate", "CreateUser", "IsActive", "TeacherId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"), 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"), 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"), 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "RTeacherLessons",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "LessonId", "TeacherId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)2, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)1, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "PackageUsers",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "PackageId", "UpdateDate", "UpdateUser", "UserId" },
                values: new object[,]
                {
                    { new Guid("2762f9cc-a007-4f0d-96a3-67d7af0a3259"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 6L },
                    { new Guid("282a33b3-4871-450a-9364-ea4a09473663"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 12L },
                    { new Guid("33f62067-25d2-429a-afe5-4ed9c4d4a0e0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 15L },
                    { new Guid("509514c9-b66e-4f10-8307-cac9eb2ed1b4"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 3L },
                    { new Guid("568081ad-b78c-448a-974b-cc4dfddc6f84"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 10L },
                    { new Guid("74ea982a-ef8b-4048-8f0c-71e498b45876"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 8L },
                    { new Guid("88fbf700-c46c-4cff-83fb-c4cde25add5d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 14L },
                    { new Guid("8e2b978b-9672-41c6-a030-0a6b40fb69bf"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 11L },
                    { new Guid("95427f3c-1228-4a8a-baa8-84ee156dd46f"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 1L },
                    { new Guid("97d4d695-bcd4-4766-a388-dc3d8396689d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 19L },
                    { new Guid("9d48b435-6699-4abd-9a94-86d98ea4869d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 4L },
                    { new Guid("aaf09372-334e-4227-9272-9c1e85cdfbda"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 16L },
                    { new Guid("aced1f4d-138a-4909-bae2-0e28eb30412f"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 18L },
                    { new Guid("bb64a9a1-60c3-4258-a23c-7fa5a34a7ffa"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 17L },
                    { new Guid("c92dd1f5-b3e9-4280-bd47-e56da8544ba7"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 13L },
                    { new Guid("ca0c28f0-354e-4e2d-a87c-5adbdb07076d"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 20L },
                    { new Guid("cc964ed9-ad9c-4260-9062-0083021743f5"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 7L },
                    { new Guid("e3d47f7d-baf9-43b1-9a44-ca636c3f0d9a"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 2L },
                    { new Guid("e492b61a-7503-45a4-9163-986cb048415c"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 9L },
                    { new Guid("f8f18ff6-045b-4d50-83af-3819fe9e674a"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, 5L }
                });

            migrationBuilder.InsertData(
                table: "RPackageLessons",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "LessonId", "PackageId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)1, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)2, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)3, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)4, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)5, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)6, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)7, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)8, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)9, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)10, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)11, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedecb"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)12, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "RPackageSchools",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "PackageId", "SchoolId", "UpdateDate", "UpdateUser" },
                values: new object[] { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, (byte)5, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_PackageId",
                table: "OrderDetails",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_1",
                table: "Packages",
                columns: new[] { "Name", "PaymentRenewalPeriod" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageUsers_UserId",
                table: "PackageUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RPackageUsers_1",
                table: "PackageUsers",
                columns: new[] { "PackageId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RPackageLesson_1",
                table: "RPackageLessons",
                columns: new[] { "PackageId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RPackageLessons_LessonId",
                table: "RPackageLessons",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_RPackageSchools_1",
                table: "RPackageSchools",
                columns: new[] { "PackageId", "SchoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RPackageSchools_SchoolId",
                table: "RPackageSchools",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_RTeacherClassRooms_ClassRoomId",
                table: "RTeacherClassRooms",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassRooms_1",
                table: "RTeacherClassRooms",
                columns: new[] { "TeacherId", "ClassRoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RTeacherLessons_LessonId",
                table: "RTeacherLessons",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessons_1",
                table: "RTeacherLessons",
                columns: new[] { "TeacherId", "LessonId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRooms_Packages_PackageId",
                table: "ClassRooms",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassRooms_Packages_PackageId",
                table: "ClassRooms");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "PackageUsers");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RPackageLessons");

            migrationBuilder.DropTable(
                name: "RPackageSchools");

            migrationBuilder.DropTable(
                name: "RTeacherClassRooms");

            migrationBuilder.DropTable(
                name: "RTeacherLessons");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropColumn(
                name: "AddtionalCredit",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AutomaticPayment",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PackageCredit",
                table: "Users",
                newName: "QuestionCount");

            migrationBuilder.RenameColumn(
                name: "PackageId",
                table: "ClassRooms",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassRooms_PackageId",
                table: "ClassRooms",
                newName: "IX_ClassRooms_GroupId");

            migrationBuilder.AddColumn<byte>(
                name: "GroupId",
                table: "Users",
                type: "smallint",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 15);

            migrationBuilder.AddColumn<string>(
                name: "TcNo",
                table: "Teachers",
                type: "citext",
                maxLength: 11,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 8);

            migrationBuilder.AddColumn<string>(
                name: "TcNo",
                table: "Students",
                type: "citext",
                maxLength: 11,
                nullable: true)
                .Annotation("Relational:ColumnOrder", 9);

            migrationBuilder.AlterColumn<int>(
                name: "GainId",
                table: "QuizQuestions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    SortNo = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    IsWebVisible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.UniqueConstraint("UK_Groups_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "TeacherClassRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    ClassRoomId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherClassRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherClassRooms_ClassRooms_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherClassRooms_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeacherLessons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherLessons_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TeacherLessons_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LessonGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GroupId = table.Column<byte>(type: "smallint", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonGroups_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                });

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
                table: "Groups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Sözel", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Sayısal", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Eşit Ağırlık", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Yabancı Dil", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Hepsi", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Orta Okul", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L },
                    { (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, "Lise", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L }
                });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "TcNo",
                value: "33333333333");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                column: "TcNo",
                value: "44444444444");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                column: "TcNo",
                value: "55555555555");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4,
                column: "TcNo",
                value: "66666666666");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5,
                column: "TcNo",
                value: "77777777777");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6,
                column: "TcNo",
                value: "88888888888");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 7,
                column: "TcNo",
                value: "12312312399");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 8,
                column: "TcNo",
                value: "12312312388");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 9,
                column: "TcNo",
                value: "12312312377");

            migrationBuilder.InsertData(
                table: "TeacherClassRooms",
                columns: new[] { "Id", "ClassRoomId", "CreateDate", "CreateUser", "IsActive", "TeacherId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"), 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"), 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"), 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "TeacherLessons",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "LessonId", "TeacherId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)2, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (byte)1, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1,
                column: "TcNo",
                value: "11111111111");

            migrationBuilder.UpdateData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 2,
                column: "TcNo",
                value: "22222222222");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20L,
                column: "GroupId",
                value: (byte)5);

            migrationBuilder.InsertData(
                table: "LessonGroups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "GroupId", "IsActive", "LessonId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec1"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)2, true, (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec2"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec3"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)2, true, (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec4"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)2, true, (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec5"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)2, true, (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec6"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)2, true, (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec7"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec8"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec9"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)10, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedecb"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)12, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "SchoolGroups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "GroupId", "IsActive", "SchoolId", "UpdateDate", "UpdateUser" },
                values: new object[] { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedec0"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L, (byte)5, true, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1L });

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "UK_Teachers_TcNo",
                table: "Teachers",
                column: "TcNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Students_TcNo",
                table: "Students",
                column: "TcNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonGroups_1",
                table: "LessonGroups",
                columns: new[] { "GroupId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonGroups_LessonId",
                table: "LessonGroups",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolGroups_1",
                table: "SchoolGroups",
                columns: new[] { "GroupId", "SchoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchoolGroups_SchoolId",
                table: "SchoolGroups",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassRooms_1",
                table: "TeacherClassRooms",
                columns: new[] { "TeacherId", "ClassRoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassRooms_ClassRoomId",
                table: "TeacherClassRooms",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessons_1",
                table: "TeacherLessons",
                columns: new[] { "TeacherId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherLessons_LessonId",
                table: "TeacherLessons",
                column: "LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassRooms_Groups_GroupId",
                table: "ClassRooms",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Groups_GroupId",
                table: "Users",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
