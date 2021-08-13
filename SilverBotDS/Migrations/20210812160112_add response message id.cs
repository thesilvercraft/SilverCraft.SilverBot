using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverBotDS.Migrations
{
    public partial class addresponsemessageid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "ResponseMessageID",
                table: "plannedEvents",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseMessageID",
                table: "plannedEvents");
        }
    }
}
