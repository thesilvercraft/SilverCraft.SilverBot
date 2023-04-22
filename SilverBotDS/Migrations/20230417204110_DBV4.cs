using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilverBotDS.Migrations
{
    /// <inheritdoc />
    public partial class DBV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plannedEvents",
                columns: table => new
                {
                    EventID = table.Column<string>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    UserID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ChannelID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageID = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ResponseMessageID = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Data = table.Column<string>(type: "TEXT", nullable: false),
                    Handled = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plannedEvents", x => x.EventID);
                });

            migrationBuilder.CreateTable(
                name: "serverSettings",
                columns: table => new
                {
                    ServerSettingsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LangName = table.Column<string>(type: "TEXT", nullable: false),
                    ServerStatsCategoryId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    EnabledModules = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerStatsTemplatesInJson = table.Column<string>(type: "TEXT", nullable: false),
                    RepeatThings = table.Column<bool>(type: "INTEGER", nullable: false),
                    PrefixesInJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serverSettings", x => x.ServerSettingsId);
                });

            migrationBuilder.CreateTable(
                name: "translatorSettings",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsTranslator = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurrentCustomLanguage_Id = table.Column<Guid>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_LangName = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UnknownError = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_LangCodeGoogleTranslate = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Hi = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TimeInUtc = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CommandIsDisabled = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequestedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_DblaReturnedNull = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AlreadyConnected = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UserNotConnected = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_VolumeNotCorrect = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Joined = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NotConnected = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NotPlaying = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NothingInQueue = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NothingInQueueToRemove = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NothingInQueueHistory = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RemovedFront = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RemovedFrontWithAuthorName = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RemovedXSongOrSongs = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RemovedSong = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RemovedSongs = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_LoopingSong = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_LoopingQueue = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WrongImageCount = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NotLooping = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoResults = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Left = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_InformationAbout = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_JoinedSilverCraft = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PrefixUsedTopgg = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ShuffledSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_User = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Userid = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_IsAnOwner = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_IsABot = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AccountCreationDate = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AccountJoinDate = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SilverhostingJokeTitle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SilverhostingJokeDescription = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PurgeNumberNegative = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PurgeNothingToDelete = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PurgeRemovedFront = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PurgeRemovedSingle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PurgeRemovedPlural = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoEmotesFound = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SearchedFor = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_MultipleEmotesFound = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AlreadyOptedIn = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Server = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_OptedIn = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_OptedInWebshot = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_OptedOutWebshot = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AllAvailibleEmotes = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UserIsBannedFromSilversocial = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UselessFact = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UserHasLowerRole = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Ban = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Kick = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_BotHasLowerRole = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RandomGif = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PoweredByGiphy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Meme = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoImageGeneric = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_EmoteWasLargerThan256K = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_MoreThanOneImageGeneric = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_OutputFileLargerThan8M = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PageGif = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PageGifButtonText = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PageNuget = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PeriodExpired = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UserIsntBot = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NowPlaying = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NowPlayingBy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Enqueued = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_EnqueuedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SkippedNP = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CanForceSkip = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NotPaused = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Voted = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AlreadyVoted = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TimeTillTrackPlays = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TimeWhenTrackPlayed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TimesTrackLooped = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SearchFail = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SearchFailTitle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SearchFailDescription = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Success = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UrbanExample = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SongLength = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SongTimePosition = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SongTimeLeft = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SongTimeLeftSongLoopingCurrent = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SongTimeLeftSongLooping = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_LoadedSilverBotPlaylistWithTitle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SongNotExist = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_VersionInfoTitle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PurgedBySilverBotReason = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NotValidLanguage = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CultureInfo = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_BotBannedUser = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_BotKickedUser = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AddedXAmountOfSongs = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TrackingStarted = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TrackingStopped = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TrackCanNotBeSeeked = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandSelf = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandOther = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandGeneralFail = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandCardSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandFailSelf = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandFailOther = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandLeaderBoardTitle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_XPCommandLeaderBoardPerson = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_DisabledRepeatedPhrases = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_EnableRepeatedPhrases = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CheckFailed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ChecksFailed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_InvalidOverload = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GeneralException = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoMatchingSubcommandsAndGroupNotExecutable = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_QuotePreviewQuoteID = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_QuoteGetNoBook = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_QuoteGetNoQuoteWithId = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_QuotePreviewDeleteSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandHelpString = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandNoDescription = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandGroupCanBeExecuted = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandGroupAliases = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandGroupArguments = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandGroupSubcommands = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_HelpCommandGroupListingAllCommands = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireDJCheckFailed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireGuildCheckFailed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireNsfwCheckFailed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireOwnerCheckFailed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireRolesCheckFailedSG = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireRolesCheckFailedPL = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireBotPermisionsCheckFailedPL = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireBotPermisionsCheckFailedSG = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireUserPermisionsCheckFailedPL = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireUserPermisionsCheckFailedSG = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireBotAndUserPermisionsCheckFailedPL = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_RequireBotAndUserPermisionsCheckFailedSG = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_UnknownImageFormat = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NetVipsLoadFail = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AttributeDataBaseCheckNoDirectMessages = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AttributeDataBaseCheckWebShot = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_JpegSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SilverSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ComicSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ResizeSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_TintSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_MathSteps = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Results = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SomethingsContributors = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NuGetVerified = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Type = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Downloads = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_Version = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SetToProvidedStrings = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_SetToDefaultStrings = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoPerm = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CategorySetSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_EmojiMessageDownloadStart = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_EmojiMessageDownloadEnd = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_EmojiEnd = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleNoPermManageRoles = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleIntro = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleTitle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleResponseYes = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleResponseYes2 = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleResponseYes3 = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleResponseNo = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleResponseNo2 = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleResponseNo3 = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleRolesAdded = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleDone = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleNone = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleMainLoop = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReactionRoleEmbedColour = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_FreeToPlayGameType = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NotAvailableGameType = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CostsMoneyGameTypeBug = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoGamesWereReturned = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_NoGamesWereReturnedDescription = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AmericanMoney = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_OS = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_DsharpplusVersion = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_VersionNumber = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GitRepo = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GitCommitHash = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GitBranch = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_IsDirty = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CLR = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReminderErrorNoContent = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReminderSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ListReminderNone = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ListReminderStart = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ListReminderListMore = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_ReminderContent = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PollResultsStart = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PollResultsResultYes = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PollResultsResultNo = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PollResultsResultUndecided = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_PollErrorQuestionNull = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GiveawayResultsNoReactions = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GiveawayItemNull = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_GiveawayResultsWon = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_QueueNothing = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CancelReminderErrorNoEvent = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CancelReminderErrorMultiple = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CancelReminderErrorAlreadyHandled = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_CancelReminderSuccess = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_LavalinkNotSetup = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsitePlayerResumed = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsitePlayerPaused = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteSkippedViaVote = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteVotedForSkip = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteSkipped = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteShuffled = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteLoopOff = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteLoopSong = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteLoopQueue = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsitePlayingNothingTrackName = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsitePlayPause = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteVoteSkip = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteForceSkip = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteShuffle = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteNoLoop = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteBLoopSong = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteBLoopQueue = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteBVolumeDown = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_WebsiteBVolumeUp = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentCustomLanguage_AddedBingToChannel = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translatorSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userExperiences",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    XP = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userExperiences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "userQuotes",
                columns: table => new
                {
                    QuoteId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    QuoteContent = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userQuotes", x => x.QuoteId);
                });

            migrationBuilder.CreateTable(
                name: "userSettings",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LangName = table.Column<string>(type: "TEXT", nullable: false),
                    IsBanned = table.Column<bool>(type: "INTEGER", nullable: false),
                    UsesNewMusicPage = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatabaseWebhookData",
                columns: table => new
                {
                    DatabaseWebhookDataId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServerSettingsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseWebhookData", x => x.DatabaseWebhookDataId);
                    table.ForeignKey(
                        name: "FK_DatabaseWebhookData_serverSettings_ServerSettingsId",
                        column: x => x.ServerSettingsId,
                        principalTable: "serverSettings",
                        principalColumn: "ServerSettingsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReactionRoleMappings",
                columns: table => new
                {
                    MappingId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServerSettingsId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_ReactionRoleMappings_serverSettings_ServerSettingsId",
                        column: x => x.ServerSettingsId,
                        principalTable: "serverSettings",
                        principalColumn: "ServerSettingsId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "translatorSettings_CustomLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TranslatorSettingsId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LangName = table.Column<string>(type: "TEXT", nullable: false),
                    UnknownError = table.Column<string>(type: "TEXT", nullable: false),
                    LangCodeGoogleTranslate = table.Column<string>(type: "TEXT", nullable: false),
                    Hi = table.Column<string>(type: "TEXT", nullable: false),
                    TimeInUtc = table.Column<string>(type: "TEXT", nullable: false),
                    CommandIsDisabled = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedBy = table.Column<string>(type: "TEXT", nullable: false),
                    DblaReturnedNull = table.Column<string>(type: "TEXT", nullable: false),
                    AlreadyConnected = table.Column<string>(type: "TEXT", nullable: false),
                    UserNotConnected = table.Column<string>(type: "TEXT", nullable: false),
                    VolumeNotCorrect = table.Column<string>(type: "TEXT", nullable: false),
                    Joined = table.Column<string>(type: "TEXT", nullable: false),
                    NotConnected = table.Column<string>(type: "TEXT", nullable: false),
                    NotPlaying = table.Column<string>(type: "TEXT", nullable: false),
                    NothingInQueue = table.Column<string>(type: "TEXT", nullable: false),
                    NothingInQueueToRemove = table.Column<string>(type: "TEXT", nullable: false),
                    NothingInQueueHistory = table.Column<string>(type: "TEXT", nullable: false),
                    RemovedFront = table.Column<string>(type: "TEXT", nullable: false),
                    RemovedFrontWithAuthorName = table.Column<string>(type: "TEXT", nullable: false),
                    RemovedXSongOrSongs = table.Column<string>(type: "TEXT", nullable: false),
                    RemovedSong = table.Column<string>(type: "TEXT", nullable: false),
                    RemovedSongs = table.Column<string>(type: "TEXT", nullable: false),
                    LoopingSong = table.Column<string>(type: "TEXT", nullable: false),
                    LoopingQueue = table.Column<string>(type: "TEXT", nullable: false),
                    WrongImageCount = table.Column<string>(type: "TEXT", nullable: false),
                    NotLooping = table.Column<string>(type: "TEXT", nullable: false),
                    NoResults = table.Column<string>(type: "TEXT", nullable: false),
                    Left = table.Column<string>(type: "TEXT", nullable: false),
                    InformationAbout = table.Column<string>(type: "TEXT", nullable: false),
                    JoinedSilverCraft = table.Column<string>(type: "TEXT", nullable: false),
                    PrefixUsedTopgg = table.Column<string>(type: "TEXT", nullable: false),
                    ShuffledSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    User = table.Column<string>(type: "TEXT", nullable: false),
                    Userid = table.Column<string>(type: "TEXT", nullable: false),
                    IsAnOwner = table.Column<string>(type: "TEXT", nullable: false),
                    IsABot = table.Column<string>(type: "TEXT", nullable: false),
                    AccountCreationDate = table.Column<string>(type: "TEXT", nullable: false),
                    AccountJoinDate = table.Column<string>(type: "TEXT", nullable: false),
                    SilverhostingJokeTitle = table.Column<string>(type: "TEXT", nullable: false),
                    SilverhostingJokeDescription = table.Column<string>(type: "TEXT", nullable: false),
                    PurgeNumberNegative = table.Column<string>(type: "TEXT", nullable: false),
                    PurgeNothingToDelete = table.Column<string>(type: "TEXT", nullable: false),
                    PurgeRemovedFront = table.Column<string>(type: "TEXT", nullable: false),
                    PurgeRemovedSingle = table.Column<string>(type: "TEXT", nullable: false),
                    PurgeRemovedPlural = table.Column<string>(type: "TEXT", nullable: false),
                    NoEmotesFound = table.Column<string>(type: "TEXT", nullable: false),
                    SearchedFor = table.Column<string>(type: "TEXT", nullable: false),
                    MultipleEmotesFound = table.Column<string>(type: "TEXT", nullable: false),
                    AlreadyOptedIn = table.Column<string>(type: "TEXT", nullable: false),
                    Server = table.Column<string>(type: "TEXT", nullable: false),
                    OptedIn = table.Column<string>(type: "TEXT", nullable: false),
                    OptedInWebshot = table.Column<string>(type: "TEXT", nullable: false),
                    OptedOutWebshot = table.Column<string>(type: "TEXT", nullable: false),
                    AllAvailibleEmotes = table.Column<string>(type: "TEXT", nullable: false),
                    UserIsBannedFromSilversocial = table.Column<string>(type: "TEXT", nullable: false),
                    UselessFact = table.Column<string>(type: "TEXT", nullable: false),
                    UserHasLowerRole = table.Column<string>(type: "TEXT", nullable: false),
                    Ban = table.Column<string>(type: "TEXT", nullable: false),
                    Kick = table.Column<string>(type: "TEXT", nullable: false),
                    BotHasLowerRole = table.Column<string>(type: "TEXT", nullable: false),
                    RandomGif = table.Column<string>(type: "TEXT", nullable: false),
                    PoweredByGiphy = table.Column<string>(type: "TEXT", nullable: false),
                    Meme = table.Column<string>(type: "TEXT", nullable: false),
                    NoImageGeneric = table.Column<string>(type: "TEXT", nullable: false),
                    EmoteWasLargerThan256K = table.Column<string>(type: "TEXT", nullable: false),
                    MoreThanOneImageGeneric = table.Column<string>(type: "TEXT", nullable: false),
                    OutputFileLargerThan8M = table.Column<string>(type: "TEXT", nullable: false),
                    PageGif = table.Column<string>(type: "TEXT", nullable: false),
                    PageGifButtonText = table.Column<string>(type: "TEXT", nullable: false),
                    PageNuget = table.Column<string>(type: "TEXT", nullable: false),
                    PeriodExpired = table.Column<string>(type: "TEXT", nullable: false),
                    UserIsntBot = table.Column<string>(type: "TEXT", nullable: false),
                    NowPlaying = table.Column<string>(type: "TEXT", nullable: false),
                    NowPlayingBy = table.Column<string>(type: "TEXT", nullable: false),
                    Enqueued = table.Column<string>(type: "TEXT", nullable: false),
                    EnqueuedBy = table.Column<string>(type: "TEXT", nullable: false),
                    SkippedNP = table.Column<string>(type: "TEXT", nullable: false),
                    CanForceSkip = table.Column<string>(type: "TEXT", nullable: false),
                    NotPaused = table.Column<string>(type: "TEXT", nullable: false),
                    Voted = table.Column<string>(type: "TEXT", nullable: false),
                    AlreadyVoted = table.Column<string>(type: "TEXT", nullable: false),
                    TimeTillTrackPlays = table.Column<string>(type: "TEXT", nullable: false),
                    TimeWhenTrackPlayed = table.Column<string>(type: "TEXT", nullable: false),
                    TimesTrackLooped = table.Column<string>(type: "TEXT", nullable: false),
                    SearchFail = table.Column<string>(type: "TEXT", nullable: false),
                    SearchFailTitle = table.Column<string>(type: "TEXT", nullable: false),
                    SearchFailDescription = table.Column<string>(type: "TEXT", nullable: false),
                    Success = table.Column<string>(type: "TEXT", nullable: false),
                    UrbanExample = table.Column<string>(type: "TEXT", nullable: false),
                    SongLength = table.Column<string>(type: "TEXT", nullable: false),
                    SongTimePosition = table.Column<string>(type: "TEXT", nullable: false),
                    SongTimeLeft = table.Column<string>(type: "TEXT", nullable: false),
                    SongTimeLeftSongLoopingCurrent = table.Column<string>(type: "TEXT", nullable: false),
                    SongTimeLeftSongLooping = table.Column<string>(type: "TEXT", nullable: false),
                    LoadedSilverBotPlaylistWithTitle = table.Column<string>(type: "TEXT", nullable: false),
                    SongNotExist = table.Column<string>(type: "TEXT", nullable: false),
                    VersionInfoTitle = table.Column<string>(type: "TEXT", nullable: false),
                    PurgedBySilverBotReason = table.Column<string>(type: "TEXT", nullable: false),
                    NotValidLanguage = table.Column<string>(type: "TEXT", nullable: false),
                    CultureInfo = table.Column<string>(type: "TEXT", nullable: false),
                    BotBannedUser = table.Column<string>(type: "TEXT", nullable: false),
                    BotKickedUser = table.Column<string>(type: "TEXT", nullable: false),
                    AddedXAmountOfSongs = table.Column<string>(type: "TEXT", nullable: false),
                    TrackingStarted = table.Column<string>(type: "TEXT", nullable: false),
                    TrackingStopped = table.Column<string>(type: "TEXT", nullable: false),
                    TrackCanNotBeSeeked = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandSelf = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandOther = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandGeneralFail = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandCardSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandFailSelf = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandFailOther = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandLeaderBoardTitle = table.Column<string>(type: "TEXT", nullable: false),
                    XPCommandLeaderBoardPerson = table.Column<string>(type: "TEXT", nullable: false),
                    DisabledRepeatedPhrases = table.Column<string>(type: "TEXT", nullable: false),
                    EnableRepeatedPhrases = table.Column<string>(type: "TEXT", nullable: false),
                    CheckFailed = table.Column<string>(type: "TEXT", nullable: false),
                    ChecksFailed = table.Column<string>(type: "TEXT", nullable: false),
                    InvalidOverload = table.Column<string>(type: "TEXT", nullable: false),
                    GeneralException = table.Column<string>(type: "TEXT", nullable: false),
                    NoMatchingSubcommandsAndGroupNotExecutable = table.Column<string>(type: "TEXT", nullable: false),
                    QuotePreviewQuoteID = table.Column<string>(type: "TEXT", nullable: false),
                    QuoteGetNoBook = table.Column<string>(type: "TEXT", nullable: false),
                    QuoteGetNoQuoteWithId = table.Column<string>(type: "TEXT", nullable: false),
                    QuotePreviewDeleteSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandHelpString = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandNoDescription = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandGroupCanBeExecuted = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandGroupAliases = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandGroupArguments = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandGroupSubcommands = table.Column<string>(type: "TEXT", nullable: false),
                    HelpCommandGroupListingAllCommands = table.Column<string>(type: "TEXT", nullable: false),
                    RequireDJCheckFailed = table.Column<string>(type: "TEXT", nullable: false),
                    RequireGuildCheckFailed = table.Column<string>(type: "TEXT", nullable: false),
                    RequireNsfwCheckFailed = table.Column<string>(type: "TEXT", nullable: false),
                    RequireOwnerCheckFailed = table.Column<string>(type: "TEXT", nullable: false),
                    RequireRolesCheckFailedSG = table.Column<string>(type: "TEXT", nullable: false),
                    RequireRolesCheckFailedPL = table.Column<string>(type: "TEXT", nullable: false),
                    RequireBotPermisionsCheckFailedPL = table.Column<string>(type: "TEXT", nullable: false),
                    RequireBotPermisionsCheckFailedSG = table.Column<string>(type: "TEXT", nullable: false),
                    RequireUserPermisionsCheckFailedPL = table.Column<string>(type: "TEXT", nullable: false),
                    RequireUserPermisionsCheckFailedSG = table.Column<string>(type: "TEXT", nullable: false),
                    RequireBotAndUserPermisionsCheckFailedPL = table.Column<string>(type: "TEXT", nullable: false),
                    RequireBotAndUserPermisionsCheckFailedSG = table.Column<string>(type: "TEXT", nullable: false),
                    UnknownImageFormat = table.Column<string>(type: "TEXT", nullable: false),
                    NetVipsLoadFail = table.Column<string>(type: "TEXT", nullable: false),
                    AttributeDataBaseCheckNoDirectMessages = table.Column<string>(type: "TEXT", nullable: false),
                    AttributeDataBaseCheckWebShot = table.Column<string>(type: "TEXT", nullable: false),
                    JpegSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    SilverSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    ComicSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    ResizeSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    TintSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    MathSteps = table.Column<string>(type: "TEXT", nullable: false),
                    Results = table.Column<string>(type: "TEXT", nullable: false),
                    SomethingsContributors = table.Column<string>(type: "TEXT", nullable: false),
                    NuGetVerified = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Downloads = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    SetToProvidedStrings = table.Column<string>(type: "TEXT", nullable: false),
                    SetToDefaultStrings = table.Column<string>(type: "TEXT", nullable: false),
                    NoPerm = table.Column<string>(type: "TEXT", nullable: false),
                    CategorySetSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    EmojiMessageDownloadStart = table.Column<string>(type: "TEXT", nullable: false),
                    EmojiMessageDownloadEnd = table.Column<string>(type: "TEXT", nullable: false),
                    EmojiEnd = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleNoPermManageRoles = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleIntro = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleTitle = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleResponseYes = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleResponseYes2 = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleResponseYes3 = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleResponseNo = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleResponseNo2 = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleResponseNo3 = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleRolesAdded = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleDone = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleNone = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleMainLoop = table.Column<string>(type: "TEXT", nullable: false),
                    ReactionRoleEmbedColour = table.Column<string>(type: "TEXT", nullable: false),
                    FreeToPlayGameType = table.Column<string>(type: "TEXT", nullable: false),
                    NotAvailableGameType = table.Column<string>(type: "TEXT", nullable: false),
                    CostsMoneyGameTypeBug = table.Column<string>(type: "TEXT", nullable: false),
                    NoGamesWereReturned = table.Column<string>(type: "TEXT", nullable: false),
                    NoGamesWereReturnedDescription = table.Column<string>(type: "TEXT", nullable: false),
                    AmericanMoney = table.Column<string>(type: "TEXT", nullable: false),
                    OS = table.Column<string>(type: "TEXT", nullable: false),
                    DsharpplusVersion = table.Column<string>(type: "TEXT", nullable: false),
                    VersionNumber = table.Column<string>(type: "TEXT", nullable: false),
                    GitRepo = table.Column<string>(type: "TEXT", nullable: false),
                    GitCommitHash = table.Column<string>(type: "TEXT", nullable: false),
                    GitBranch = table.Column<string>(type: "TEXT", nullable: false),
                    IsDirty = table.Column<string>(type: "TEXT", nullable: false),
                    CLR = table.Column<string>(type: "TEXT", nullable: false),
                    ReminderErrorNoContent = table.Column<string>(type: "TEXT", nullable: false),
                    ReminderSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    ListReminderNone = table.Column<string>(type: "TEXT", nullable: false),
                    ListReminderStart = table.Column<string>(type: "TEXT", nullable: false),
                    ListReminderListMore = table.Column<string>(type: "TEXT", nullable: false),
                    ReminderContent = table.Column<string>(type: "TEXT", nullable: false),
                    PollResultsStart = table.Column<string>(type: "TEXT", nullable: false),
                    PollResultsResultYes = table.Column<string>(type: "TEXT", nullable: false),
                    PollResultsResultNo = table.Column<string>(type: "TEXT", nullable: false),
                    PollResultsResultUndecided = table.Column<string>(type: "TEXT", nullable: false),
                    PollErrorQuestionNull = table.Column<string>(type: "TEXT", nullable: false),
                    GiveawayResultsNoReactions = table.Column<string>(type: "TEXT", nullable: false),
                    GiveawayItemNull = table.Column<string>(type: "TEXT", nullable: false),
                    GiveawayResultsWon = table.Column<string>(type: "TEXT", nullable: false),
                    QueueNothing = table.Column<string>(type: "TEXT", nullable: false),
                    CancelReminderErrorNoEvent = table.Column<string>(type: "TEXT", nullable: false),
                    CancelReminderErrorMultiple = table.Column<string>(type: "TEXT", nullable: false),
                    CancelReminderErrorAlreadyHandled = table.Column<string>(type: "TEXT", nullable: false),
                    CancelReminderSuccess = table.Column<string>(type: "TEXT", nullable: false),
                    LavalinkNotSetup = table.Column<string>(type: "TEXT", nullable: false),
                    WebsitePlayerResumed = table.Column<string>(type: "TEXT", nullable: false),
                    WebsitePlayerPaused = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteSkippedViaVote = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteVotedForSkip = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteSkipped = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteShuffled = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteLoopOff = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteLoopSong = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteLoopQueue = table.Column<string>(type: "TEXT", nullable: false),
                    WebsitePlayingNothingTrackName = table.Column<string>(type: "TEXT", nullable: false),
                    WebsitePlayPause = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteVoteSkip = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteForceSkip = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteShuffle = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteNoLoop = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteBLoopSong = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteBLoopQueue = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteBVolumeDown = table.Column<string>(type: "TEXT", nullable: false),
                    WebsiteBVolumeUp = table.Column<string>(type: "TEXT", nullable: false),
                    AddedBingToChannel = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translatorSettings_CustomLanguages", x => new { x.TranslatorSettingsId, x.Id });
                    table.ForeignKey(
                        name: "FK_translatorSettings_CustomLanguages_translatorSettings_TranslatorSettingsId",
                        column: x => x.TranslatorSettingsId,
                        principalTable: "translatorSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BingRankingData",
                columns: table => new
                {
                    BingRankingDataId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ServerId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    UserId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    BingCount = table.Column<ulong>(type: "INTEGER", nullable: false),
                    ServerSettingsId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserSettingsId = table.Column<ulong>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BingRankingData", x => x.BingRankingDataId);
                    table.ForeignKey(
                        name: "FK_BingRankingData_serverSettings_ServerSettingsId",
                        column: x => x.ServerSettingsId,
                        principalTable: "serverSettings",
                        principalColumn: "ServerSettingsId");
                    table.ForeignKey(
                        name: "FK_BingRankingData_userSettings_UserSettingsId",
                        column: x => x.UserSettingsId,
                        principalTable: "userSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BingRankingData_ServerSettingsId",
                table: "BingRankingData",
                column: "ServerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_BingRankingData_UserSettingsId",
                table: "BingRankingData",
                column: "UserSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_DatabaseWebhookData_ServerSettingsId",
                table: "DatabaseWebhookData",
                column: "ServerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReactionRoleMappings_ServerSettingsId",
                table: "ReactionRoleMappings",
                column: "ServerSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BingRankingData");

            migrationBuilder.DropTable(
                name: "DatabaseWebhookData");

            migrationBuilder.DropTable(
                name: "plannedEvents");

            migrationBuilder.DropTable(
                name: "ReactionRoleMappings");

            migrationBuilder.DropTable(
                name: "translatorSettings_CustomLanguages");

            migrationBuilder.DropTable(
                name: "userExperiences");

            migrationBuilder.DropTable(
                name: "userQuotes");

            migrationBuilder.DropTable(
                name: "userSettings");

            migrationBuilder.DropTable(
                name: "serverSettings");

            migrationBuilder.DropTable(
                name: "translatorSettings");
        }
    }
}
