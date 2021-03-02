using SilverBotDS.Attributes;
using SilverBotDS.Commands;
using SilverBotDS.Commands.Gamering;
using SilverBotDS.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Threading.Tasks;

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

        private const ulong CurrentConfVer = 8;

        [XmlDescription("Does the bot use the: True-Config or False-Internal splashes")]
        public bool UseSplashConfig { get; set; } = true;

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

        [XmlDescription("The Lavalink Node WebSocket host(name).")]
        public string LavalinkWebSocketUri { get; set; } = "ws://localhost:2333/";

        [XmlDescription("The Lavalink Node Password.")]
        public string LavalinkPassword { get; set; } = "youshallnotpass";

        [XmlDescription("Friday text channel. leave none to disable")]
        public ulong FridayTextChannel { get; set; }

        [XmlDescription("Friday voice channel. leave none to disable")]
        public ulong FridayVoiceChannel { get; set; }

        [XmlDescription("What kind of database to use, 1 for postgres, 2 for litedb (recommended)")]
        public int DatabaseType { get; set; } = 1;

        [XmlDescription("Conection string for database, Unused if using litedb, make null if stored in DATABASE_URL")]
        public string ConnString { get; set; } = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

        [XmlDescription("Do we even try to load the aiml bot")]
        public bool UseAimlBot { get; set; } = false;

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
                switch (rl.ToLower())
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
                            readconfig.ConfigVer = CurrentConfVer;
                            MakeDocumentWithComments(Xmlutils.SerializeToXmlDocument(readconfig)).Save(streamWriter);
                            streamWriter.Close();
                        }

                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\silverbot.xml")
                            {
                                UseShellExecute = true
                            }
                        }.Start();
                        new Process
                        {
                            StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\silverbotold.xml")
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
                            switch (nrl.ToLower())
                            {
                                case "y":
                                    break;

                                case "n":
                                    Environment.Exit(421);
                                    break;

                                default:
                                    await OutdatedConfigTask(readconfig);
                                    break;
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
                        StartInfo = new ProcessStartInfo(Environment.CurrentDirectory + "\\silverbot.xml")
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.xml should have opened in the default app, edit it, save it and press enter");
                    Thread.Sleep(1000);
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
            SetDatabase(readconfig);
            Giphy.Set(readconfig?.Gtoken);
            Fortnite.Setapi(readconfig?.FApiToken);
            return readconfig;
        }

        public static void SetDatabase(Config config)
        {
            switch (config.DatabaseType)
            {
                case 1:
                    {
                        //postgres
                        PostgreDatabase postgre;
                        if (config != null && !string.IsNullOrEmpty(config.ConnString))
                        {
                            postgre = new(config.ConnString);
                        }
                        else
                        {
                            var tmp = new Uri(Environment.GetEnvironmentVariable("DATABASE_URL") ?? throw new InvalidOperationException());
                            var usernameandpass = tmp.UserInfo.Split(":");
                            var connString = $"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}";
                            postgre = new(connString);
                        }
                        Console.WriteLine("Using a postgre database");
                        Program.SetDatabase(postgre);
                        break;
                    }
                case 2:
                    {
                        //litedb

                        Console.WriteLine("Using a litedb database");
                        Program.SetDatabase(new LiteDBDatabase());
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}