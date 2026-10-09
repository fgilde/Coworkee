using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FeaturesFilesSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EditedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cw",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Editions",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Values = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Editions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileFolders",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileFolders_FileFolders_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cw",
                        principalTable: "FileFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stars = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.CheckConstraint("CK_Ratings_Stars", "\"Stars\" BETWEEN 1 AND 5");
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFeatures",
                schema: "cw",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    EditionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Overrides = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantFeatures", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_TenantFeatures_Editions_EditionId",
                        column: x => x.EditionId,
                        principalSchema: "cw",
                        principalTable: "Editions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TenantFeatures_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "cw",
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContentType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    BlobKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredFiles_FileFolders_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "cw",
                        principalTable: "FileFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntityTags",
                schema: "cw",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "cw",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                schema: "cw",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TenantId_EntityType_EntityId_CreatedAt",
                schema: "cw",
                table: "Comments",
                columns: new[] { "TenantId", "EntityType", "EntityId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Editions_Name",
                schema: "cw",
                table: "Editions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_TagId_EntityId",
                schema: "cw",
                table: "EntityTags",
                columns: new[] { "TagId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EntityTags_TenantId_EntityId",
                schema: "cw",
                table: "EntityTags",
                columns: new[] { "TenantId", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_ParentId",
                schema: "cw",
                table: "FileFolders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileFolders_TenantId_ParentId",
                schema: "cw",
                table: "FileFolders",
                columns: new[] { "TenantId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_TenantId_EntityType_EntityId_UserId",
                schema: "cw",
                table: "Ratings",
                columns: new[] { "TenantId", "EntityType", "EntityId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_FolderId",
                schema: "cw",
                table: "StoredFiles",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_TenantId_FolderId",
                schema: "cw",
                table: "StoredFiles",
                columns: new[] { "TenantId", "FolderId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TenantId_EntityType_Name",
                schema: "cw",
                table: "Tags",
                columns: new[] { "TenantId", "EntityType", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantFeatures_EditionId",
                schema: "cw",
                table: "TenantFeatures",
                column: "EditionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "EntityTags",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "Ratings",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "StoredFiles",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "TenantFeatures",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "FileFolders",
                schema: "cw");

            migrationBuilder.DropTable(
                name: "Editions",
                schema: "cw");
        }
    }
}
