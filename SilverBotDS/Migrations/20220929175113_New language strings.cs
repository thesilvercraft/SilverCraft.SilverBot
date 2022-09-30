using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    public partial class Newlanguagestrings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimesTrackLooped",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_TimesTrackLooped",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimesTrackLooped",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_TimesTrackLooped",
                table: "translatorSettings");
        }
    }
}
