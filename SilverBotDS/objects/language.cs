using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using DSharpPlus.CommandsNext;

namespace SilverBotDS.Objects
{
    //TODO finish the summaries and shiz
    //I know i promised language support but im kinda of an idiot so yeah
    internal class Language
    {
        /// <summary>
        /// <b>Hi </b>@max<b>!</b>
        /// <para/>
        /// <b>Hi␣</b>{0}<b>!</b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>String.Format(Language.Hi,Username);</c>
        /// </remarks>
        public string Hi { get; set; } = "Hi {0}!";

        /// <summary>
        /// <b>Time in UTC is </b>01.1.2021 17:20:28
        /// <para/>
        /// <b>Time␣in␣UTC is␣</b>{0}
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>String.Format(Language.Time_In_Utc,DateTime.NowUTC.ToString(Language.Time_format));</c>
        /// </remarks>
        public string TimeInUtc { get; set; } = "Time in UTC is {0}";

        /// <summary>
        /// This command is disabled
        /// </summary>
        public string CommandIsDisabled { get; set; } = "This command is disabled";

        /// <summary>
        /// <b>Requested by </b>SilverDimond
        /// <para/>
        /// <b>Requested␣by␣</b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>Language.Requested_by+Username</c> <strong>MAY BE SUBJECT TO CHANGE</strong>
        /// </remarks>

        public string RequestedBy { get; set; } = "Requested by ";
        /// <summary>
        /// Something went wrong, it's probably on my end.
        /// </summary>

        public string DblaReturnedNull { get; set; } = "Something went wrong, it's probably on my end.";

        /// <summary>
        /// Default(EN)
        /// </summary>
        public string LangName { get; set; } = "Default(EN)";

        /// <summary>
        /// must be en or de for useless facts
        /// </summary>
        public string LangCodeForUselessFacts { get; set; } = "en";

        //Music related
        /// <summary>
        ///I'm already in a voice channel!
        /// </summary>
        public string AlreadyConnected { get; set; } = "I'm already in a voice channel!";

        /// <summary>
        ///You must be in a voice channel!
        /// </summary>
        public string UserNotConnected { get; set; } = "You must be in a voice channel!";

        /// <summary>
        /// <b>Joined </b>Testing<b>!</b>
        /// <para/>
        /// <b>Joined␣</b>{0}<b>!</b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>String.Format(Language.Joined,Channel));</c>
        /// </remarks>
        public string Joined { get; set; } = "Joined {0}!";

        /// <summary>
        /// I'm not connected to a voice channel.
        /// </summary>
        public string NotConnected { get; set; } = "I'm not connected to a voice channel.";

        /// <summary>
        /// Woaaah there, I'm not playing any tracks.
        /// </summary>
        public string NotPlaying { get; set; } = "Woah there, I'm not playing any tracks.";

        /// <summary>
        /// Woah there, there is nothing queued next.
        /// </summary>
        public string NothingInQueue { get; set; } = "Woah there, there is nothing queued next.";

        /// <summary>
        /// Never gonna give you up<b> by </b>Rick Astley
        /// <para/>
        /// <b>␣by␣</b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>Title+Language.Song_by_author+Author</c>
        /// </remarks>
        public string SongByAuthor { get; set; } = " by ";

        /// <summary>
        /// Tip: A Role Named ***DJ*** Will Restrict Access To Members With The Role.
        /// </summary>
        public string UseDjMessage { get; set; } = "Tip: A Role Named ***DJ*** Will Restrict Access To Members With The Role.";

        /// <summary>
        /// That track doesnt exist my dude
        /// </summary>
        public string TrackNotExist { get; set; } = "That track doesnt exist my dude";

        public string RemovedFront { get; set; } = "Removed: "; //"Removed: "Never gonna give you up by Rick Astley
        public string PausedFront { get; set; } = "Paused: "; //"Paused: "Never gonna give you up by Rick Astley
        public string ResumedFront { get; set; } = "Resumed: "; //"Resumed: "Never gonna give you up by Rick Astley

        /// <summary>
        /// Please provide a search term.
        /// </summary>
        public string NoSearchTerm { get; set; } = "Please provide a search term.";

        /// <summary>
        /// Not sure which voice channel to disconnect from.
        /// </summary>
        public string NotSureWhichChannel { get; set; } = "Not sure which voice channel to disconnect from.";

        public string SeekedFront { get; set; } = "I've seeked: `"; //"I've seeked: `"Never gonna give you up by Rick Astley to 00:00:00.
        public string SeekedMiddle { get; set; } = "` to "; //I've seeked: `Never gonna give you up"` to "00:00:00.
        public string SeekedBack { get; set; } = "."; //I've seeked: `Never gonna give you up` to 00:00:00"."
        public string NoLyricsFound { get; set; } = "No lyrics found for "; //"No lyrics found for Never gonna give you up
        public string NoLongerPlaying { get; set; } = "No longer playing anything.";
        public string AlreadyStoped { get; set; } = "Woaaah there, I can't stop the stopped forced.";
        public string VotingIsBelow85Percent { get; set; } = "You need more than 85% votes to skip this song.";
        public string UserDoesntHaveDj { get; set; } = "You must have a role named `dj`!";
        public string LoopingSong { get; set; } = "Now loopin song 🔂";
        public string LoopingQueue { get; set; } = "Now loopin queue 🔁";
        public string NotLooping { get; set; } = "Now not loopin.";

        /// <summary>
        /// I wasn't able to find anything for `Never gonna give you up`.
        /// </summary>
        /// <remarks>
        ///  I wasn't able to find anything for `{0}`.
        /// </remarks>
        public string NoResults { get; set; } = "I wasn't able to find anything for `{0}`"; //I wasn't able to find anything for `"Never gonna give you up`.

        public string NooneInVc { get; set; } = "Noone is in voice chat. 😥";
        public string NextItemNotTrack { get; set; } = "Next item in queue is not a track. 😥";
        public string QueueCompleted { get; set; } = "Queue completed! Please add more tracks to continue!";

        /// <summary>
        /// I've left {0}!
        /// </summary>
        public string Left { get; set; } = "I've left {0}!";

        public string Goodbye { get; set; } = "Goodbye";

        //Commands
        public string Test { get; set; } = "test";

        /// <summary>
        /// Information about␣
        /// </summary>
        public string InformationAbout { get; set; } = "Information about ";

        /// <summary>
        /// Has joined the SilverCraft Discord
        /// </summary>
        public string JoinedSilverCraft { get; set; } = "Has joined the SilverCraft Discord";

        /// <summary>
        /// Is a SilverCraft bot admin
        /// </summary>
        public string IsSilverBotAdmin { get; set; } = "Is a SilverCraft bot admin";

        /// <summary>
        /// Prefix used
        /// </summary>
        public string PrefixUsedTopgg { get; set; } = "Prefix used";

        /// <summary>
        /// User:␣
        /// </summary>
        public string User { get; set; } = "User: ";

        /// <summary>
        /// ID
        /// </summary>
        public string Userid { get; set; } = "ID";

        /// <summary>
        /// Has joined the SilverCraft Server
        /// </summary>
        public string HasJoinedSupport { get; set; } = "Has joined the SilverCraft Server";

        /// <summary>
        /// Is a SilverCraft bot admin
        /// </summary>
        public string IsAdmin { get; set; } = "Is a SilverCraft bot admin";

        /// <summary>
        /// Is a DJ
        /// </summary>
        public string IsADj { get; set; } = "Is a DJ";

        /// <summary>
        /// Is a SilverCraft bot owner
        /// </summary>
        public string IsAnOwner { get; set; } = "Is a SilverCraft bot owner";

        /// <summary>
        /// Is a bot
        /// </summary>
        public string IsABot { get; set; } = "Is a bot";

        public string Error { get; set; } = "Error";
        public string ErrorSentToLogs { get; set; } = "Complete error sent to Bot logs";

        /// <summary>
        ///SilverBot sponsored by SilverHosting
        /// </summary>
        public string SilverhostingJokeTitle { get; set; } = "SilverBot sponsored by SilverHosting";

        /// <summary>
        ///Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
        /// </summary>
        public string SilverhostingJokeDescription { get; set; } = "Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)";

        public string PurgeNumberNegative { get; set; } = "The amount of messages to remove must be positive.";
        public string PurgeNothingToDelete { get; set; } = "Nothing to delete.";
        public string BotGuilds { get; set; } = "Bot guilds";
        public string PurgeRemovedFront { get; set; } = "Done. Removed "; //"Done. Removed "69 messages
        public string PurgeRemovedSingle { get; set; } = " message"; //Done. Removed 1" message"
        public string PurgeRemovedPlural { get; set; } = " messages"; //Done. Removed 69" messages"
        public string InviteTitle { get; set; } = "SilverCraftBot invite link";
        public string InviteDescription { get; set; } = "SilverCraftBot is in open beta https://discord.com/api/oauth2/authorize?client_id=702445582559739976&permissions=2147483639&scope=bot";

        /// <summary>
        /// **No emote found**
        /// </summary>
        public string NoEmotesFound { get; set; } = "**No emote found**";

        /// <summary>
        /// Searched for: `{0}`
        /// </summary>
        public string SearchedFor { get; set; } = "Searched for: `{0}`";

        /// <summary>
        /// **Multiple emotes found**
        /// </summary>
        public string MultipleEmotesFound { get; set; } = "**Multiple emotes found**";

        /// <summary>
        /// Already opted into SilverBot emotes
        /// </summary>
        public string AlreadyOptedIn { get; set; } = "Already opted into SilverBot emotes";

        /// <summary>
        /// Server :{0}
        /// </summary>
        public string Server { get; set; } = "Server :{0}";

        /// <summary>
        /// Opted into SilverBot emotes
        /// </summary>
        public string OptedIn { get; set; } = "Opted into SilverBot emotes";

        /// <summary>
        /// **All available emotes**
        /// </summary>
        public string AllAvailibleEmotes { get; set; } = "**All available emotes**";

        /// <summary>
        /// You are banned from using the silversocial features
        /// </summary>
        public string UserIsBannedFromSilversocial { get; set; } = "You are banned from using the silversocial features";

        /// <summary>
        /// This server/server owner is banned from using the silversocial features
        /// </summary>
        public string GuildIsBannedFromSilversocial { get; set; } = "This server/server owner is banned from using the silversocial features";

        public string UserIsNotBannedFromSilversocial { get; set; } = "You are not banned POG";
        public string TimeFormat { get; set; } = "G"; //https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings?redirectedfrom=MSDN
        public string UnableToSendDm { get; set; } = "I was unable to send a message to this person";

        public string[] Banstrings { get; set; } = new[]
                {
                 "was yeeted off this server by",
                 "was sniped off this server by",
                 "walked into a cactus whilst trying to escape",
                 "was pummeled by",
                 "experienced kinetic energy whilst trying to escape",
                 "told someone to buy the bedrock edition infront of"
                };

        public string BanDmFront { get; set; } = "You have been banned from ";
        public string KickDmFront { get; set; } = "You have been banned from ";
        public string SupportWarningTitle { get; set; } = "This feature is experimental if you really need support join [here](https://discord.gg/ZT4gZCyReB)";
        public string SupportWhatKind { get; set; } = "What kind of support do you need?";
        public string SupportWhatKindNumbers { get; set; } = "1.Add an command" + Environment.NewLine + "2.Add an splash" + Environment.NewLine + "You should [Click here](https://discord.gg/ZT4gZCyReB) for anything else";
        public string SupportCommandName { get; set; } = "What will this command be like? (e.g. meme)";
        public string SupportCommandAction { get; set; } = "What will this command do? (e.g. show help information about the bot)";
        public string NullInQueue { get; set; } = "Null in queue please report to [here](https://discord.gg/ZT4gZCyReB) thanks";
        public string WarnTitle { get; set; } = "Oh boy";
        public string WarnDescMiddle { get; set; } = " has been warned by "; //Max" has been warned by "SilverDimond
        public string WarnFieldReason { get; set; } = "Reason"; //Reason : posting memes in general
        public string WarnFieldId { get; set; } = "Id"; //Id : 6969
        public string AllServerWarns { get; set; } = "All server warnings";
        public string InfWarned { get; set; } = " warned "; //SilverDimond warned max at 00:69
        public string InfAt { get; set; } = " at "; //SilverDimond warned max at 00:69
        public string NoInfOnServer { get; set; } = "No infraction data on this server";
        public string MemeIsNsfw { get; set; } = "The meme was NSFW";
        public string Credits { get; set; } = "Programming by SilverDimond#1691, Most things by SilverDimond#1691, 69% of splashes by ThePajamaSlime#9391, Useless questining by TheNewRicardo#4935, Microsoft for c#, The c# community for their solutions i didnt use, All the 69 NuGet packeges im using, 362700421191565322 the wierd dog that uses python, And most importantly thank you for using the bot.";
        public string BotName { get; set; } = "SilverBot";
        public string UselessFact { get; set; } = "Useless fact";

        /// <summary>
        /// You must have a higher role than the person you are trying to␣
        /// </summary>
        public string UserHasLowerRole { get; set; } = "You must have a higher role than the person you are trying to ";

        public string Ban { get; set; } = "ban";
        public string Kick { get; set; } = "kick";
        public string BotHasLowerRole { get; set; } = "I must have a higher role than the person you are trying to ";
        public string AvailableLanguages { get; set; } = "Available languages:";
        public string RandomGif { get; set; } = "Random GIF:";

        /// <summary>
        /// Powered by GIPHY
        /// </summary>
        public string PoweredByGiphy { get; set; } = "Powered by GIPHY";

        public string SilverBotCommands { get; set; } = "SilverBot Commands";
        public string AllemotesGuild { get; set; } = "Guild: ";
        public string UserDoesntHaveBanMembers { get; set; } = "You must have BanMembers permission";
        public string UserDoesntHaveKickMembers { get; set; } = "You must have kickmembers permission";
        public string SupportTitle { get; set; } = "Add a command or splash to SilverCraftBot";
        public string SupportDescription { get; set; } = "Splashes https://forms.gle/vYfaKFmqfsZJGc2e8 Commands https://forms.gle/HR3AhCA6yg4LfULU8 Or join https://discord.gg/ZT4gZCyReB";

        /// <summary>
        /// Meme:␣
        /// </summary>
        public string Meme { get; set; } = "Meme: ";

        /// <summary>
        /// You didnt attach a image
        /// </summary>
        /// <remarks>
        /// Ment to be used when there isnt a more better term
        /// </remarks>
        public string NoImageGeneric { get; set; } = "You didnt attach a image";

        /// <summary>
        /// Bruh discord no likey you need something less than 256kb you sent {0}
        /// </summary>
        public string EmoteWasLargerThan256K { get; set; } = "Bruh discord no likey you need something less than 256kb you sent {0}";

        /// <summary>
        /// You attached more than one image
        /// </summary>
        /// <remarks>
        /// Ment to be used when there isnt a more better term
        /// </remarks>
        public string MoreThanOneImageGeneric { get; set; } = "You attached more than one image";

        /// <summary>
        /// The output file is larger than 8mb, its {0}
        /// </summary>
        /// <remarks>
        /// Ment to be used when there isnt a more better term
        /// </remarks>
        public string OutputFileLargerThan8M { get; set; } = "The output file is larger than 8mb, its {0}";

        /// <summary>
        ///Search results for the term `{0}`
        /// </summary>
        [Obsolete("Use Searched_For")]
        public string SearchResultsForTerm { get; set; } = "Search results for the term `{0}`";

        /// <summary>
        /// Page {0}/{1} Use `next` in the next 5 min to see the next page
        /// </summary>
        public string PageGif { get; set; } = "Page {0}/{1} Use `next` in the next 5 min to see the next page";

        /// <summary>
        /// Page {0}/{1}
        /// </summary>
        public string PageNuget { get; set; } = "Page {0}/{1}";

        /// <summary>
        /// You may not send `next` as the 5 minutes expired
        /// </summary>
        public string PeriodExpired { get; set; } = "You may not send `next` as the 5 minutes expired";

        /// <summary>
        /// Hey that isn't a bot, i think.
        /// </summary>
        public string UserIsntBot { get; set; } = "Hey that isn't a bot, i think.";

        /// <summary>
        /// Requested by: |loopbot|
        /// </summary>
        public string Loopbot { get; set; } = "loopbot";

        /// <summary>
        /// Requested by: |queuebot|
        /// </summary>
        public string Queuebot { get; set; } = "queuebot";

        /// <summary>
        /// Now playing: {0} 🔂
        /// </summary>
        public string NowPlayingLoop { get; set; } = "Now playing: {0} 🔂";

        /// <summary>
        /// Now playing: {0}
        /// </summary>
        public string NowPlaying { get; set; } = "Now playing: {0}";

        /// <summary>
        /// Enqueued: {0}
        /// </summary>
        public string Enqueued { get; set; } = "Enqueued: {0}";

        /// <summary>
        /// Skipped: {0}, Now playing {1}
        /// </summary>
        public string SkippedNP { get; set; } = "Skipped: {0}, Now playing {1}";

        /// <summary>
        /// Your vote was counted
        /// </summary>
        public string Voted { get; set; } = "Your vote was counted";

        /// <summary>
        /// Your vote was counted already
        /// </summary>
        public string AlreadyVoted { get; set; } = "Your vote was counted already";

        /// <summary>
        /// Estimated time till it plays: 
        /// </summary>
        public string TimeTillTrackPlays { get; set; } = "Estimated time till it plays:";

        /// <summary>
        /// {0}\nNow playing: {1}
        /// </summary>
        public string ReasonNowPlaying { get; set; } = "{0}\nNow playing: {1}";

        /// <summary>
        /// Uh oh something went wrong. Please try again a little bit later.
        /// </summary>
        public string SearchFail { get; set; } = "Uh oh something went wrong. Please try again a little bit later.";

        public string CultureInfo { get; set; } = "en-GB";

        public CultureInfo GetCultureInfo()
        {
            return new CultureInfo(CultureInfo);
        }

        public ImageThings Imagethings { get; set; } = new();

        private static readonly Dictionary<string, Language> CachedLanguages = new Dictionary<string, Language>();
        private static readonly bool Logging = true;

        public static async System.Threading.Tasks.Task<Language> GetAsync(string a)
        {
            if (CachedLanguages.Count != 0)
            {
                if (CachedLanguages.ContainsKey(a))
                {
                    if (Logging)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[LANGUAGES]");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" huge success retriving ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(a);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" from cache");
                        Console.ResetColor();
                    }

                    return CachedLanguages[a];
                }
                else
                {
                    if (Logging)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[LANGUAGES]");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write($" huge fail loading ");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(a);
                        Console.ResetColor();
                    }

                    return new Language();
                }
            }
            else
            {
                if (Directory.Exists(Environment.CurrentDirectory + @"\languages\"))
                {
                    foreach (var u in Directory.GetFiles(Environment.CurrentDirectory + @"\languages\"))
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
                if (Logging)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[LANGUAGES]");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" done reading languages,");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(CachedLanguages.Count);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" languages loaded");
                    Console.ResetColor();
                }

                return await GetAsync(a);
            }
        }

        public static Language GetLanguageFromCtx(CommandContext ctx)
        {
            return GetLanguageFromId(ctx.Guild?.Id);
        }

        public static Language GetLanguageFromId(ulong? id)
        {
            if (id is null)
            {
                return new Language();
            }
            //TODO: IMPLEMENT THE THING LOL
            return new Language();
        }
    }

    public class ImageThings
    {
        public string JpegSuccess { get; set; } = "There ya go a jpegnized image";
        public string SilverSuccess { get; set; } = "There ya go a silver image";
        public string ComicSuccess { get; set; } = "There ya go a image with the comic filter";
        public string ResizeSuccess { get; set; } = "There ya go a resized image";
        public string TintSuccess { get; set; } = "There ya go a tinted image";
    }
}