using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.EF.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

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
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.UniqueConstraint("UK_Groups_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    SortNo = table.Column<byte>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.UniqueConstraint("UK_Lessons_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "OperationClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "citext", maxLength: 200, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperationClaims", x => x.Id);
                    table.UniqueConstraint("UK_OperationClaims_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Schools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 250, nullable: false),
                    TaxNumber = table.Column<string>(type: "citext", maxLength: 11, nullable: false),
                    Address = table.Column<string>(type: "citext", nullable: false),
                    City = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    AuthorizedName = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    AuthorizedPhone = table.Column<string>(type: "citext", maxLength: 15, nullable: false),
                    AuthorizedEmail = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    UserCount = table.Column<int>(type: "integer", nullable: false),
                    LicenseEndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schools", x => x.Id);
                });

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
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
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
                name: "ClassRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    No = table.Column<short>(type: "smallint", nullable: false),
                    Branch = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassRooms_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "citext", maxLength: 250, nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "bytea", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    MustPasswordChange = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 250, nullable: false),
                    Surname = table.Column<string>(type: "citext", maxLength: 250, nullable: false),
                    Phone = table.Column<string>(type: "citext", maxLength: 15, nullable: false),
                    ProfileUrl = table.Column<string>(type: "citext", nullable: true),
                    Email = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: true),
                    ConnectionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    StudentNo = table.Column<string>(type: "citext", maxLength: 15, nullable: false),
                    TcNo = table.Column<string>(type: "citext", maxLength: 11, nullable: false),
                    Phone = table.Column<string>(type: "citext", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    ClassRoomId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_ClassRooms_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationDeviceTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    DeviceToken = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDeviceTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationDeviceTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PasswordTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Expried = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false),
                    QuestionPictureBase64 = table.Column<string>(type: "citext", nullable: false),
                    QuestionPictureFileName = table.Column<string>(type: "citext", nullable: false),
                    QuestionPictureExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    AnswerText = table.Column<string>(type: "citext", nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    AnswerPictureFileName = table.Column<string>(type: "citext", nullable: false, defaultValue: ""),
                    AnswerPictureExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false, defaultValue: ""),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SendForQuiz = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TryCount = table.Column<byte>(type: "smallint", nullable: false, defaultValue: (byte)0),
                    GainId = table.Column<int>(type: "integer", nullable: true),
                    RightOption = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Gains_GainId",
                        column: x => x.GainId,
                        principalTable: "Gains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Questions_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Questions_Users_CreateUser",
                        column: x => x.CreateUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Token = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedByIp = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RevokedByIp = table.Column<string>(type: "citext", maxLength: 50, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "citext", maxLength: 100, nullable: true),
                    ReasonRevoked = table.Column<string>(type: "citext", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SimilarQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LessonId = table.Column<byte>(type: "smallint", nullable: false),
                    QuestionPicture = table.Column<string>(type: "citext", nullable: false),
                    QuestionPictureFileName = table.Column<string>(type: "citext", nullable: false),
                    QuestionPictureExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    ResponseQuestion = table.Column<string>(type: "citext", nullable: false),
                    ResponseQuestionFileName = table.Column<string>(type: "citext", nullable: false),
                    ResponseQuestionExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    ResponseAnswer = table.Column<string>(type: "citext", nullable: false),
                    ResponseAnswerFileName = table.Column<string>(type: "citext", nullable: false),
                    ResponseAnswerExtension = table.Column<string>(type: "citext", maxLength: 10, nullable: false),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    SendForQuiz = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    GainId = table.Column<int>(type: "integer", nullable: true),
                    RightOption = table.Column<char>(type: "character(1)", maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimilarQuestions_Gains_GainId",
                        column: x => x.GainId,
                        principalTable: "Gains",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SimilarQuestions_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SimilarQuestions_Users_CreateUser",
                        column: x => x.CreateUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserOperationClaims",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OperationClaimId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOperationClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOperationClaims_OperationClaims_OperationClaimId",
                        column: x => x.OperationClaimId,
                        principalTable: "OperationClaims",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserOperationClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreateUser = table.Column<long>(type: "bigint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateUser = table.Column<long>(type: "bigint", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Name = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    TcNo = table.Column<string>(type: "citext", maxLength: 11, nullable: false),
                    Phone = table.Column<string>(type: "citext", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "citext", maxLength: 100, nullable: false),
                    Branch = table.Column<string>(type: "citext", maxLength: 50, nullable: false),
                    SchoolId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Teachers_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
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

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Sözel", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Sayısal", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "CreateDate", "CreateUser", "IsActive", "Name", "SortNo", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Türk Dili ve Edebiyatı", (byte)1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Matematik", (byte)2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "İngilizce", (byte)3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Fizik", (byte)4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Kimya", (byte)5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Biyoloji", (byte)6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Geometri", (byte)7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Tarih", (byte)8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Coğrafya", (byte)9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)10, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Felsefe", (byte)10, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { (byte)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, "Din Kültürü ve Ahlak Bilgisi", (byte)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "OperationClaims",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "Schools",
                columns: new[] { "Id", "Address", "AuthorizedEmail", "AuthorizedName", "AuthorizedPhone", "City", "CreateDate", "CreateUser", "IsActive", "LicenseEndDate", "Name", "TaxNumber", "UpdateDate", "UpdateUser", "UserCount" },
                values: new object[] { 1, "TeknoPark", "okul@mail.com", "Yetkili", "5999999999", "Gaziantep", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, new DateTime(2050, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hamster Koleji", "9999999999", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, 157 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "CreateDate", "Email", "IsActive", "MustPasswordChange", "Name", "PasswordHash", "PasswordSalt", "Phone", "ProfileUrl", "SchoolId", "Surname", "Type", "UserName" },
                values: new object[,]
                {
                    { 1L, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "root@mail.com", true, false, "Root", new byte[] { 169, 145, 33, 161, 147, 58, 15, 169, 19, 31, 236, 3, 128, 147, 151, 45, 188, 253, 68, 70, 153, 152, 73, 88, 253, 225, 151, 194, 216, 163, 110, 253, 172, 109, 2, 180, 132, 19, 48, 181, 217, 89, 227, 138, 159, 251, 96, 176, 113, 54, 11, 219, 157, 136, 251, 120, 124, 56, 153, 29, 36, 35, 129, 141 }, new byte[] { 22, 25, 49, 68, 114, 216, 25, 253, 239, 196, 230, 130, 40, 214, 153, 94, 28, 188, 154, 225, 50, 31, 161, 21, 4, 230, 179, 118, 232, 155, 171, 114, 197, 6, 252, 53, 35, 172, 165, 92, 20, 162, 101, 242, 248, 163, 238, 160, 154, 196, 49, 79, 75, 39, 86, 23, 235, 103, 53, 30, 125, 117, 85, 109, 131, 66, 2, 219, 134, 223, 230, 64, 180, 36, 225, 254, 237, 167, 255, 137, 54, 86, 113, 27, 104, 47, 172, 200, 53, 23, 217, 143, 228, 57, 211, 92, 242, 99, 140, 90, 93, 1, 134, 181, 53, 38, 226, 125, 45, 80, 44, 8, 43, 67, 20, 84, 161, 155, 150, 7, 31, 182, 239, 204, 76, 162, 82, 81 }, "5000000001", null, null, "Kullanıcı", (byte)1, "root" },
                    { 2L, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@mail.com", true, false, "Admin", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000002", null, null, "Kullanıcı", (byte)1, "Admin" },
                    { 15L, null, new DateTime(2024, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "kazimyildirimeng@gmail.com", true, false, "Kazım", new byte[] { 186, 31, 96, 48, 95, 78, 158, 166, 85, 180, 187, 134, 246, 224, 115, 214, 111, 80, 51, 192, 138, 129, 200, 8, 1, 31, 183, 138, 110, 232, 23, 68, 117, 97, 147, 141, 33, 52, 101, 79, 162, 46, 153, 107, 207, 73, 111, 234, 192, 119, 232, 192, 21, 66, 179, 216, 87, 72, 216, 181, 95, 59, 109, 162 }, new byte[] { 174, 222, 90, 210, 27, 13, 26, 160, 176, 57, 91, 39, 224, 32, 135, 218, 59, 222, 74, 61, 12, 41, 215, 48, 59, 181, 35, 162, 42, 142, 223, 232, 224, 172, 216, 100, 255, 252, 82, 87, 138, 99, 90, 181, 169, 189, 219, 44, 46, 161, 190, 185, 145, 56, 27, 69, 79, 138, 117, 62, 193, 77, 101, 124, 35, 4, 133, 97, 27, 239, 210, 160, 152, 223, 205, 92, 141, 5, 252, 162, 186, 38, 248, 210, 252, 119, 53, 66, 33, 157, 253, 74, 164, 131, 117, 233, 172, 99, 167, 200, 54, 59, 162, 8, 126, 247, 95, 97, 143, 181, 226, 132, 117, 168, 71, 54, 38, 229, 15, 196, 150, 93, 138, 167, 89, 254, 27, 124 }, "5413695228", null, null, "Yıldırım", (byte)1, "kazim" }
                });

            migrationBuilder.InsertData(
                table: "ClassRooms",
                columns: new[] { "Id", "Branch", "CreateDate", "CreateUser", "IsActive", "No", "SchoolId", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { 1, "A", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (short)1, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 2, "B", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (short)1, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 3, "C", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, true, (short)1, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

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
                    { new Guid("a1a84a26-a7e4-4671-a979-d65fbbbedeca"), new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, (byte)1, true, (byte)11, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "Branch", "CreateDate", "CreateUser", "Email", "IsActive", "Name", "Phone", "SchoolId", "StudentId", "Surname", "TcNo", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { 1, "Matematik", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "hoca1@mail.com", true, "Öğretmen 1", "5000000004", 1, null, "Kullanıcı", "11111111111", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 2, "Türkçe", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "hoca2@mail.com", true, "Öğretmen 2", "5000000005", 1, null, "Kullanıcı", "22222222222", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

            migrationBuilder.InsertData(
                table: "UserOperationClaims",
                columns: new[] { "Id", "OperationClaimId", "UserId" },
                values: new object[] { -1L, 1, 1L });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ConnectionId", "CreateDate", "Email", "IsActive", "MustPasswordChange", "Name", "PasswordHash", "PasswordSalt", "Phone", "ProfileUrl", "SchoolId", "Surname", "Type", "UserName" },
                values: new object[,]
                {
                    { 3L, null, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "okul@mail.com", true, false, "Okul", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000003", null, 1, "Kullanıcı", (byte)2, "Okul" },
                    { 4L, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hoca1@mail.com", true, false, "Öğretmen 1", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000004", null, 1, "Kullanıcı", (byte)3, "Hoca1" },
                    { 5L, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hoca2@mail.com", true, false, "Öğretmen 2", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000005", null, 1, "Kullanıcı", (byte)3, "Hoca2" },
                    { 6L, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogrenci1@mail.com", true, false, "Öğrenci 1", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000006", null, 1, "Kullanıcı", (byte)4, "Öğrenci1" },
                    { 7L, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogrenci2@mail.com", true, false, "Öğrenci 2", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000007", null, 1, "Kullanıcı", (byte)4, "Öğrenci2" },
                    { 8L, 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogrenci3@mail.com", true, false, "Öğrenci 3", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000008", null, 1, "Kullanıcı", (byte)4, "Öğrenci3" },
                    { 9L, 4, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogrenci4@mail.com", true, false, "Öğrenci 4", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000009", null, 1, "Kullanıcı", (byte)4, "Öğrenci4" },
                    { 10L, 5, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogrenci5@mail.com", true, false, "Öğrenci 5", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000010", null, 1, "Kullanıcı", (byte)4, "Öğrenci5" },
                    { 11L, 6, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogrenci6@mail.com", true, false, "Öğrenci 6", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5000000011", null, 1, "Kullanıcı", (byte)4, "Öğrenci6" },
                    { 12L, 7, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ozancank@gmail.com", true, false, "Ozan Can", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5069151010", null, 1, "Kösemez", (byte)4, "ozancank@gmail.com" },
                    { 13L, 8, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "942alicankesen@gmail.com", true, false, "Alican", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5313914388", null, 1, "Kesen", (byte)4, "942alicankesen@gmail.com" },
                    { 14L, 9, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "balcan1905@gmail.com", true, false, "Eyüp", new byte[] { 91, 194, 62, 143, 170, 150, 7, 228, 50, 239, 166, 107, 207, 24, 2, 7, 203, 74, 96, 38, 172, 98, 246, 47, 95, 139, 182, 129, 194, 123, 154, 92, 176, 160, 43, 225, 40, 197, 64, 221, 241, 92, 215, 64, 33, 0, 43, 23, 69, 19, 45, 253, 198, 229, 249, 127, 17, 216, 37, 153, 50, 186, 168, 162 }, new byte[] { 110, 138, 57, 138, 8, 214, 100, 204, 96, 244, 112, 122, 216, 29, 143, 233, 207, 44, 246, 94, 145, 242, 43, 105, 129, 206, 65, 48, 233, 219, 35, 237, 138, 38, 46, 252, 49, 89, 130, 30, 31, 164, 44, 32, 185, 212, 83, 225, 98, 112, 163, 142, 69, 255, 194, 130, 80, 230, 18, 42, 105, 158, 161, 163, 212, 99, 63, 48, 166, 190, 0, 193, 209, 227, 88, 214, 227, 127, 237, 209, 34, 245, 113, 202, 224, 237, 193, 49, 143, 88, 2, 63, 145, 186, 148, 230, 187, 10, 74, 170, 207, 173, 100, 18, 117, 202, 224, 138, 24, 82, 148, 101, 188, 135, 109, 153, 7, 7, 30, 140, 252, 99, 195, 195, 20, 24, 253, 151 }, "5550593005", null, 1, "Balcan", (byte)4, "balcan1905@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "ClassRoomId", "CreateDate", "CreateUser", "Email", "IsActive", "Name", "Phone", "StudentNo", "Surname", "TcNo", "UpdateDate", "UpdateUser" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ogrenci1@mail.com", true, "Öğrenci 1", "5000000006", "001", "Kullanıcı", "33333333333", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 2, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ogrenci2@mail.com", true, "Öğrenci 2", "5000000007", "002", "Kullanıcı", "44444444444", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 3, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ogrenci3@mail.com", true, "Öğrenci 3", "5000000008", "003", "Kullanıcı", "55555555555", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 4, 2, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ogrenci4@mail.com", true, "Öğrenci 4", "5000000009", "004", "Kullanıcı", "66666666666", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 5, 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ogrenci5@mail.com", true, "Öğrenci 5", "5000000010", "005", "Kullanıcı", "77777777777", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 6, 3, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ogrenci6@mail.com", true, "Öğrenci 6", "5000000011", "006", "Kullanıcı", "88888888888", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 7, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "ozancank@gmail.com", true, "Ozan Can", "5069151010", "007", "Kösemez", "12312312399", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 8, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "942alicankesen@gmail.com", true, "Alican", "5313914388", "008", "Kesen", "12312312388", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L },
                    { 9, 1, new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L, "balcan1905@gmail.com", true, "Eyüp", "5550593005", "009", "Balcan", "12312312377", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2L }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ClassRooms_1",
                table: "ClassRooms",
                columns: new[] { "No", "Branch", "SchoolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassRooms_SchoolId",
                table: "ClassRooms",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Gains_1",
                table: "Gains",
                columns: new[] { "Name", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gains_LessonId",
                table: "Gains",
                column: "LessonId");

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
                name: "IX_NotificationDeviceTokens_UserId",
                table: "NotificationDeviceTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordTokens_Token",
                table: "PasswordTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordTokens_UserId",
                table: "PasswordTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CreateUser",
                table: "Questions",
                column: "CreateUser");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GainId",
                table: "Questions",
                column: "GainId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LessonId",
                table: "Questions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UK_Schools_Name",
                table: "Schools",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Schools_TaxNumber",
                table: "Schools",
                column: "TaxNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SimilarQuestions_CreateUser",
                table: "SimilarQuestions",
                column: "CreateUser");

            migrationBuilder.CreateIndex(
                name: "IX_SimilarQuestions_GainId",
                table: "SimilarQuestions",
                column: "GainId");

            migrationBuilder.CreateIndex(
                name: "IX_SimilarQuestions_LessonId",
                table: "SimilarQuestions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassRoomId",
                table: "Students",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "UK_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Students_Phone",
                table: "Students",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Students_TcNo",
                table: "Students",
                column: "TcNo",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_SchoolId",
                table: "Teachers",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_StudentId",
                table: "Teachers",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "UK_Teachers_Email",
                table: "Teachers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Teachers_Phone",
                table: "Teachers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Teachers_TcNo",
                table: "Teachers",
                column: "TcNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserOperationClaims_OperationClaimId",
                table: "UserOperationClaims",
                column: "OperationClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOperationClaims_UserId",
                table: "UserOperationClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SchoolId",
                table: "Users",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "UK_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Users_Phone",
                table: "Users",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonGroups");

            migrationBuilder.DropTable(
                name: "NotificationDeviceTokens");

            migrationBuilder.DropTable(
                name: "PasswordTokens");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "SimilarQuestions");

            migrationBuilder.DropTable(
                name: "TeacherClassRooms");

            migrationBuilder.DropTable(
                name: "TeacherLessons");

            migrationBuilder.DropTable(
                name: "UserOperationClaims");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Gains");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "OperationClaims");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "ClassRooms");

            migrationBuilder.DropTable(
                name: "Schools");
        }
    }
}
