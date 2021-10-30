using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SilverBotDS.Migrations
{
    public partial class Addeventsindatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plannedEvents",
                columns: table => new
                {
                    EventID = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UserID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Handled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plannedEvents", x => x.EventID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "plannedEvents");
        }
    }
}