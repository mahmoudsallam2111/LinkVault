using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkVault.Migrations
{
    /// <inheritdoc />
    public partial class AddLinkReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppLinkReminders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LinkId = table.Column<Guid>(type: "uuid", nullable: false),
                    RemindAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsTriggered = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TriggeredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppLinkReminders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppLinkReminders_AppLinks_LinkId",
                        column: x => x.LinkId,
                        principalTable: "AppLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserReminderSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultReminderHours = table.Column<int>(type: "integer", nullable: false, defaultValue: 24),
                    EnableInAppNotifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EnableEmailNotifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserReminderSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppLinkReminders_IsTriggered_RemindAt",
                table: "AppLinkReminders",
                columns: new[] { "IsTriggered", "RemindAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLinkReminders_LinkId_IsTriggered",
                table: "AppLinkReminders",
                columns: new[] { "LinkId", "IsTriggered" });

            migrationBuilder.CreateIndex(
                name: "IX_AppLinkReminders_UserId",
                table: "AppLinkReminders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserReminderSettings_UserId",
                table: "AppUserReminderSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppLinkReminders");

            migrationBuilder.DropTable(
                name: "AppUserReminderSettings");
        }
    }
}
