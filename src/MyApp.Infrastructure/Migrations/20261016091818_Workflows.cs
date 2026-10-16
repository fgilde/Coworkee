using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Workflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkflowApprovals",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Link = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RequiresAll = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RemindAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RemindedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EscalatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowItems",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    From = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    To = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Transition = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    At = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowApprovers",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovalId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Approved = table.Column<bool>(type: "boolean", nullable: true),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DecidedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Escalation = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowApprovers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowApprovers_WorkflowApprovals_ApprovalId",
                        column: x => x.ApprovalId,
                        principalSchema: "cw",
                        principalTable: "WorkflowApprovals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowApprovals_Status_DueAt",
                schema: "cw",
                table: "WorkflowApprovals",
                columns: new[] { "Status", "DueAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowApprovals_TenantId_EntityType_EntityId_Status",
                schema: "cw",
                table: "WorkflowApprovals",
                columns: new[] { "TenantId", "EntityType", "EntityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowApprovers_ApprovalId",
                schema: "cw",
                table: "WorkflowApprovers",
                column: "ApprovalId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowApprovers_UserId_Approved",
                schema: "cw",
                table: "WorkflowApprovers",
                columns: new[] { "UserId", "Approved" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowItems_TenantId_EntityType_EntityId",
                schema: "cw",
                table: "WorkflowItems",
                columns: new[] { "TenantId", "EntityType", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowItems_TenantId_EntityType_State",
                schema: "cw",
                table: "WorkflowItems",
                columns: new[] { "TenantId", "EntityType", "State" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_TenantId_EntityType_EntityId_At",
                schema: "cw",
                table: "WorkflowSteps",
                columns: new[] { "TenantId", "EntityType", "EntityId", "At" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowApprovers",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "WorkflowItems",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "WorkflowSteps",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "WorkflowApprovals",
                schema: "cw");
        }
    }
}
