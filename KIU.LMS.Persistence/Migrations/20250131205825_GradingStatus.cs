using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class GradingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GradingStatus",
                table: "Solution",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GradingStatus",
                table: "Solution");
        }
    }
}
