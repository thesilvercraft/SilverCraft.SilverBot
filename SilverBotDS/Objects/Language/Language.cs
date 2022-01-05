using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using SilverBotDS.Attributes;

namespace SilverBotDS.Objects;

[Owned]
public class Language
{
    private static readonly Dictionary<string, Language> CachedLanguages = new();

    private static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    [ForeignKey("TranslatorSettings.Id")]
    [JsonIgnore]
    public Guid Id { get; set; }

    /// <summary>
    ///     Default(EN)
    /// </summary>
    public string LangName { get; set; } = "English(EN)";

    /// <summary>
    ///     A 2-4 alphanumeric code
    /// </summary>
    public string LangCodeGoogleTranslate { get; set; } = "en";

    /// <summary>
    ///     <b>Hi </b>@max<b>!</b>
    ///     <para />
    ///     <b>Hi␣</b>{0}<b>!</b>
    /// </summary>
    /// <remarks>
    ///     Needs to be used like <c>String.Format(Language.Hi,Username);</c>
    /// </remarks>
    public string Hi { get; set; } = "Hi {0}!";

    /// <summary>
    ///     <b>Time in UTC is </b>01.1.2021 17:20:28
    ///     <para />
    ///     <b>Time␣in␣UTC is␣</b>{0}
    /// </summary>
    /// <remarks>
    ///     Needs to be used like <c>String.Format(Language.Time_In_Utc,DateTime.NowUTC.ToString(Language.Time_format));</c>
    /// </remarks>
    public string TimeInUtc { get; set; } = "Time in UTC is {0}";

    /// <summary>
    ///     This command is disabled
    /// </summary>
    public string CommandIsDisabled { get; set; } = "This command is disabled";

    /// <summary>
    ///     <b>Requested by </b>SilverDimond
    ///     <para />
    ///     <b>Requested␣by␣</b>
    /// </summary>
    /// <remarks>
    ///     Needs to be used like <c>Language.Requested_by+Username</c> <strong>MAY BE SUBJECT TO CHANGE</strong>
    /// </remarks>
    public string RequestedBy { get; set; } = "Requested by ";

    /// <summary>
    ///     Something went wrong, it's probably on my end.
    /// </summary>
    public string DblaReturnedNull { get; set; } = "Something went wrong, it's probably on my end.";

    /// <summary>
    ///     must be EN or DE for useless facts
    /// </summary>
    public string LangCodeForUselessFacts { get; set; } = "en";

    //Music related
    /// <summary>
    ///     I'm already in a voice channel!
    /// </summary>
    public string AlreadyConnected { get; set; } = "I'm already in a voice channel!";

    /// <summary>
    ///     You must be in a voice channel!
    /// </summary>
    public string UserNotConnected { get; set; } = "You must be in a voice channel!";

    /// <summary>
    ///     Please use a value between 0 and 100!
    /// </summary>
    public string VolumeNotCorrect { get; set; } = "Please use a value between 1 and 100%!";

    /// <summary>
    ///     <b>Joined </b>Testing<b>!</b>
    ///     <para />
    ///     <b>Joined␣</b>{0}<b>!</b>
    /// </summary>
    /// <remarks>
    ///     Needs to be used like <c>String.Format(Language.Joined,Channel));</c>
    /// </remarks>
    public string Joined { get; set; } = "Joined {0}!";

    /// <summary>
    ///     I'm not connected to a voice channel.
    /// </summary>
    public string NotConnected { get; set; } = "I'm not connected to a voice channel.";

    /// <summary>
    ///     Whoa there, I'm not playing any tracks.
    /// </summary>
    public string NotPlaying { get; set; } = "Whoa there, I'm not playing any tracks.";

    /// <summary>
    ///     Whoa there, there is nothing queued next.
    /// </summary>
    public string NothingInQueue { get; set; } = "Whoa there, there is nothing queued next.";

    public string NothingInQueueHistory { get; set; } = "Whoa there is nothing in the queue history";

    /// <summary>
    ///     Never gonna give you up<b> by </b>Rick Astley
    ///     <para />
    ///     <b>␣by␣</b>
    /// </summary>
    /// <remarks>
    ///     Needs to be used like <c>Title+Language.Song_by_author+Author</c>
    /// </remarks>
    public string SongByAuthor { get; set; } = " by ";

    public string RemovedFront { get; set; } = "Removed: "; //"Removed: "Never gonna give you up by Rick Astley

    /// <summary>
    ///     Removed 20 songs
    ///     <br />
    ///     see <seealso cref="RemovedSong" /> and <seealso cref="RemovedSongs" />
    /// </summary>
    public string RemovedXSongOrSongs { get; set; } = "Removed {0} {1}";

    public string RemovedSong { get; set; } = "song";
    public string RemovedSongs { get; set; } = "songs";

    /// <summary>
    ///     Now looping song 🔂
    /// </summary>
    public string LoopingSong { get; set; } = "Now looping song 🔂";

    /// <summary>
    ///     Now looping queue 🔁
    /// </summary>
    public string LoopingQueue { get; set; } = "Now looping queue 🔁";

    /// <summary>
    ///     Send **an** image my guy
    /// </summary>
    public string WrongImageCount { get; set; } = "Send **an** image my guy";

    /// <summary>
    ///     Now not looping.
    /// </summary>
    public string NotLooping { get; set; } = "Now not looping.";

    /// <summary>
    ///     I wasn't able to find anything for `Never gonna give you up`.
    /// </summary>
    /// <remarks>
    ///     I wasn't able to find anything for `{0}`.
    /// </remarks>
    public string NoResults { get; set; } =
        "I wasn't able to find anything for `{0}`"; //I wasn't able to find anything for `"Never gonna give you up`.

    /// <summary>
    ///     I've left {0}!
    /// </summary>
    public string Left { get; set; } = "I've left {0}!";

    /// <summary>
    ///     Information about␣
    /// </summary>
    public string InformationAbout { get; set; } = "Information about ";

    /// <summary>
    ///     Has joined the SilverCraft Discord
    /// </summary>
    public string JoinedSilverCraft { get; set; } = "Has joined the SilverCraft Discord";

    /// <summary>
    ///     Prefix used
    /// </summary>
    public string PrefixUsedTopgg { get; set; } = "Prefix used";

    /// <summary>
    ///     The playlist has been shuffled
    /// </summary>
    public string ShuffledSuccess { get; set; } = "The playlist has been shuffled";

    /// <summary>
    ///     User:␣
    /// </summary>
    public string User { get; set; } = "User: ";

    /// <summary>
    ///     ID
    /// </summary>
    public string Userid { get; set; } = "ID";

    /// <summary>
    ///     Is a SilverCraft bot owner
    /// </summary>
    public string IsAnOwner { get; set; } = "Is a SilverCraft bot owner";

    /// <summary>
    ///     Is a bot
    /// </summary>
    public string IsABot { get; set; } = "Is a bot";

    public string AccountCreationDate { get; set; } = "Joined discord on";
    public string AccountJoinDate { get; set; } = "Joined this server on";

    /// <summary>
    ///     SilverBot sponsored by SilverHosting
    /// </summary>
    public string SilverhostingJokeTitle { get; set; } = "SilverBot sponsored by SilverHosting";

    /// <summary>
    ///     Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
    /// </summary>
    public string SilverhostingJokeDescription { get; set; } =
        "Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)";

    public string PurgeNumberNegative { get; set; } = "The amount of messages to remove must be positive.";
    public string PurgeNothingToDelete { get; set; } = "Nothing to delete.";

    public string PurgeRemovedFront { get; set; } = "Done. Removed "; //"Done. Removed "69 messages
    public string PurgeRemovedSingle { get; set; } = " message"; //Done. Removed 1" message"
    public string PurgeRemovedPlural { get; set; } = " messages"; //Done. Removed 69" messages"

    /// <summary>
    ///     **No emote found**
    /// </summary>
    public string NoEmotesFound { get; set; } = "**No emote found**";

    /// <summary>
    ///     Searched for: `{0}`
    /// </summary>
    public string SearchedFor { get; set; } = "Searched for: `{0}`";

    /// <summary>
    ///     **Multiple emotes found**
    /// </summary>
    public string MultipleEmotesFound { get; set; } = "**Multiple emotes found**";

    /// <summary>
    ///     Already opted into SilverBot emotes
    /// </summary>
    public string AlreadyOptedIn { get; set; } = "Already opted into SilverBot emotes";

    /// <summary>
    ///     Server :{0}
    /// </summary>
    public string Server { get; set; } = "Server: {0}";

    /// <summary>
    ///     Opted into SilverBot emotes
    /// </summary>
    public string OptedIn { get; set; } = "Opted into SilverBot emotes";

    /// <summary>
    ///     Opted into SilverBot WebShot
    /// </summary>
    public string OptedInWebshot { get; set; } = "Opted into SilverBot WebShot";

    /// <summary>
    ///     Opted out of SilverBot WebShot
    /// </summary>
    public string OptedOutWebshot { get; set; } = "Opted out of SilverBot WebShot";

    /// <summary>
    ///     **All available emotes**
    /// </summary>
    public string AllAvailibleEmotes { get; set; } = "**All available emotes**";

    /// <summary>
    ///     You are banned from using the SilverSocial features
    /// </summary>
    public string UserIsBannedFromSilversocial { get; set; } = "You are banned from using the SilverSocial features";

    public string UselessFact { get; set; } = "Useless fact";

    /// <summary>
    ///     You must have a higher role than the person you are trying to␣
    /// </summary>
    public string UserHasLowerRole { get; set; } = "You must have a higher role than the person you are trying to ";

    public string Ban { get; set; } = "ban";
    public string Kick { get; set; } = "kick";
    public string BotHasLowerRole { get; set; } = "I must have a higher role than the person you are trying to ";
    public string RandomGif { get; set; } = "Random GIF:";

    /// <summary>
    ///     Powered by GIPHY
    /// </summary>
    public string PoweredByGiphy { get; set; } = "Powered by GIPHY";

    /// <summary>
    ///     Meme:␣
    /// </summary>
    public string Meme { get; set; } = "Meme: ";

    /// <summary>
    ///     You didn't attach a image
    /// </summary>
    /// <remarks>
    ///     Ment to be used when there isn't a more better term
    /// </remarks>
    public string NoImageGeneric { get; set; } = "You didn't attach a image";

    /// <summary>
    ///     Bruv discord no likey you need something less than 256kb you sent {0}
    /// </summary>
    public string EmoteWasLargerThan256K { get; set; } =
        "Bruv discord no likey you need something less than 256kb you sent {0}";

    /// <summary>
    ///     You attached more than one image
    /// </summary>
    /// <remarks>
    ///     Ment to be used when there isn't a more better term
    /// </remarks>
    public string MoreThanOneImageGeneric { get; set; } = "You attached more than one image";

    /// <summary>
    ///     The output file is larger than 8mb, its {0}
    /// </summary>
    /// <remarks>
    ///     Ment to be used when there isn't a more better term
    /// </remarks>
    public string OutputFileLargerThan8M { get; set; } = "The output file is larger than 8mb, its {0}";

    /// <summary>
    ///     Page {0}/{1} Use `next` in the next 5 min to see the next page
    /// </summary>
    public string PageGif { get; set; } =
        "Page {0}/{1} Click on the `Next` button in the next 5 min to see the next page";

    public string PageGifButtonText { get; set; } = "Next";

    /// <summary>
    ///     Page {0}/{1}
    /// </summary>
    public string PageNuget { get; set; } = "Page {0}/{1}";

    /// <summary>
    ///     You may not send `next` as the 5 minutes expired
    /// </summary>
    public string PeriodExpired { get; set; } = "You may not send `next` as the 5 minutes expired";

    /// <summary>
    ///     Hey that isn't a bot, i think.
    /// </summary>
    public string UserIsntBot { get; set; } = "Hey that isn't a bot, i think.";

    /// <summary>
    ///     Now playing: {0}
    /// </summary>
    public string NowPlaying { get; set; } = "Now playing: {0}";

    /// <summary>
    ///     Enqueued: {0}
    /// </summary>
    public string Enqueued { get; set; } = "Enqueued: {0}";

    /// <summary>
    ///     Skipped: {0}, Now playing {1}
    /// </summary>
    public string SkippedNP { get; set; } = "Skipped: {0}, Now playing {1}";

    /// <summary>
    ///     You can use `sd!forceskip` to instantly skip the song
    /// </summary>
    public string CanForceSkip { get; set; } = "You can use `sd!forceskip` to instantly skip the song";

    /// <summary>
    ///     It's not paused right now.
    /// </summary>
    public string NotPaused { get; set; } = "It's not paused right now.";

    /// <summary>
    ///     Your vote was counted
    /// </summary>
    public string Voted { get; set; } = "Your vote was counted";

    /// <summary>
    ///     Your vote was counted already
    /// </summary>
    public string AlreadyVoted { get; set; } = "Your vote was counted already";

    /// <summary>
    ///     Estimated time till it plays:
    /// </summary>
    public string TimeTillTrackPlays { get; set; } = "Estimated time till it plays:";

    public string TimeWhenTrackPlayed { get; set; } = "Time when the track started:";

    /// <summary>
    ///     Uh oh something went wrong. Please try again a little bit later.
    /// </summary>
    public string SearchFail { get; set; } = "Uh oh something went wrong. Please try again a little bit later.";

    /// <summary>
    ///     Uh oh something went wrong.
    /// </summary>
    public string SearchFailTitle { get; set; } = "Uh oh something went wrong.";

    /// <summary>
    ///     Please try again a little bit later.
    /// </summary>
    public string SearchFailDescription { get; set; } = "Please try again a little bit later.";

    public string Success { get; set; } = "GREAT SUCCESS! HIGH FIVE EPIC GAMERS";
    public string UrbanExample { get; set; } = "Example";
    public string SongLength { get; set; } = "Length";
    public string SongTimePosition { get; set; } = "Position";
    public string SongTimeLeft { get; set; } = "Time left:";

    /// <summary>
    ///     Never (song is looping)
    /// </summary>
    public string SongTimeLeftSongLoopingCurrent { get; set; } = "N/A (current song is looping)";

    /// <summary>
    ///     Never (current song is looping)
    /// </summary>
    public string SongTimeLeftSongLooping { get; set; } = "N/A (song is looping)";

    public string LoadedSilverBotPlaylistWithTitle { get; set; } = "Loaded SilverBot playlist/queue file, called {0}";
    public string SongNotExist { get; set; } = "That song does not exist my dude";
    public string VersionInfoTitle { get; set; } = "SilverBot Version info";
    public string PurgedBySilverBotReason { get; set; } = "Purged by SilverBot for {0}";
    public string NotValidLanguage { get; set; } = "That isn't a valid language, valid languages are: {0}";
    public string CultureInfo { get; set; } = "en-GB";

    public string BotBannedUser { get; set; } = "The bot has attempted to ban the user";
    public string BotKickedUser { get; set; } = "The bot has attempted to kick the user";
    public string AddedXAmountOfSongs { get; set; } = "Added {0} songs to the queue";
    public string TrackingStarted { get; set; } = "This isn't implemented.";
    public string TrackingStopped { get; set; } = "Enabled 24/7 mode";
    public string TrackCanNotBeSeeked { get; set; } = "This track can not be seeked";
    public string XPCommandSelf { get; set; } = "You have {0} XP, level {1}";
    public string XPCommandOther { get; set; } = "They have {0} XP, level {1}";
    public string XPCommandGeneralFail { get; set; } = "Ooops the database broke, please dm an SilverBot admin";

    public string XPCommandFailSelf { get; set; } =
        "It appears that something either went wrong or you do not have any xp.";

    public string XPCommandFailOther { get; set; } =
        "It appears that something either went wrong or they do not have any xp.";

    public string XPCommandLeaderBoardTitle { get; set; } = "XP leaderboard:";
    public string XPCommandLeaderBoardPerson { get; set; } = "{0} has {1}XP";

    public string DisabledRepeatedPhrases { get; set; } =
        "Disabled the repeating of simple phrases (like `e` or `fock`).";

    public string EnableRepeatedPhrases { get; set; } = "Enabled the repeating of simple phrases (like `e` or `fock`).";

    //bot error stuff here
    /// <summary>
    ///     Check {0} failed
    /// </summary>
    public string CheckFailed { get; set; } = "Check {0} failed";

    /// <summary>
    ///     Checks failed
    /// </summary>
    public string ChecksFailed { get; set; } = "Checks failed";

    /// <summary>
    ///     Invalid overload for `{0}`
    /// </summary>
    public string InvalidOverload { get; set; } = "Invalid overload for `{0}`";

    /// <summary>
    ///     ⚠ An error occurred, more information has been sent to the bot owner's log.
    /// </summary>
    public string GeneralException { get; set; } =
        "⚠ An error occurred, more information has been sent to the bot owner's log.";

    /// <summary>
    ///     No matching subcommands were found, and this group is not executable.
    /// </summary>
    public string NoMatchingSubcommandsAndGroupNotExecutable { get; set; } =
        "No matching subcommands were found, and this group is not executable.";

    public string QuotePreviewQuoteID { get; set; } = "Quote ID: {0}";
    public string QuoteGetNoBook { get; set; } = "You do not have a \"Quote Book\", you can get one by adding a quote.";
    public string QuoteGetNoQuoteWithId { get; set; } = "You do not have a quote with that ID.";
    public string QuotePreviewDeleteSuccess { get; set; } = "The quote has been deleted.";
    public string HelpCommandHelpString { get; set; } = "Help";
    public string HelpCommandNoDescription { get; set; } = "No description provided.";

    public string HelpCommandGroupCanBeExecuted { get; set; } =
        $"This group {Formatter.Bold("can")} be executed as a standalone command.";

    public string HelpCommandGroupAliases { get; set; } = "Aliases";
    public string HelpCommandGroupArguments { get; set; } = "Arguments";
    public string HelpCommandGroupSubcommands { get; set; } = "Subcommands";

    public string HelpCommandGroupListingAllCommands { get; set; } =
        "Listing all commands and groups. Specify a command to see more information.";

    public string RequireDJCheckFailed { get; set; } = "You must have the DJ role to use that";
    public string RequireGuildCheckFailed { get; set; } = "You must be in a Guild (aka discord server) to use that.";
    public string RequireNsfwCheckFailed { get; set; } = "You must be in a channel that is marked NSFW to use that.";
    public string RequireOwnerCheckFailed { get; set; } = "You must be a SilverBot owner to use that.";
    public string RequireRolesCheckFailedSG { get; set; } = "You must have the {0} role to use that.";
    public string RequireRolesCheckFailedPL { get; set; } = "You must have the roles named {0} to use that.";

    public string RequireBotPermisionsCheckFailedPL { get; set; } =
        "I must have the permisions {0} for you to use that.";

    public string RequireBotPermisionsCheckFailedSG { get; set; } =
        "I must have the permision {0} for you to use that.";

    public string RequireUserPermisionsCheckFailedPL { get; set; } = "You must have the permisions {0} to use that.";
    public string RequireUserPermisionsCheckFailedSG { get; set; } = "You must have the permision {0} to use that.";

    public string RequireBotAndUserPermisionsCheckFailedPL { get; set; } =
        "We both must have the permisions {0} for you to use that.";

    public string RequireBotAndUserPermisionsCheckFailedSG { get; set; } =
        "We both must have the permision {0} for you to use that.";

    public string UnknownImageFormat { get; set; } =
        "I do not know how to read that format, I can only read BMP, TIFF, GIF, PNG, TGA and JPEG photos.";

    public string JpegSuccess { get; set; } = "There ya go a jpegnized image";
    public string SilverSuccess { get; set; } = "There ya go a silver image";
    public string ComicSuccess { get; set; } = "There ya go a image with the comic filter";
    public string ResizeSuccess { get; set; } = "There ya go a resized image";
    public string TintSuccess { get; set; } = "There ya go a tinted image";
    public string MathSteps { get; set; } = "Math steps: ";
    public string Results { get; set; } = "Result: ";
    public string SomethingsContributors { get; set; } = "{0}'s contributors";
    public string NuGetVerified { get; set; } = "NuGet verified";
    public string Type { get; set; } = "Type";
    public string Downloads { get; set; } = "<:green_download_icon:805051604797227038>";
    public string Version { get; set; } = "Version";
    public string SetToProvidedStrings { get; set; } = "Set them to the provided things";
    public string SetToDefaultStrings { get; set; } = "Set them to the default things";
    public string NoPerm { get; set; } = "Please give me permission to manage channels in the category";

    public string CategorySetSuccess { get; set; } =
        "Set the category to {0}, you might want to add atleast 4 channels of any kind in it and then run the `setstatstrings` command";

    public string EmojiMessageDownloadStart { get; set; } =
        "Downloading at most {0} messages which will take around {1} cause of discord ratelimit";

    public string EmojiMessageDownloadEnd { get; set; } =
        "Downloaded {0} while expecting {1}, estimated time was: {2} actual time was {3} expected time if provided correct limit would be {4}\nWell anyways processing messages";

    public string EmojiEnd { get; set; } =
        "i went through {0} messages in {1}(including download) {2}(excluding download)\nEmote usage data:";

    public string FreeToPlayGameType { get; set; } = "F2P";
    public string NotAvailableGameType { get; set; } = "Not Available";
    public string CostsMoneyGameTypeBug { get; set; } = "It costs merica bucks but idk how much";
    public string NoGamesWereReturned { get; set; } = "No Games were returned";
    public string NoGamesWereReturnedDescription { get; set; } = "Try again later, or try changing your search term";
    public string AmericanMoney { get; set; } = "{0} merica bucks";
    public string OS { get; set; } = "Operating system:";
    public string DsharpplusVersion { get; set; } = "Dsharpplus version:";
    public string VersionNumber { get; set; } = "Version number";
    public string GitRepo { get; set; } = "Git repo";
    public string GitCommitHash { get; set; } = "Git Commit hash";
    public string GitBranch { get; set; } = "Git Branch";
    public string IsDirty { get; set; } = "Is dirty";
    public string CLR { get; set; } = "CLR";

    public CultureInfo GetCultureInfo()
    {
        return new CultureInfo(CultureInfo);
    }

    public static Dictionary<string, Language> GetLoadedLanguages()
    {
        if (CachedLanguages.Count == 0)
        {
            var t = GetAsync("en");
            t.Wait();
        }

        return CachedLanguages;
    }

    public static string[] LoadedLanguages()
    {
        if (CachedLanguages.Count == 0)
        {
            var t = GetAsync("en");
            t.Wait();
        }

        return CachedLanguages.Keys.ToArray();
    }

    public static async Task<Language> GetAsync(string a)
    {
        a = a.Trim();
        if (CachedLanguages.Count != 0)
        {
            if (CachedLanguages.ContainsKey(a))
            {
                return CachedLanguages[a];
            }

            return new Language();
        }

        var folloc = $"{Environment.CurrentDirectory}{Program.DirSlash}Languages{Program.DirSlash}";
        if (Directory.Exists(folloc))
        {
            foreach (var folder in Directory.GetDirectories(folloc))
            {
                foreach (var u in Directory.GetFiles(folder))
                {
                    await using Stream stream = File.OpenRead(u);
                    var reader = new StreamReader(stream);
                    var content = await reader.ReadToEndAsync();
                    reader.Close();
                    reader.Dispose();
                    var asdf = JsonSerializer.Deserialize<Language>(content);
                    var name = Path.GetFileNameWithoutExtension(u);
                    CachedLanguages.Add(name, asdf);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(folloc + $"{Program.DirSlash}en");
            await SerialiseDefaultAsync(
                $"{Environment.CurrentDirectory}{Program.DirSlash}languages{Program.DirSlash}en{Program.DirSlash}en.json");
        }

        return await GetAsync(a);
    }

    public static async Task SerialiseDefaultAsync(string loc)
    {
        using var streamWriter = new StreamWriter(loc);
        await streamWriter.WriteAsync(JsonSerializer.Serialize(new Language(), options));
    }

    public static void SerialiseDefault(string loc)
    {
        using var streamWriter = new StreamWriter(loc);
        streamWriter.Write(JsonSerializer.Serialize(new Language(), options));
    }

    public static async Task<Language> GetLanguageFromGuildIdAsync(ulong id, DatabaseContext db)
    {
        return await GetAsync(db.GetLangCodeGuild(id));
    }

    public static async Task<Language> GetLanguageFromCtxAsync(CommandContext ctx)
    {
        using var db = (DatabaseContext) ctx.Services.GetService(typeof(DatabaseContext));
        var conf = (Config) ctx.CommandsNext.Services.GetService(typeof(Config));
        if (await RequireTranslatorAttribute.IsTranslator(conf, ctx.Client, ctx.User.Id, ctx.Channel.Id))
        {
            var t = db.translatorSettings.FirstOrDefault(x => x.Id == ctx.User.Id);

            if (t?.IsTranslator == true && t.CurrentCustomLanguage != null)
            {
                await db.Entry(t).ReloadAsync();
                return t.CurrentCustomLanguage;
            }
        }

        return await GetAsync(ctx.Channel.IsPrivate
            ? db.GetLangCodeUser(ctx.User.Id)
            : db.GetLangCodeGuild(ctx.Guild.Id));
    }

    public static async Task<Language> GetLanguageFromCtxAsync(BaseContext ctx)
    {
        using var db = (DatabaseContext) ctx.Services.GetService(typeof(DatabaseContext));
        var conf = (Config) ctx.Services.GetService(typeof(Config));
        if (await RequireTranslatorAttribute.IsTranslator(conf, ctx.Client, ctx.User.Id, ctx.Channel.Id))
        {
            var t = db.translatorSettings.FirstOrDefault(x => x.Id == ctx.User.Id);
            if (t?.IsTranslator == true && t.CurrentCustomLanguage != null)
            {
                await db.Entry(t).ReloadAsync();
                return t.CurrentCustomLanguage;
            }
        }

        return await GetAsync(ctx.Channel.IsPrivate
            ? db.GetLangCodeUser(ctx.User.Id)
            : db.GetLangCodeGuild(ctx.Guild.Id));
    }
}