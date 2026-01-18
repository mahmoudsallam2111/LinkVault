using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkVault.Migrations
{
    /// <inheritdoc />
    public partial class Added_UserEmailPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserEmailPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Newsletter = table.Column<bool>(type: "boolean", nullable: false),
                    LinkSharing = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityAlerts = table.Column<bool>(type: "boolean", nullable: false),
                    WeeklyDigest = table.Column<bool>(type: "boolean", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserEmailPreferences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserEmailPreferences_UserId",
                table: "AppUserEmailPreferences",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserEmailPreferences");
        }
    }
}
