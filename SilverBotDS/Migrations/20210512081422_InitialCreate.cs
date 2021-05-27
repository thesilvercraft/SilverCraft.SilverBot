using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverBotDS.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "serverSettings",
                columns: table => new
                {
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LangName = table.Column<string>(type: "TEXT", nullable: true),
                    EmotesOptin = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerStatsCategoryId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serverSettings", x => x.ServerId);
                });

            migrationBuilder.CreateTable(
                name: "userSettings",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LangName = table.Column<string>(type: "TEXT", nullable: true),
                    IsBanned = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerStatString",
                columns: table => new
                {
                    Key = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Template = table.Column<string>(type: "TEXT", nullable: true),
                    ServerSettingsServerId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerStatString", x => x.Key);
                    table.ForeignKey(
                        name: "FK_ServerStatString_serverSettings_ServerSettingsServerId",
                        column: x => x.ServerSettingsServerId,
                        principalTable: "serverSettings",
                        principalColumn: "ServerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServerStatString_ServerSettingsServerId",
                table: "ServerStatString",
                column: "ServerSettingsServerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerStatString");

            migrationBuilder.DropTable(
                name: "userSettings");

            migrationBuilder.DropTable(
                name: "serverSettings");
        }
    }
}