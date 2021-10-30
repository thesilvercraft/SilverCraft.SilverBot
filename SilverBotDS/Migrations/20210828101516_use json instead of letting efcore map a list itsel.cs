using Microsoft.EntityFrameworkCore.Migrations;

namespace SilverBotDS.Migrations
{
    public partial class usejsoninsteadoflettingefcoremapalistitsel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerStatString");

            migrationBuilder.AddColumn<string>(
                name: "ServerStatsTemplatesInJson",
                table: "serverSettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServerStatsTemplatesInJson",
                table: "serverSettings");

            migrationBuilder.CreateTable(
                name: "ServerStatString",
                columns: table => new
                {
                    Key = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerSettingsServerId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Template = table.Column<string>(type: "TEXT", nullable: true)
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
    }
}