using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    public partial class Beemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XPCommandLevel",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "XPCommandLevels",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_XPCommandLevel",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_XPCommandLevels",
                table: "translatorSettings");

            migrationBuilder.AddColumn<bool>(
                name: "UsesNewMusicPage",
                table: "userSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsesNewMusicPage",
                table: "userSettings");

            migrationBuilder.AddColumn<string>(
                name: "XPCommandLevel",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "XPCommandLevels",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_XPCommandLevel",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_XPCommandLevels",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);
        }
    }
}