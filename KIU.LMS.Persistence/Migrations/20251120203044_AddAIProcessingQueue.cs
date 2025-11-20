using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KIU.LMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAIProcessingQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIProcessingQueue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    MetaData = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_AIProcessingQueue", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIProcessingQueue_CreateDate",
                table: "AIProcessingQueue",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_AIQueue_Status_Type",
                table: "AIProcessingQueue",
                columns: new[] { "Status", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIProcessingQueue");
        }
    }
}
