using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    public partial class Addreminderstringstodb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ListReminderListMore",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListReminderNone",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ListReminderStart",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReminderErrorNoContent",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReminderSuccess",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ListReminderListMore",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ListReminderNone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ListReminderStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReminderErrorNoContent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReminderSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListReminderListMore",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ListReminderNone",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ListReminderStart",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReminderErrorNoContent",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReminderSuccess",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ListReminderListMore",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ListReminderNone",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ListReminderStart",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReminderErrorNoContent",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReminderSuccess",
                table: "translatorSettings");
        }
    }
}