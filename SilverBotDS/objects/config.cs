using DSharpPlus.Entities;
using SilverBotDS.Attributes;
using SilverBotDS.Commands;
using SilverBotDS.Commands.Gamering;
using SilverBotDS.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SilverBotDS.Objects
{
    [Serializable]
    public class Config
    {
        [XmlDescription("Array of prefixes the bot will respond to")]
        public string[] Prefix { get; set; } = { "sd!" };

        [XmlDescription("The Discord token, can be got from https://discord.com/developers/")]
        public string Token { get; set; } = "Discord_Token_Here";

        [XmlDescription("The Giphy token, can be got from https://developers.giphy.com/")]
        public string Gtoken { get; set; } = "Giphy_Token_Here";

        [XmlDescription("The FortniteAPI token, can be got from https://dash.fortnite-api.com/")]
        public string FApiToken { get; set; } = "Fortnite_Token_Here";

        [XmlDescription("Location of java.exe to be able to launch lavalink (type java if in path or if using LiNUX)")]
        public string JavaLoc { get; set; } = "C:\\Program Files\\Java\\jdk-13\\bin\\java.exe";

        [XmlDescription("(ulong)Id of main server")]
        public ulong ServerId { get; set; } = 679353407667961877;

        [XmlDescription("Interval to use so discord dont ban us, in ms, is int32 so use -1 if you want no splash changes, defaults to 30m as kae kinda was like hey you know discord has probably put you on a watchlist")]
        public int MsInterval { get; set; } = 1800000;

        [XmlDescription("The current config version, don't change unless told by the bot or silverdimond")]
        public ulong? ConfigVer { get; set; } = null;

        private const ulong CurrentConfVer = 21;

        [XmlDescription("Webhook for logging")]
        public string LogWebhook { get; set; } = "https://discordapp.com/api/webhooks/id/key";

        [XmlDescription("Token used for bot command, leave 'None' to disable")]
        public string TopggSidToken { get; set; } = "None";

        [XmlDescription("Set true if its a sid cookie, false if a bot one")]
        public bool TopggIsSelfbot { get; set; } = true;

        [XmlDescription("What kind of browser to use, used for selinium. 1 for chrome (chromedriver) 2 for firefox")]
        public int BrowserType { get; set; } = 1;

        [XmlDescription("Location of that browser, leave blank for CWD(current working directory)")]
        public string DriverLocation { get; set; } = "";

        [XmlDescription("The Lavalink Node restful HTTP api URI.")]
        public string LavalinkRestUri { get; set; } = "http://localhost:2333/";

        [XmlDescription("The Lavalink Node WebSocket URI.")]
        public string LavalinkWebSocketUri { get; set; } = "ws://localhost:2333/";

        [XmlDescription("The Lavalink Node Password.")]
        public string LavalinkPassword { get; set; } = "youshallnotpass";

        [XmlDescription("Friday text channel. leave none to disable")]
        public ulong FridayTextChannel { get; set; }

        [XmlDescription("Friday voice channel. leave none to disable")]
        public ulong FridayVoiceChannel { get; set; }

        [XmlDescription("What kind of database to use, 1 for postgres (slower but a traditional database), 2 for litedb (faster if your local disk is fast, not really a database))")]
        public int DatabaseType { get; set; } = 1;

        [XmlDescription("Conection string for database, Unused if using litedb, make null if stored in DATABASE_URL")]
        public string ConnString { get; set; } = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

        [XmlDescription("Uses Jering.Javascript.NodeJS to execute some javascript code")]
        public bool UseNodeJs { get; set; } = false;

        [XmlDescription("Allow silverbot to download and start Lavalink, if you disable this your responsible for launching lavalink")]
        public bool AutoDownloadAndStartLavalink { get; set; } = true;

        [XmlDescription("Allow silverbot to load (or make) a colors config, useful if you plan on adding colors")]
        public bool ColorConfig { get; set; } = true;

        [XmlDescription("Allow silverbot to connect to lavalink and use audio commands, useful if you have lavalink installed or if you allowed silverbot to install and run lavalink (requires java)")]
        public bool UseLavaLink { get; set; } = true;

        [XmlDescription("Allow silverbot to emulate bubot (enables bibi picture commands and other stuff)")]
        public bool EmulateBubot { get; set; } = false;

        [XmlDescription("Allow silverbot owner only commands (may allow the bot owner to kill pc)")]
        public bool AllowOwnerOnlyCommands { get; set; } = true;

        [XmlDescription("Allow commands that someone might find distasteful like the fake nsfw commands that just expose everyone that try's to run them")]
        public bool AllowTrollCommands { get; set; } = false;

        [XmlDescription("A temporary way to disable the interactivity tracker")]
        public bool SitInVc { get; set; } = false;

        [XmlDescription("Stuff to log into microsoft, leave as is while i am developing calendar stuff")]
        public string MicrosoftGraphClientId { get; set; } = "Graph-Client-Id-Here";

        [XmlDescription("Stuff to use spotify with, leave as is if you are too bored to make a spotify app if you arent go to https://developer.spotify.com/dashboard")]
        public string SpotifyClientId { get; set; } = "Spotify_CLIENT_ID";

        public string SpotifyClientSecret { get; set; } = "Spotify_CLIENT_SECRET";

        [XmlDescription("Do we enable the shitty server statistics")]
        public bool EnableServerStatistics { get; set; } = true;

        public Splash[] Splashes { get; set; } = {
      new("D̶U̶K̶T̶  Silver Hosting", ActivityType.Watching),
      new(".NET gang", ActivityType.Watching),
      new("C# gang", ActivityType.Watching),
      new("畂桳栠摩琠敨映捡獴", ActivityType.Watching),
      new("As seen on TV!", ActivityType.Playing),
      new("100% pure!", ActivityType.Playing),
      new("20 GOTO 10!", ActivityType.Playing),
      new("Déjà vu!", ActivityType.Playing),
      new("Déjà vu!", ActivityType.Playing),
      new("Does barrel rolls!", ActivityType.Playing),
      new("Follow the train, CJ!", ActivityType.Playing),
      new("NOT Google anlyticsed!", ActivityType.Playing),
      new("Beta!", ActivityType.Playing),
      new("Internet enabled!", ActivityType.Playing),
      new("David please look at your table", ActivityType.Playing),
      new("Its a bot!", ActivityType.Playing),
      new("Made in Macedonia", ActivityType.Playing),
      new("Now supports emoji 😉", ActivityType.Playing),
      new("Creeper aw man.", ActivityType.Playing),
      new("if not OK then return end", ActivityType.Playing),
      new("Now on {platform}.", ActivityType.Playing),
      new("splashes.json", ActivityType.Playing),
      new("Limited edition!", ActivityType.Playing),
      new("Should not be texted while driving", ActivityType.Playing),
      new("Should not be tested while driving", ActivityType.Playing),
      new("Try it!", ActivityType.Playing),
      new("Have you finished your homework?", ActivityType.Playing),
      new("UwU whats dis", ActivityType.Playing),
      new("Digital Dancing!", ActivityType.Playing),
      new("5 is best number!", ActivityType.Playing),
      new("C#!", ActivityType.Playing),
      new("Plastic Guns!", ActivityType.Playing),
      new("Lines from 2004!", ActivityType.Playing),
      new("Absolutely no sarcasm!", ActivityType.Playing),
      new("Thinking involved!", ActivityType.Playing),
      new("huehuehue", ActivityType.Playing),
      new("!gnilleps sdrawkcab", ActivityType.Playing),
      new("xXPr0$n1p3$Xx", ActivityType.Playing),
      new("Sharpshooter!", ActivityType.Playing),
      new("Tsar Bomba!", ActivityType.Playing),
      new("Vive l'atome!", ActivityType.Playing),
      new("10% Milk!", ActivityType.Playing),
      new("Available in stores!", ActivityType.Playing),
      new("Gluten Free!", ActivityType.Playing),
      new("But wait there's more!", ActivityType.Playing),
      new("How much do you think SilverBot is worth. Think one minute before you anwser.", ActivityType.Playing),
      new("RREEEEE", ActivityType.Playing),
      new("except in nebraska", ActivityType.Playing),
      new("Above Average Man!", ActivityType.Playing),
      new("ThinThick is better than Swag", ActivityType.Playing),
      new("Glorified Drift Wood!", ActivityType.Playing),
      new("T0P K3K", ActivityType.Playing),
      new("Maximizing Profits!", ActivityType.Playing),
      new("Somewhat Persuasive!", ActivityType.Playing),
      new("am vary smert", ActivityType.Playing),
      new("Blue!", ActivityType.Playing),
      new("splashes.json", ActivityType.Playing),
      new("Green!", ActivityType.Playing),
      new("1nd Floor!", ActivityType.Playing),
      new("noice", ActivityType.Playing),
      new("wait this isn't mee6", ActivityType.Playing),
      new("fReE cAkE>>", ActivityType.Playing),
      new("Exclamation Mark!", ActivityType.Playing),
      new("Strongly Dislikes Birch!", ActivityType.Playing),
      new("Bestest Grammar!", ActivityType.Playing),
      new("feedback plz", ActivityType.Playing),
      new("Relevant Text!", ActivityType.Playing),
      new("finally in beta", ActivityType.Playing),
      new("needs update", ActivityType.Playing),
      new("Woah!", ActivityType.Playing),
      new("fruit salad is best salad", ActivityType.Playing),
      new("75% less random!", ActivityType.Playing),
      new("Move now, think later!", ActivityType.Playing),
      new("*teleports behind you*", ActivityType.Playing),
      new("4 Letters!", ActivityType.Playing),
      new("Brandon yes", ActivityType.Playing),
      new("Less than Minimum!", ActivityType.Playing),
      new("****need copy and link****", ActivityType.Playing),
      new("Flatter than Kansas!", ActivityType.Playing),
      new("Not a Fan of Fuzzies!", ActivityType.Playing),
      new("Consider Fullscreen!", ActivityType.Playing),
      new("The Exits are... Irrelevant!", ActivityType.Playing),
      new("The facts they dont want you to know", ActivityType.Playing),
      new("max should not be trusted", ActivityType.Playing),
      new("LE:L", ActivityType.Playing),
      new("requires additional pylons", ActivityType.Playing),
      new("THAT FACE DOWN", ActivityType.Playing),
      new("Heterogeneous Greens!", ActivityType.Playing),
      new("Old White French House!", ActivityType.Playing),
      new("Really Dark Purples!", ActivityType.Playing),
      new("Two Ups Thumb!", ActivityType.Playing),
      new("Gilden Ultra Cotton Pocket Tee!", ActivityType.Playing),
      new("Crazy Cool Coincidence!", ActivityType.Playing),
      new("Over the mountains, into Terrellia!", ActivityType.Playing),
      new("Your content is good!", ActivityType.Playing),
      new("striker disapproves", ActivityType.Playing),
      new("Fools' Gold!", ActivityType.Playing),
      new("Powered by Wind!", ActivityType.Playing),
      new("Globular Resources!", ActivityType.Playing),
      new("Falcon Heavy!", ActivityType.Playing),
      new("shoutout to you", ActivityType.Playing),
      new("Acid Trips!", ActivityType.Playing),
      new("lttstore.com", ActivityType.Playing),
      new("Wonder-filly-ful!", ActivityType.Playing),
      new("Nostalgic!", ActivityType.Playing),
      new("Giv quadratic formula or I game end", ActivityType.Playing),
      new("T-shape is cancer", ActivityType.Playing),
      new("som splashes for u ^_^", ActivityType.Playing),
      new("1% less memory!", ActivityType.Playing),
      new("Kalorichen has invited you to play Minecraft", ActivityType.Playing),
      new("Kalorichan has invited you to play Minecraft", ActivityType.Playing),
      new("ThePajamaSlime! ThePajamaSlime! ThePajamaSlime!", ActivityType.Playing),
      new("Xbox 360 Edition", ActivityType.Playing),
      new("sign in with your microsoft account", ActivityType.Playing),
      new("Now with 69% more ThePajamaSlime!", ActivityType.Playing),
      new("Teach me the stats!", ActivityType.Playing),
      new("Teach me the strats!", ActivityType.Playing),
      new("capped image of caps with captions in caps", ActivityType.Playing),
      new("buy shark tale on dvd", ActivityType.Playing),
      new("subscribe to idots", ActivityType.Playing),
      new("Cinnamon oat cereal!", ActivityType.Playing),
      new("Silver chest unlocked!", ActivityType.Playing),
      new("Flame-Headed Boy!", ActivityType.Playing),
      new("lamo", ActivityType.Playing),
      new("Non-Newtonian Fluids!", ActivityType.Playing),
      new("Intensive and Extensive!", ActivityType.Playing),
      new("Aquatic!", ActivityType.Playing),
      new("4TH DOWN AND 9!", ActivityType.Playing),
      new("pls r8 5", ActivityType.Playing),
      new("dabbing on them endermans", ActivityType.Playing),
      new("GIMP FTW", ActivityType.Playing),
      new("Bush hid the facts", ActivityType.Playing),
      new("ugh, finally, no more sugar", ActivityType.Playing),
      new("1 splash per update?", ActivityType.Playing),
      new("uploading...41%", ActivityType.Playing),
      new("my stuff stuff", ActivityType.Playing),
      new("teleporting-porter-pearl", ActivityType.Playing),
      new("bot now has graphics, you're welcome", ActivityType.Playing),
      new("(it's a json file with a list, not a method)", ActivityType.Playing),
      new("superb bus service", ActivityType.Playing),
      new("SELECT * FROM BITCHES WHERE (NAME=\"MAX\")", ActivityType.Playing),
      new("SELCET? AWESMOE!", ActivityType.Playing),
      new("pretty pink spigots", ActivityType.Playing),
      new("you're uh winner", ActivityType.Playing),
      new("wifi killed the dinosaurs", ActivityType.Playing),
      new("60 layers of wax", ActivityType.Playing),
      new("12 years later", ActivityType.Playing),
      new("it came in teal!", ActivityType.Playing),
      new("pretend the sun is real", ActivityType.Playing),
      new("people hate people", ActivityType.Playing),
      new("north of what?", ActivityType.Playing),
      new("non-lethal stabbing", ActivityType.Playing),
      new("pertrify yourself!", ActivityType.Playing),
      new("pay the poor", ActivityType.Playing),
      new("check your spam!", ActivityType.Playing),
      new("see it with your ears!", ActivityType.Playing),
      new("hear it with your eyes!", ActivityType.Playing),
      new("it'll change, soon!", ActivityType.Playing),
      new("space is key!", ActivityType.Playing),
      new("for the love of god dont use dukt hosting", ActivityType.Playing),
      new("max is bad?", ActivityType.Playing),
      new("Smash Mouth - All Star", ActivityType.ListeningTo),
      new("indentation error", ActivityType.Watching),
      new("in 5,4,3,2,1 I know you see", ActivityType.Playing),
      new("the world will change for me", ActivityType.Playing),
      new("and be so wonderful", ActivityType.Playing),
      new("https://www.youtube.com/watch?v=HPM05dQgdig", ActivityType.Playing),
      new("RIP XBOX360", ActivityType.Playing),
      new("https://catjam.tk/", ActivityType.Playing),
      new("Leave it all to me", ActivityType.ListeningTo),
      new("Thanks ThePajamaSlime!", ActivityType.Playing),
      new("10000000% c# by 1969", ActivityType.Playing),
      new("Proudly European", ActivityType.Playing),
      new("@everyone", ActivityType.Playing),
      new("2 meters apart", ActivityType.Playing),
      new("ERROR", ActivityType.Playing),
      new("THE CAKE IS A LIE", ActivityType.Playing),
      new("ThE CaKe Is A lIe", ActivityType.Playing),
      new("Developers", ActivityType.ListeningTo),
      new("Developers music video", ActivityType.ListeningTo),
      new("GREAT SUCCESS!", ActivityType.Playing),
      new("Doki Doki literature club", ActivityType.Playing),
    };

        private static XmlDocument MakeDocumentWithComments(XmlDocument xmlDocument)
        {
            foreach (var i in typeof(Config).GetMembers())
            {
                foreach (var e in i.GetCustomAttributes(false))
                {
                    if (e.GetType() == typeof(XmlDescriptionAttribute))
                    {
                        xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, $"/Config/{i.Name}", ((XmlDescriptionAttribute)e).description);
                    }
                }
            }

            return xmlDocument;
        }

        public static async Task OutdatedConfigTask(Config readconfig)
        {
            Console.WriteLine("Outdated config detected. Would you like a new one to generate? (Y/n)");
            var rl = Console.ReadLine();
            if (rl != null)
                switch (rl.ToLowerInvariant())
                {
                    case "y":
                        using (var streamReader = new StreamReader("silverbot.xml"))
                        {
                            await using (var streamWriter = new StreamWriter("silverbotold.xml", false))
                            {
                                await streamWriter.WriteAsync(await streamReader.ReadToEndAsync());
                                streamWriter.Close();
                            }

                            streamReader.Close();
                        }

                        await using (var streamWriter = new StreamWriter("silverbot.xml"))
                        {
                            if (readconfig.ConfigVer < 20 && File.Exists("splashes.json"))
                            {
                                using StreamReader reader = new("splashes.json");
                                readconfig.Splashes = JsonSerializer.Deserialize<DiscordActivity[]>(json: await reader.ReadToEndAsync()).Select(item => Splash.GetFromDiscordActivity(item)).ToArray();
                            }
                            readconfig.ConfigVer = CurrentConfVer;

                            MakeDocumentWithComments(Xmlutils.SerializeToXmlDocument(readconfig)).Save(streamWriter);
                            streamWriter.Close();
                        }

                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + Program.DirSlash + "silverbot.xml")
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + Program.DirSlash + "silverbotold.xml")
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                        Console.WriteLine(
                            "Ok now transfer the values from silverbotold.xml to silverbot.xml and restart thx");
                        Environment.Exit(420);
                        break;

                    case "n":
                        Console.WriteLine(
                            "Outdated config detected. Would you like to try loading with the outdated config? (Y/n)");
                        var nrl = Console.ReadLine();
                        if (nrl != null)
                        {
                            switch (nrl.ToLowerInvariant())
                            {
                                case "y":
                                    {
                                        break;
                                    }
                                case "n":
                                    {
                                        Environment.Exit(421);
                                        break;
                                    }
                                default:
                                    {
                                        await OutdatedConfigTask(readconfig);
                                        break;
                                    }
                            }
                        }

                        break;

                    default:
                        await OutdatedConfigTask(readconfig);
                        break;
                }
        }

        public static async Task<Config> GetAsync()
        {
            var serializer = new XmlSerializer(typeof(Config));

            if (!File.Exists("silverbot.xml"))
            {
                await using (var streamWriter = new StreamWriter("silverbot.xml"))
                {
                    MakeDocumentWithComments(Xmlutils.SerializeToXmlDocument(new Config
                    {
                        ConfigVer = CurrentConfVer
                    })).Save(streamWriter);
                    streamWriter.Close();
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + Program.DirSlash + "silverbot.xml")
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.xml should have opened in the default app, edit it, save it and press enter");

                    Console.WriteLine("Press any key WHEN READY to continue...");
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.xml should exist in the CWD, edit it, save it and restart silverbot");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Environment.Exit(420);
                }
            }

            await using var fs = File.Open("silverbot.xml", FileMode.Open);
            var readconfig = serializer.Deserialize(fs) as Config;
            fs.Close();
            if (readconfig != null && (readconfig.ConfigVer == null || readconfig.ConfigVer != CurrentConfVer))
            {
                await OutdatedConfigTask(readconfig);
            }
            Giphy.Set(readconfig?.Gtoken);
            Fortnite.Setapi(readconfig?.FApiToken);
            return readconfig;
        }
    }
}