using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace SilverBotDS.Config
{
    public class Config
    {
        public string[] Prefix { get; set; } = { "sd!" };

        public string Token { get; set; } = "Discord_Token_Here";
        public string Gtoken { get; set; } = "Giphy_Token_Here";
        public string FApiToken { get; set; } = "Fartnite_Token_Here";
        public string JavaLoc { get; set; } = "C:\\Program Files\\Java\\jdk-13\\bin\\java.exe";
        public ulong LogChannel { get; set; } = 717166388044628018;
        public ulong ServerId { get; set; } = 679353407667961877;
        public ulong AdminRoleId { get; set; } = 746821906602131506;
        public List<ulong> Botowners { get; set; } = new List<ulong> { 264081339316305920 };
        public string ConnString { get; set; } = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";
        public int MsInterval { get; set; } = 1800000;
        public ulong? ConfigVer { get; set; } = null;
        public static ulong CurrentConfVer = 1;

        public static XmlDocument MakeDocumentWithComments(XmlDocument xmlDocument, bool isexample = false)
        {
            if (isexample)
            {
                xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/Prefix", "DONT EDIT THIS DOCUMENT, COMPARE IT TO SILVERBOT.XML AND EDIT THAT");
            }
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/Prefix", "Array of prefixes the bot will respond to");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/Token", "The Discord token, can be got from https://discord.com/developers/");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/Gtoken", "The Giphy token, can be got from https://developers.giphy.com/");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/FApiToken", "The FortniteAPI token, can be got from https://dash.fortnite-api.com/");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/JavaLoc", "Location of java.exe to be able to launch lavalink (type java if in path or if using LiNUX)");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/LogChannel", "(ulong)Id of channel for general logging");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/ServerId", "(ulong)Id of Server where the bot admin role can be found");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/AdminRoleId", "(ulong)Id of bot admin role");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/Botowners", "(List<ulong>)Id's of bot owners");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/ConnString", "PostgresSQL conection string make null if stored in ENV");
            xmlDocument = Xmlutils.CommentBeforeObject(xmlDocument, "/Config/MsInterval", "Interval to use so discord dont ban us, in ms, is int32 so use -1 if you want no splash changes, defaults to 30m as kae kinda was like hey you know discord has probably put you on a watchlist");
            return xmlDocument;
        }

        public static Config Get()
        {
            Config new_config = new Config
            {
                ConfigVer = CurrentConfVer
            };
            XmlSerializer serializer = new XmlSerializer(new_config.GetType());
            try
            {
                if (!File.Exists("silverbot.xml"))
                {
                    using (StreamWriter streamWriter = new StreamWriter("silverbot.xml"))
                    {
                        MakeDocumentWithComments(Xmlutils.SerializeToXmlDocument(new_config)).Save(streamWriter);
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

                using FileStream fs = File.Open("silverbot.xml", FileMode.Open);
            Config readconfig = serializer.Deserialize(fs) as Config;
            fs.Close();
            if (readconfig.ConfigVer == null || readconfig.ConfigVer < CurrentConfVer)
            {
            outdated:
                Console.WriteLine("Outdated config detected. Would you like a new one to generate? (Y/n)");
                string rl = Console.ReadLine();
                switch (rl.ToLower())
                {
                    case "y":
                        using (StreamReader streamReader = new StreamReader("silverbot.xml"))
                        {
                            using (StreamWriter streamWriter = new StreamWriter("silverbotnew.xml"))
                            {
                                streamWriter.Write(streamReader.ReadToEnd());
                                streamWriter.Close();
                            }
                            streamReader.Close();
                        }
                        using (StreamWriter streamWriter = new StreamWriter("silverbot.xml"))
                        {
                            MakeDocumentWithComments(Xmlutils.SerializeToXmlDocument(new_config)).Save(streamWriter);
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
                        Console.WriteLine("Ok now transfer the values from silverbotold.xml to silverbot.xml and restart thx");
                        Environment.Exit(420);
                        break;

                    case "n":
                        Console.WriteLine("Outdated config detected. Would you like to try loading with the outdated config? (Y/n)");
                        string nrl = Console.ReadLine();
                        switch (nrl.ToLower())
                        {
                            case "y":
                                break;

                            case "n":
                                Environment.Exit(421);
                                break;

                            default:
                                goto outdated;
                        }
                        break;

                    default:
                        goto outdated;
                }
            }
            if (!string.IsNullOrEmpty(readconfig.ConnString))
            {
                Database.Setconnstring(readconfig.ConnString);
            }
            else
            {
                Uri tmp = new Uri(Environment.GetEnvironmentVariable("DATABASE_URL"));
                string[] usernameandpass = tmp.UserInfo.Split(":");
                string ConnString = $"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}";
                Database.Setconnstring(ConnString);
            }
            GiphyO.Set(new GiphyDotNet.Manager.Giphy(readconfig.Gtoken));
            Fortnite.Setapi(new Fortnite_API.FortniteApi(readconfig.FApiToken));
            return readconfig;
        }

            catch (FileNotFoundException)
            {
                System.Environment.Exit(420);
                private Config asdf = new Config();
                return asdf;
            }
}
    }
}