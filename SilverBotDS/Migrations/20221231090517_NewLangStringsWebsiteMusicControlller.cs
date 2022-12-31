using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    /// <inheritdoc />
    public partial class NewLangStringsWebsiteMusicControlller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteBLoopQueue",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteBLoopSong",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteBVolumeDown",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteBVolumeUp",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteForceSkip",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteLoopOff",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteLoopQueue",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteLoopSong",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteNoLoop",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsitePlayPause",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsitePlayerPaused",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsitePlayerResumed",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsitePlayingNothingTrackName",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteShuffle",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteShuffled",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteSkipped",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteSkippedViaVote",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteVoteSkip",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WebsiteVotedForSkip",
                table: "translatorSettings_CustomLanguages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandSelf",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandOther",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandLeaderBoardTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandLeaderBoardPerson",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandGeneralFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandFailSelf",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandFailOther",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandCardSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_WrongImageCount",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Voted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_VolumeNotCorrect",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_VersionNumber",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_VersionInfoTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Version",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Userid",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserNotConnected",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserIsntBot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserIsBannedFromSilversocial",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserHasLowerRole",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_User",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UselessFact",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UrbanExample",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UnknownImageFormat",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UnknownError",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Type",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TrackingStopped",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TrackingStarted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TrackCanNotBeSeeked",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TintSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimesTrackLooped",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimeWhenTrackPlayed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimeTillTrackPlays",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimeInUtc",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Success",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimePosition",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimeLeftSongLoopingCurrent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimeLeftSongLooping",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimeLeft",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongNotExist",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongLength",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongByAuthor",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SomethingsContributors",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SkippedNP",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SilverhostingJokeTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SilverhostingJokeDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SilverSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ShuffledSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SetToProvidedStrings",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SetToDefaultStrings",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Server",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchedFor",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchFailTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchFailDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Results",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ResizeSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireUserPermisionsCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireUserPermisionsCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireRolesCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireRolesCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireOwnerCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireNsfwCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireGuildCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireDJCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotPermisionsCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotPermisionsCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotAndUserPermisionsCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotAndUserPermisionsCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequestedBy",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedXSongOrSongs",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedSongs",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedSong",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedFront",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReminderSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReminderErrorNoContent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReminderContent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleRolesAdded",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes3",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes2",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo3",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo2",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleNone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleNoPermManageRoles",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleMainLoop",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleIntro",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleEmbedColour",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleDone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RandomGif",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuotePreviewQuoteID",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuotePreviewDeleteSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuoteGetNoQuoteWithId",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuoteGetNoBook",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QueueNothing",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgedBySilverBotReason",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeRemovedSingle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeRemovedPlural",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeRemovedFront",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeNumberNegative",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeNothingToDelete",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PrefixUsedTopgg",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PoweredByGiphy",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultYes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultUndecided",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultNo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollErrorQuestionNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PeriodExpired",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PageNuget",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PageGifButtonText",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PageGif",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OutputFileLargerThan8M",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OptedOutWebshot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OptedInWebshot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OptedIn",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OS",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NuGetVerified",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NowPlaying",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueueToRemove",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueueHistory",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueue",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotValidLanguage",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotPlaying",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotPaused",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotLooping",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotConnected",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotAvailableGameType",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoResults",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoPerm",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoMatchingSubcommandsAndGroupNotExecutable",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoImageGeneric",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoGamesWereReturnedDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoGamesWereReturned",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoEmotesFound",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NetVipsLoadFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_MultipleEmotesFound",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_MoreThanOneImageGeneric",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Meme",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_MathSteps",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LoopingSong",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LoopingQueue",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LoadedSilverBotPlaylistWithTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ListReminderStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ListReminderNone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ListReminderListMore",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Left",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LavalinkNotSetup",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LangName",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LangCodeGoogleTranslate",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Kick",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_JpegSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_JoinedSilverCraft",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Joined",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_IsDirty",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_IsAnOwner",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_IsABot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_InvalidOverload",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_InformationAbout",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentCustomLanguage_Id",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Hi",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandNoDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandHelpString",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupSubcommands",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupListingAllCommands",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupCanBeExecuted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupArguments",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupAliases",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GiveawayResultsWon",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GiveawayResultsNoReactions",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GiveawayItemNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GitRepo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GitCommitHash",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GitBranch",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GeneralException",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_FreeToPlayGameType",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Enqueued",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EnableRepeatedPhrases",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmoteWasLargerThan256K",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmojiMessageDownloadStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmojiMessageDownloadEnd",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmojiEnd",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_DsharpplusVersion",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Downloads",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_DisabledRepeatedPhrases",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_DblaReturnedNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CultureInfo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CostsMoneyGameTypeBug",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CommandIsDisabled",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ComicSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ChecksFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CategorySetSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderErrorNoEvent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderErrorMultiple",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderErrorAlreadyHandled",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CanForceSkip",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CLR",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_BotKickedUser",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_BotHasLowerRole",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_BotBannedUser",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Ban",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckWebShot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckNoDirectMessages",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AmericanMoney",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AlreadyVoted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AlreadyOptedIn",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AlreadyConnected",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AllAvailibleEmotes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AddedXAmountOfSongs",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AccountJoinDate",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AccountCreationDate",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteBLoopQueue",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteBLoopSong",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteBVolumeDown",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteBVolumeUp",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteForceSkip",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteLoopOff",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteLoopQueue",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteLoopSong",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteNoLoop",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsitePlayPause",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsitePlayerPaused",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsitePlayerResumed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsitePlayingNothingTrackName",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteShuffle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteShuffled",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteSkipped",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteSkippedViaVote",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteVoteSkip",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentCustomLanguage_WebsiteVotedForSkip",
                table: "translatorSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteBLoopQueue",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteBLoopSong",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteBVolumeDown",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteBVolumeUp",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteForceSkip",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteLoopOff",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteLoopQueue",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteLoopSong",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteNoLoop",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsitePlayPause",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsitePlayerPaused",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsitePlayerResumed",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsitePlayingNothingTrackName",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteShuffle",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteShuffled",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteSkipped",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteSkippedViaVote",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteVoteSkip",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "WebsiteVotedForSkip",
                table: "translatorSettings_CustomLanguages");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteBLoopQueue",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteBLoopSong",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteBVolumeDown",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteBVolumeUp",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteForceSkip",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteLoopOff",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteLoopQueue",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteLoopSong",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteNoLoop",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsitePlayPause",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsitePlayerPaused",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsitePlayerResumed",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsitePlayingNothingTrackName",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteShuffle",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteShuffled",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteSkipped",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteSkippedViaVote",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteVoteSkip",
                table: "translatorSettings");

            migrationBuilder.DropColumn(
                name: "CurrentCustomLanguage_WebsiteVotedForSkip",
                table: "translatorSettings");

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandSelf",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandOther",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandLeaderBoardTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandLeaderBoardPerson",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandGeneralFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandFailSelf",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandFailOther",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_XPCommandCardSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_WrongImageCount",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Voted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_VolumeNotCorrect",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_VersionNumber",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_VersionInfoTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Version",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Userid",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserNotConnected",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserIsntBot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserIsBannedFromSilversocial",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UserHasLowerRole",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_User",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UselessFact",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UrbanExample",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UnknownImageFormat",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_UnknownError",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Type",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TrackingStopped",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TrackingStarted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TrackCanNotBeSeeked",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TintSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimesTrackLooped",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimeWhenTrackPlayed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimeTillTrackPlays",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_TimeInUtc",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Success",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimePosition",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimeLeftSongLoopingCurrent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimeLeftSongLooping",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongTimeLeft",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongNotExist",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongLength",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SongByAuthor",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SomethingsContributors",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SkippedNP",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SilverhostingJokeTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SilverhostingJokeDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SilverSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ShuffledSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SetToProvidedStrings",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SetToDefaultStrings",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Server",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchedFor",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchFailTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchFailDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_SearchFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Results",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ResizeSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireUserPermisionsCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireUserPermisionsCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireRolesCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireRolesCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireOwnerCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireNsfwCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireGuildCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireDJCheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotPermisionsCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotPermisionsCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotAndUserPermisionsCheckFailedSG",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequireBotAndUserPermisionsCheckFailedPL",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RequestedBy",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedXSongOrSongs",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedSongs",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedSong",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RemovedFront",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReminderSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReminderErrorNoContent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReminderContent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleRolesAdded",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes3",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes2",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseYes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo3",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo2",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleResponseNo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleNone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleNoPermManageRoles",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleMainLoop",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleIntro",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleEmbedColour",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ReactionRoleDone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_RandomGif",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuotePreviewQuoteID",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuotePreviewDeleteSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuoteGetNoQuoteWithId",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QuoteGetNoBook",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_QueueNothing",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgedBySilverBotReason",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeRemovedSingle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeRemovedPlural",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeRemovedFront",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeNumberNegative",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PurgeNothingToDelete",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PrefixUsedTopgg",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PoweredByGiphy",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultYes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultUndecided",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollResultsResultNo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PollErrorQuestionNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PeriodExpired",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PageNuget",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PageGifButtonText",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_PageGif",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OutputFileLargerThan8M",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OptedOutWebshot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OptedInWebshot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OptedIn",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_OS",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NuGetVerified",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NowPlaying",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueueToRemove",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueueHistory",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NothingInQueue",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotValidLanguage",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotPlaying",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotPaused",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotLooping",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotConnected",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NotAvailableGameType",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoResults",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoPerm",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoMatchingSubcommandsAndGroupNotExecutable",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoImageGeneric",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoGamesWereReturnedDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoGamesWereReturned",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NoEmotesFound",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_NetVipsLoadFail",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_MultipleEmotesFound",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_MoreThanOneImageGeneric",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Meme",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_MathSteps",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LoopingSong",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LoopingQueue",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LoadedSilverBotPlaylistWithTitle",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ListReminderStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ListReminderNone",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ListReminderListMore",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Left",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LavalinkNotSetup",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LangName",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_LangCodeGoogleTranslate",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Kick",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_JpegSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_JoinedSilverCraft",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Joined",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_IsDirty",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_IsAnOwner",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_IsABot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_InvalidOverload",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_InformationAbout",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CurrentCustomLanguage_Id",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Hi",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandNoDescription",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandHelpString",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupSubcommands",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupListingAllCommands",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupCanBeExecuted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupArguments",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_HelpCommandGroupAliases",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GiveawayResultsWon",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GiveawayResultsNoReactions",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GiveawayItemNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GitRepo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GitCommitHash",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GitBranch",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_GeneralException",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_FreeToPlayGameType",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Enqueued",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EnableRepeatedPhrases",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmoteWasLargerThan256K",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmojiMessageDownloadStart",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmojiMessageDownloadEnd",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_EmojiEnd",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_DsharpplusVersion",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Downloads",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_DisabledRepeatedPhrases",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_DblaReturnedNull",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CultureInfo",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CostsMoneyGameTypeBug",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CommandIsDisabled",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ComicSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_ChecksFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CheckFailed",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CategorySetSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderSuccess",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderErrorNoEvent",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderErrorMultiple",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CancelReminderErrorAlreadyHandled",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CanForceSkip",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_CLR",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_BotKickedUser",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_BotHasLowerRole",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_BotBannedUser",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_Ban",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckWebShot",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AttributeDataBaseCheckNoDirectMessages",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AmericanMoney",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AlreadyVoted",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AlreadyOptedIn",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AlreadyConnected",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AllAvailibleEmotes",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AddedXAmountOfSongs",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AccountJoinDate",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentCustomLanguage_AccountCreationDate",
                table: "translatorSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
