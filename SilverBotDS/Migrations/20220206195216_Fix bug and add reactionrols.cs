using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    public partial class Fixbugandaddreactionrols : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttributeDataBaseCheckNoDirectMessages",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttributeDataBaseCheckWebShot",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleDone",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleEmbedColour",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleIntro",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleMainLoop",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleNoPermManageRoles",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleNone",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleResponseNo",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleResponseNo2",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleResponseNo3",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleResponseYes",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleResponseYes2",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleResponseYes3",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleRolesAdded",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReactionRoleTitle",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckNoDirectMessages",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckWebShot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleDone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleEmbedColour",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleIntro",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleMainLoop",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleNoPermManageRoles",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleNone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo2",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo3",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes2",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes3",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleRolesAdded",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributeDataBaseCheckNoDirectMessages",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "AttributeDataBaseCheckWebShot",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleDone",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleEmbedColour",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleIntro",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleMainLoop",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleNoPermManageRoles",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleNone",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleResponseNo",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleResponseNo2",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleResponseNo3",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleResponseYes",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleResponseYes2",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleResponseYes3",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleRolesAdded",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "ReactionRoleTitle",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckNoDirectMessages",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckWebShot",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleDone",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleEmbedColour",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleIntro",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleMainLoop",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleNoPermManageRoles",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleNone",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo2",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo3",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes2",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes3",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleRolesAdded",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_ReactionRoleTitle",
                table: "translatorSettings");
        }
    }
}