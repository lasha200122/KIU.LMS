using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AssignmentTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AIGrader",
                table: "Assignment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SolutionType",
                table: "Assignment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIGrader",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "SolutionType",
                table: "Assignment");
        }
    }
}
