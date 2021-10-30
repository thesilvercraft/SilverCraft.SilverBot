using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SilverBotDS.Migrations
{
    public partial class adduserquotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "userQuotes",
                columns: table => new
                {
                    QuoteId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    QuoteContent = table.Column<string>(type: "TEXT", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userQuotes", x => x.QuoteId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userQuotes");
        }
    }
}