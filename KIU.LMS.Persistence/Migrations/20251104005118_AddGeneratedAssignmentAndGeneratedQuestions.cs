using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratedAssignmentAndGeneratedQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneratedAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    Models = table.Column<List<string>>(type: "text[]", nullable: false),
                    TaskContent = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_GeneratedAssignment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedQuestion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionText = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    OptionA = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OptionB = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OptionC = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    OptionD = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExplanationCorrect = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ExplanationIncorrect = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
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
                    table.PrimaryKey("PK_GeneratedQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedQuestion_GeneratedAssignment_GeneratedAssignmentId",
                        column: x => x.GeneratedAssignmentId,
                        principalTable: "GeneratedAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedAssignment_CreateDate",
                table: "GeneratedAssignment",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedQuestion_CreateDate",
                table: "GeneratedQuestion",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedQuestion_GeneratedAssignmentId",
                table: "GeneratedQuestion",
                column: "GeneratedAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedQuestion");

            migrationBuilder.DropTable(
                name: "GeneratedAssignment");
        }
    }
}
