using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverBotDS.Migrations
{
    public partial class addtoggleforrepeatedthings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RepeatThings",
                table: "serverSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepeatThings",
                table: "serverSettings");
        }
    }
}