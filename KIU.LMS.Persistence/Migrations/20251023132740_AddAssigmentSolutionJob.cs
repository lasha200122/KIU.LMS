using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigmentSolutionJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValidationsCount",
                table: "Assignment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssignmentSolutionJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Meta = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Result = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_AssignmentSolutionJob", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSolutionJob_AssignmentId",
                table: "AssignmentSolutionJob",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSolutionJob_CreateDate",
                table: "AssignmentSolutionJob",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSolutionJob_SolutionId",
                table: "AssignmentSolutionJob",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentSolutionJob_Status_Attempts",
                table: "AssignmentSolutionJob",
                columns: new[] { "Status", "Attempts" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentSolutionJob");

            migrationBuilder.DropColumn(
                name: "ValidationsCount",
                table: "Assignment");
        }
    }
}
