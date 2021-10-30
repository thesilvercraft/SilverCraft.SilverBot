using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverBotDS.Migrations
{
    public partial class addwebshottoserversettngs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WebShot",
                table: "serverSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebShot",
                table: "serverSettings");
        }
    }
}