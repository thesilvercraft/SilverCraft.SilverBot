using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    public partial class Allowthetranslationofremindercontent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GiveawayItemNull",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiveawayResultsNoReactions",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiveawayResultsWon",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PollErrorQuestionNull",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PollResultsResultNo",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PollResultsResultUndecided",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PollResultsResultYes",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PollResultsStart",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueueNothing",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReminderContent",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_GiveawayItemNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_GiveawayResultsNoReactions",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_GiveawayResultsWon",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_PollErrorQuestionNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultNo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultUndecided",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultYes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_PollResultsStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_QueueNothing",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReminderContent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GiveawayItemNull",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "GiveawayResultsNoReactions",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "GiveawayResultsWon",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "PollErrorQuestionNull",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "PollResultsResultNo",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "PollResultsResultUndecided",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "PollResultsResultYes",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "PollResultsStart",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "QueueNothing",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReminderContent",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_GiveawayItemNull",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_GiveawayResultsNoReactions",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_GiveawayResultsWon",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_PollErrorQuestionNull",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_PollResultsResultNo",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_PollResultsResultUndecided",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_PollResultsResultYes",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_PollResultsStart",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_QueueNothing",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReminderContent",
                table: "translatorSettings");
        }
    }
}
