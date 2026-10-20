using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReportSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutgoingMailAttachments",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MailId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingMailAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutgoingMailAttachments_OutgoingMails_MailId",
                        column: x => x.MailId,
                        principalSchema: "cw",
                        principalTable: "OutgoingMails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportSubscriptions",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    Frequency = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeZone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Weekday = table.Column<int>(type: "integer", nullable: true),
                    DayOfMonth = table.Column<int>(type: "integer", nullable: true),
                    Filter = table.Column<string>(type: "jsonb", nullable: true),
                    Attachment = table.Column<bool>(type: "boolean", nullable: false),
                    NextRunAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportSubscriptions_Reports_ReportId",
                        column: x => x.ReportId,
                        principalSchema: "cw",
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingMailAttachments_MailId",
                schema: "cw",
                table: "OutgoingMailAttachments",
                column: "MailId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubscriptions_NextRunAt",
                schema: "cw",
                table: "ReportSubscriptions",
                column: "NextRunAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubscriptions_ReportId",
                schema: "cw",
                table: "ReportSubscriptions",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubscriptions_TenantId_UserId",
                schema: "cw",
                table: "ReportSubscriptions",
                columns: new[] { "TenantId", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutgoingMailAttachments",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "ReportSubscriptions",
                schema: "cw");
        }
    }
}
