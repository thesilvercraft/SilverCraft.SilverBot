using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    /// <inheritdoc />
    public partial class ModuleEnablingPerServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmotesOptin",
                table: "serverSettings");

            migrationBuilder.DropColumn(
                name: "Reminders",
                table: "serverSettings");

            migrationBuilder.AddColumn<string>(
                name: "LavalinkNotSetup",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NetVipsLoadFail",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NothingInQueueToRemove",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnknownError",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_LavalinkNotSetup",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_NetVipsLoadFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueueToRemove",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_UnknownError",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LavalinkNotSetup",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "NetVipsLoadFail",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "NothingInQueueToRemove",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "UnknownError",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_LavalinkNotSetup",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_NetVipsLoadFail",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_NothingInQueueToRemove",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_UnknownError",
                table: "translatorSettings");

            migrationBuilder.AddColumn<bool>(
                name: "EmotesOptin",
                table: "serverSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Reminders",
                table: "serverSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
