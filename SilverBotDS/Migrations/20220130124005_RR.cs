using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace SilverBotDS.Migrations
{
    public partial class RR : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LangCodeForUselessFacts",
                table: "translatorSettings_CustomLanguages",
                newName: "XPCommandCardSuccess");

            migrationBuilder.RenameColumn(
                name: "CurrentCustomLanguage_LangCodeForUselessFacts",
                table: "translatorSettings",
                newName: "CurrentCustomLanguage_XPCommandCardSuccess");

            migrationBuilder.CreateTable(
                name: "ReactionRoleMappings",
                columns: table => new
                {
                    MappingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Emoji = table.Column<string>(type: "TEXT", nullable: true),
                    EmojiId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Mode = table.Column<ushort>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReactionRoleMappings", x => x.MappingId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReactionRoleMappings");

            migrationBuilder.RenameColumn(
                name: "XPCommandCardSuccess",
                table: "translatorSettings_CustomLanguages",
                newName: "LangCodeForUselessFacts");

            migrationBuilder.RenameColumn(
                name: "CurrentCustomLanguage_XPCommandCardSuccess",
                table: "translatorSettings",
                newName: "CurrentCustomLanguage_LangCodeForUselessFacts");
        }
    }
}