using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverBotDS.Migrations
{
    public partial class Addcustomprefixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrefixesInJson",
                table: "serverSettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrefixesInJson",
                table: "serverSettings");
        }
    }
}
