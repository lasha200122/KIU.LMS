using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "GeneratedAssignment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GeneratedTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CodeSolution = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CodeGenerationPrompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CodeGradingPrompt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
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
                    table.PrimaryKey("PK_GeneratedTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedTask_GeneratedAssignment_GeneratedAssignmentId",
                        column: x => x.GeneratedAssignmentId,
                        principalTable: "GeneratedAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedTask_CreateDate",
                table: "GeneratedTask",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedTask_GeneratedAssignmentId",
                table: "GeneratedTask",
                column: "GeneratedAssignmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedTask");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "GeneratedAssignment");
        }
    }
}
