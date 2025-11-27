using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeyForVoting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Votes_OptionId",
                table: "Votes",
                column: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_VotingOptions_OptionId",
                table: "Votes",
                column: "OptionId",
                principalTable: "VotingOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_VotingOptions_OptionId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_OptionId",
                table: "Votes");
        }
    }
}
