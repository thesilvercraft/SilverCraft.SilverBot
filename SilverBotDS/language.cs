using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SilverBotDS
{
    internal class Language
    {
        /// <summary>
        /// <b>Hi </b>@max<b>!</b>
        /// <para/>
        /// <b>Hi </b>{0}<b>!</b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>String.Format(Language.Hi,Username);</c>
        /// </remarks>
        public string Hi { get; set; } = "Hi {0}!";

        /// <summary>
        /// <b>Time in UTC is </b>01.1.2021 17:20:28
        /// <para/>
        /// <b>Time in UTC is </b>{0}
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>String.Format(Language.Time_In_Utc,DateTime.NowUTC.ToString(Language.Time_format));</c>
        /// </remarks>
        public string Time_In_Utc { get; set; } = "Time in UTC is {0}";

        /// <summary>
        /// <b>Requested by </b>SilverDimond
        /// <para/>
        /// <b>Requested by </b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>Language.Requested_by+Username</c> <strong>MAY BE SUBJECT TO CHANGE</strong>
        /// </remarks>

        public string Requested_by { get; set; } = "Requested by ";

        /// <summary>
        /// Default(EN)
        /// </summary>
        public string Lang_name { get; set; } = "Default(EN)";

        /// <summary>
        /// must be en or de for useless facts
        /// </summary>
        public string Lang_code_for_useless_facts { get; set; } = "en";

        //Music related
        /// <summary>
        /// I'm already connected to a voice channel!
        /// </summary>
        public string Already_connected { get; set; } = "I'm already connected to a voice channel!";

        /// <summary>
        /// You must be connected to a voice channel!
        /// </summary>
        public string User_not_connected { get; set; } = "You must be connected to a voice channel!";

        /// <summary>
        /// <b>Joined </b>Testing<b>!</b>
        /// </summary>
        /// <para/>
        /// <b>Joined </b>Testing<b>!</b>
        /// </summary>
        /// <remarks>
        /// Needs to be used like <c>Language.Requested_by+Username</c> <strong>MAY BE SUBJECT TO CHANGE</strong>
        /// </remarks>
        public string Joined { get; set; } = "Joined {0}!";

        /// <summary>
        /// I'm not connected to a voice channel."
        /// </summary>
        public string Not_connected { get; set; } = "I'm not connected to a voice channel.";

        /// <summary>
        /// Woaaah there, I'm not playing any tracks.
        /// </summary>
        public string Not_playing { get; set; } = "Woaaah there, I'm not playing any tracks.";

        /// <summary>
        /// Never gonna give you up<b>|</b> by <b>|</b>Rick Astley
        /// </summary>
        public string Song_by_author { get; set; } = " by "; //Never gonna give you up" by "Rick Astley

        public string Use_dj_message { get; set; } = "Tip: A Role Named ***DJ*** Will Restrict Access To Members With The Role.";
        public string Track_not_exist { get; set; } = "That track doesnt exist my dude";
        public string Removed_front { get; set; } = "Removed: "; //"Removed: "Never gonna give you up by Rick Astley
        public string Paused_front { get; set; } = "Paused: "; //"Paused: "Never gonna give you up by Rick Astley
        public string Resumed_front { get; set; } = "Resumed: "; //"Resumed: "Never gonna give you up by Rick Astley
        public string No_search_term { get; set; } = "Please provide a search term.";
        public string Not_sure_which_channel { get; set; } = "Not sure which voice channel to disconnect from.";
        public string Seeked_front { get; set; } = "I've seeked: `"; //"I've seeked: `"Never gonna give you up by Rick Astley to 00:00:00.
        public string Seeked_middle { get; set; } = "` to "; //I've seeked: `Never gonna give you up"` to "00:00:00.
        public string Seeked_back { get; set; } = "."; //I've seeked: `Never gonna give you up` to 00:00:00"."
        public string No_lyrics_found { get; set; } = "No lyrics found for "; //"No lyrics found for Never gonna give you up
        public string No_longer_playing { get; set; } = "No longer playing anything.";
        public string Already_stoped { get; set; } = "Woaaah there, I can't stop the stopped forced.";
        public string Already_voted { get; set; } = "You can't vote again.";
        public string Voting_is_below_85_percent { get; set; } = "You need more than 85% votes to skip this song.";
        public string User_doesnt_have_dj { get; set; } = "You must have a role named `dj`!";
        public string Looping_song { get; set; } = "Now loopin song 🔂";
        public string Looping_queue { get; set; } = "Now loopin queue 🔁";
        public string Not_looping { get; set; } = "Now not loopin.";
        public string No_results_front { get; set; } = "I wasn't able to find anything for `"; //"I wasn't able to find anything for `"Never gonna give you up`.
        public string No_results_back { get; set; } = "`."; //I wasn't able to find anything for `Never gonna give you up"`."
        public string Noone_in_vc { get; set; } = "Noone is in voice chat. 😥";
        public string Next_item_not_track { get; set; } = "Next item in queue is not a track. 😥";
        public string Queue_completed { get; set; } = "Queue completed! Please add more tracks to continue!";
        public string Left_front { get; set; } = "I've left ";
        public string Left_back { get; set; } = "!";
        public string Goodbye { get; set; } = "Goodbye";

        //Commands
        public string Test { get; set; } = "test";

        public string User { get; set; } = "User: ";
        public string Userid { get; set; } = "ID";
        public string Has_joined_support { get; set; } = "Has joined the SilverCraft Server";
        public string Is_admin { get; set; } = "Is a SilverCraft bot admin";
        public string Is_a_dj { get; set; } = "Is a DJ";
        public string Is_an_onwer { get; set; } = "Is a SilverCraft bot owner";
        public string Is_a_bot { get; set; } = "Is a bot";
        public string Error { get; set; } = "Error";
        public string Error_sent_to_logs { get; set; } = "Complete error sent to Bot logs";
        public string Silverhosting_joke_title { get; set; } = "SilverCraftBot sponsored by SilverHosting";
        public string Silverhosting_joke_description { get; set; } = "Use offer code [SLVR](https://www.youtube.com/watch?v=dQw4w9WgXcQ)";
        public string Purge_number_negative { get; set; } = "The amount of messages to remove must be positive.";
        public string Purge_nothing_to_delete { get; set; } = "Nothing to delete.";
        public string Bot_guilds { get; set; } = "Bot guilds";
        public string Purge_removed_front { get; set; } = "Done. Removed "; //"Done. Removed "69 messages
        public string Purge_removed_single { get; set; } = " message"; //Done. Removed 1" message"
        public string Purge_removed_plural { get; set; } = " messages"; //Done. Removed 69" messages"
        public string Invite_title { get; set; } = "SilverCraftBot invite link";
        public string Invite_description { get; set; } = "SilverCraftBot is in open beta https://discord.com/api/oauth2/authorize?client_id=702445582559739976&permissions=2147483639&scope=bot";
        public string No_emotes_found { get; set; } = "**No emote found**";
        public string Searched_for { get; set; } = "Searched for:`"; //"Searched for:`"UR MOM`
        public string Searched_for_back { get; set; } = "`"; //Searched for:`UR MOM"`"
        public string Multiple_emotes_found { get; set; } = "**Multiple emotes found**";
        public string Already_opted_in { get; set; } = "Already opted into SilverBot emotes";
        public string Opted_in { get; set; } = "Opted into SilverBot emotes";
        public string All_availible_emotes { get; set; } = "**All available emotes**";
        public string User_is_banned_from_silversocial { get; set; } = "You are banned from using the silversocial features";
        public string User_is_not_banned_from_silversocial { get; set; } = "You are not banned POG";
        public string Time_format { get; set; } = "G"; //https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings?redirectedfrom=MSDN
        public string Unable_to_send_dm { get; set; } = "I was unable to send a message to this person";

        public string[] Banstrings { get; set; } = new[]
                {
                 "was yeeted off this server by",
                 "was sniped off this server by",
                 "walked into a cactus whilst trying to escape",
                 "was pummeled by",
                 "experienced kinetic energy whilst trying to escape",
                 "told someone to buy the bedrock edition infront of"
                };

        public string Ban_dm_front { get; set; } = "You have been banned from ";
        public string Kick_dm_front { get; set; } = "You have been banned from ";
        public string Support_warning_title { get; set; } = "This feature is experimental if you really need support join [here](https://discord.gg/ZT4gZCyReB)";
        public string Support_what_kind { get; set; } = "What kind of support do you need?";
        public string Support_what_kind_numbers { get; set; } = "1.Add an command" + Environment.NewLine + "2.Add an splash" + Environment.NewLine + "You should [Click here](https://discord.gg/ZT4gZCyReB) for anything else";
        public string Support_command_name { get; set; } = "What will this command be like? (e.g. meme)";
        public string Support_command_action { get; set; } = "What will this command do? (e.g. show help information about the bot)";
        public string Null_in_queue { get; set; } = "Null in queue please report to [here](https://discord.gg/ZT4gZCyReB) thanks";
        public string Warn_title { get; set; } = "Oh boy";
        public string Warn_desc_middle { get; set; } = " has been warned by "; //Max" has been warned by "SilverDimond
        public string Warn_field_reason { get; set; } = "Reason"; //Reason : posting memes in general
        public string Warn_field_id { get; set; } = "Id"; //Id : 6969
        public string All_server_warns { get; set; } = "All server warnings";
        public string Inf_warned { get; set; } = " warned "; //SilverDimond warned max at 00:69
        public string Inf_at { get; set; } = " at "; //SilverDimond warned max at 00:69
        public string No_inf_on_server { get; set; } = "No infraction data on this server";
        public string Meme_is_nsfw { get; set; } = "The meme was NSFW";
        public string Credits { get; set; } = "Programming by SilverDimond#1691, Most things by SilverDimond#1691, 69% of splashes by ThePajamaSlime#9391, Useless questining by TheNewRicardo#4935, Microsoft for c#, The c# community for their solutions i didnt use, All the 69 NuGet packeges im using, 362700421191565322 the wierd dog that uses python, And most importantly thank you for using the bot.";
        public string Bot_Name { get; set; } = "SilverCraft Bot";
        public string Useless_fact { get; set; } = "Useless fact";
        public string User_has_lower_role { get; set; } = "You must have a higher role than the person you are trying to ";
        public string Ban { get; set; } = "ban";
        public string Kick { get; set; } = "kick";
        public string Bot_has_lower_role { get; set; } = "I must have a higher role than the person you are trying to ";
        public string Availible_languages { get; set; } = "Availible languages:";
        public string Random_GIF { get; set; } = "Random GIF:";
        public string Powered_by_GIPHY { get; set; } = "Powered by GIPHY";
        public string SilverBot_Commands { get; set; } = "SilverBot Commands";
        public string Allemotes_Guild { get; set; } = "Guild: ";
        public string User_Doesnt_Have_BanMembers { get; set; } = "You must have BanMembers permission";
        public string User_Doesnt_Have_KickMembers { get; set; } = "You must have kickmembers permission";
        public string Support_Title { get; set; } = "Add a command or splash to SilverCraftBot";
        public string Support_Description { get; set; } = "Splashes https://forms.gle/vYfaKFmqfsZJGc2e8 Commands https://forms.gle/HR3AhCA6yg4LfULU8 Or join https://discord.gg/ZT4gZCyReB";
        public string Search_Results_for_term { get; set; } = "Search results for the term `{0}`";

        /// <summary>
        /// Page {0}/{1} Use `next` in the next 5 min to see the next page
        /// </summary>
        public string Page_Gif { get; set; } = "Page {0}/{1} Use `next` in the next 5 min to see the next page";

        /// <summary>
        /// You may not send `next` as the 5 minutes expired
        /// </summary>
        public string Period_Expired { get; set; } = "You may not send `next` as the 5 minutes expired";

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
        public string Now_Playing_Loop { get; set; } = "Now playing: {0} 🔂";

        /// <summary>
        /// {0}\nNow playing: {1}
        /// </summary>
        public string Now_Playing { get; set; } = "{0}\nNow playing: {1}";

        private static Dictionary<string, Language> CachedLanguages = new Dictionary<string, Language>();
        private static readonly bool logging = true;

        public static Language Get(string a)
        {
            if (CachedLanguages.Count != 0)
            {
                if (CachedLanguages.ContainsKey(a))
                {
                    if (logging)
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
                    if (logging)
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
                    foreach (string u in Directory.GetFiles(Environment.CurrentDirectory + @"\languages\"))
                    {
                        using Stream stream = File.OpenRead(u);
                        StreamReader reader = new StreamReader(stream);
                        string content = reader.ReadToEnd();
                        reader.Dispose();
                        Language asdf = JsonSerializer.Deserialize<Language>(content);
                        string name = Path.GetFileNameWithoutExtension(u);
                        CachedLanguages.Add(name, asdf);
                    }
                }
                if (logging)
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

                return Get(a);
            }
        }

        public static Language GetLanguageFromId(ulong id)
        {
            //TODO: IMPLEMENT THE THING LOL
            return new Language();
        }
    }
}