using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SalesOrderOptionsAndPaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Options",
                schema: "app",
                table: "SalesOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                schema: "app",
                table: "SalesOrders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // demo orders seeded before: the same options and payments a fresh seed gives them
            migrationBuilder.Sql("""
                UPDATE app."SalesOrders" SET
                    "Options" = (CASE WHEN n % 5 = 0 THEN 1 ELSE 0 END) | (CASE WHEN n % 7 = 0 THEN 2 ELSE 0 END) | (CASE WHEN n % 3 = 0 THEN 4 ELSE 0 END) | (CASE WHEN n % 11 = 0 THEN 8 ELSE 0 END),
                    "Paid" = "Status" IN (2, 3) OR n % 2 = 0
                FROM (SELECT "Id" AS id, substring("Number" from 4)::int AS n FROM app."SalesOrders" WHERE "Number" ~ '^SO-[0-9]+$') AS o
                WHERE "Id" = o.id
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Options",
                schema: "app",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Paid",
                schema: "app",
                table: "SalesOrders");
        }
    }
}
