using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SoftDeleteWebhooksApprovalState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Brands_TenantId_Name",
                schema: "app",
                table: "Brands");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalState",
                schema: "app",
                table: "SalesOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Open");

            // orders whose approval ran before the state was mirrored
            migrationBuilder.Sql("""
                UPDATE app."SalesOrders" AS o SET "ApprovalState" = w."State"
                FROM cw."WorkflowItems" AS w
                WHERE w."EntityType" = 'SalesOrders' AND w."EntityId" = o."Id" AND w."TenantId" = o."TenantId";
                """);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                schema: "app",
                table: "Products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "app",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                schema: "app",
                table: "Brands",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "app",
                table: "Brands",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "app",
                table: "Brands",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WebhookDeliveries",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    NextAttemptAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ResponseStatus = table.Column<int>(type: "integer", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    Error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Request = table.Column<string>(type: "text", nullable: true),
                    Response = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookDeliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WebhookSubscriptions",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Events = table.Column<List<string>>(type: "text[]", nullable: false),
                    ProtectedSecret = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    PreviousProtectedSecret = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    PreviousSecretUntil = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsecutiveFailures = table.Column<int>(type: "integer", nullable: false),
                    DisabledAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_TenantId_Name",
                schema: "app",
                table: "Brands",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_SubscriptionId_EventId",
                schema: "cw",
                table: "WebhookDeliveries",
                columns: new[] { "SubscriptionId", "EventId" });

            migrationBuilder.CreateIndex(
                name: "IX_WebhookDeliveries_TenantId_SubscriptionId_CreatedAt",
                schema: "cw",
                table: "WebhookDeliveries",
                columns: new[] { "TenantId", "SubscriptionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WebhookSubscriptions_TenantId_IsActive",
                schema: "cw",
                table: "WebhookSubscriptions",
                columns: new[] { "TenantId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebhookDeliveries",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "WebhookSubscriptions",
                schema: "cw");

            migrationBuilder.DropIndex(
                name: "IX_Brands_TenantId_Name",
                schema: "app",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "ApprovalState",
                schema: "app",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "app",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "app",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "app",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                schema: "app",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "app",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "app",
                table: "Brands");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_TenantId_Name",
                schema: "app",
                table: "Brands",
                columns: new[] { "TenantId", "Name" },
                unique: true);
        }
    }
}
