using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Prompts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PromptId",
                table: "Assignment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Prompt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastUpdateUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleteDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeleteUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompt", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_PromptId",
                table: "Assignment",
                column: "PromptId");

            migrationBuilder.CreateIndex(
                name: "IX_Prompt_CreateDate",
                table: "Prompt",
                column: "CreateDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Prompt_PromptId",
                table: "Assignment",
                column: "PromptId",
                principalTable: "Prompt",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Prompt_PromptId",
                table: "Assignment");

            migrationBuilder.DropTable(
                name: "Prompt");

            migrationBuilder.DropIndex(
                name: "IX_Assignment_PromptId",
                table: "Assignment");

            migrationBuilder.DropColumn(
                name: "PromptId",
                table: "Assignment");
        }
    }
}
