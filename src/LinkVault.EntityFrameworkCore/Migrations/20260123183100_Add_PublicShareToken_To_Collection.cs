using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkVault.Migrations
{
    /// <inheritdoc />
    public partial class Add_PublicShareToken_To_Collection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicShareToken",
                table: "AppCollections",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicShareToken",
                table: "AppCollections");
        }
    }
}
