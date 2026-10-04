using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CoworkeeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationDigests",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastSentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDigests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ReadAt_CreatedAt",
                schema: "cw",
                table: "Notifications",
                columns: new[] { "ReadAt", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDigests_UserId",
                schema: "cw",
                table: "NotificationDigests",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationDigests",
                schema: "cw");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ReadAt_CreatedAt",
                schema: "cw",
                table: "Notifications");
        }
    }
}
