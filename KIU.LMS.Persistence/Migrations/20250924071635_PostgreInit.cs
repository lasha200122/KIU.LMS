using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PostgreInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Variables = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectId = table.Column<string>(type: "text", nullable: false),
                    ObjectType = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prompt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompt", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    PasswordSalt = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Institution = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseMaterial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseMaterialParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMaterial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMaterial_CourseMaterial_CourseMaterialParentId",
                        column: x => x.CourseMaterialParentId,
                        principalTable: "CourseMaterial",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseMaterial_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMeeting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMeeting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMeeting_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Module_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmailQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Variables = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FailureReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailQueue_EmailTemplate_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "EmailTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttempt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DeviceIdentifier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCourse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CanAccessTill = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCourse_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourse_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceIdentifier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevice_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModuleBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleBank_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBank_Module_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Module",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    StartDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Score = table.Column<decimal>(type: "numeric", nullable: true),
                    Problem = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    AIGrader = table.Column<bool>(type: "boolean", nullable: false),
                    SolutionType = table.Column<int>(type: "integer", nullable: false),
                    PromptId = table.Column<Guid>(type: "uuid", nullable: true),
                    FullScreen = table.Column<bool>(type: "boolean", nullable: false),
                    RuntimeAttempt = table.Column<int>(type: "integer", nullable: true),
                    IsTraining = table.Column<bool>(type: "boolean", nullable: false),
                    PromptText = table.Column<string>(type: "text", nullable: true),
                    CodeSolution = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignment_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignment_Prompt_PromptId",
                        column: x => x.PromptId,
                        principalTable: "Prompt",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assignment_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: true),
                    StartDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Score = table.Column<decimal>(type: "numeric", nullable: true),
                    Explanation = table.Column<bool>(type: "boolean", nullable: false),
                    TimePerQuestion = table.Column<int>(type: "integer", nullable: true),
                    MinusScore = table.Column<decimal>(type: "numeric", nullable: true),
                    PublicTill = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsTraining = table.Column<bool>(type: "boolean", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiz_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quiz_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CodeSolution = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CodeGenerationPrompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CodeGraidingPrompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Solution = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Difficulty = table.Column<int>(type: "integer", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubModule_ModuleBank_ModuleBankId",
                        column: x => x.ModuleBankId,
                        principalTable: "ModuleBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    Grade = table.Column<string>(type: "text", nullable: false),
                    FeedBack = table.Column<string>(type: "text", nullable: false),
                    GradingStatus = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solution_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Solution_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExamResult",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Score = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalQuestions = table.Column<int>(type: "integer", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamResult_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExamResult_User_StudentId",
                        column: x => x.StudentId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuizBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    QuizId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionBankId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizBank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizBank_QuestionBank_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "QuestionBank",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuizBank_Quiz_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quiz",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CourseId",
                table: "Assignment",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_CreateDate",
                table: "Assignment",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_PromptId",
                table: "Assignment",
                column: "PromptId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_TopicId",
                table: "Assignment",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreateDate",
                table: "Course",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterial_CourseId",
                table: "CourseMaterial",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterial_CourseMaterialParentId",
                table: "CourseMaterial",
                column: "CourseMaterialParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterial_CreateDate",
                table: "CourseMaterial",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMeeting_CourseId",
                table: "CourseMeeting",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMeeting_CreateDate",
                table: "CourseMeeting",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueue_CreateDate",
                table: "EmailQueue",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueue_TemplateId",
                table: "EmailQueue",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplate_CreateDate",
                table: "EmailTemplate",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResult_CreateDate",
                table: "ExamResult",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResult_QuizId",
                table: "ExamResult",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamResult_StudentId",
                table: "ExamResult",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileRecord_CreateDate",
                table: "FileRecord",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_CreateDate",
                table: "LoginAttempt",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_UserId",
                table: "LoginAttempt",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseId",
                table: "Module",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CreateDate",
                table: "Module",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleBank_CreateDate",
                table: "ModuleBank",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleBank_ModuleId",
                table: "ModuleBank",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompt_CreateDate",
                table: "Prompt",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_CreateDate",
                table: "QuestionBank",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_ModuleId",
                table: "QuestionBank",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_CourseId",
                table: "Quiz",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_CreateDate",
                table: "Quiz",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_TopicId",
                table: "Quiz",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizBank_CreateDate",
                table: "QuizBank",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_QuizBank_QuestionBankId",
                table: "QuizBank",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizBank_QuizId",
                table: "QuizBank",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Solution_AssignmentId",
                table: "Solution",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Solution_CreateDate",
                table: "Solution",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Solution_UserId",
                table: "Solution",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubModule_CreateDate",
                table: "SubModule",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_SubModule_ModuleBankId",
                table: "SubModule",
                column: "ModuleBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_CourseId",
                table: "Topic",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_CreateDate",
                table: "Topic",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_User_CreateDate",
                table: "User",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_CourseId",
                table: "UserCourse",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_CreateDate",
                table: "UserCourse",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourse_UserId",
                table: "UserCourse",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevice_CreateDate",
                table: "UserDevice",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevice_UserId",
                table: "UserDevice",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseMaterial");

            migrationBuilder.DropTable(
                name: "CourseMeeting");

            migrationBuilder.DropTable(
                name: "EmailQueue");

            migrationBuilder.DropTable(
                name: "ExamResult");

            migrationBuilder.DropTable(
                name: "FileRecord");

            migrationBuilder.DropTable(
                name: "LoginAttempt");

            migrationBuilder.DropTable(
                name: "QuizBank");

            migrationBuilder.DropTable(
                name: "Solution");

            migrationBuilder.DropTable(
                name: "SubModule");

            migrationBuilder.DropTable(
                name: "UserCourse");

            migrationBuilder.DropTable(
                name: "UserDevice");

            migrationBuilder.DropTable(
                name: "EmailTemplate");

            migrationBuilder.DropTable(
                name: "QuestionBank");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropTable(
                name: "Assignment");

            migrationBuilder.DropTable(
                name: "ModuleBank");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Prompt");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "Course");
        }
    }
}
