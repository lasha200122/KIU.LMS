using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class QuestionBankModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubModule_QuestionBank_QuestionBankId",
                table: "SubModule");

            migrationBuilder.DropIndex(
                name: "IX_SubModule_QuestionBankId",
                table: "SubModule");

            migrationBuilder.DropColumn(
                name: "QuestionBankId",
                table: "SubModule");

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId",
                table: "QuestionBank",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBank_ModuleId",
                table: "QuestionBank",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionBank_Module_ModuleId",
                table: "QuestionBank",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionBank_Module_ModuleId",
                table: "QuestionBank");

            migrationBuilder.DropIndex(
                name: "IX_QuestionBank_ModuleId",
                table: "QuestionBank");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "QuestionBank");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionBankId",
                table: "SubModule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubModule_QuestionBankId",
                table: "SubModule",
                column: "QuestionBankId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubModule_QuestionBank_QuestionBankId",
                table: "SubModule",
                column: "QuestionBankId",
                principalTable: "QuestionBank",
                principalColumn: "Id");
        }
    }
}
