using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
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

        public static Config Get()
        {
            Config new_config = new Config();
            XmlSerializer serializer = new XmlSerializer(new_config.GetType());
            try
            {
                using Stream stream = File.OpenRead("silverbot.xml");
                Config readconfig = serializer.Deserialize(stream) as Config;
                if (!string.IsNullOrEmpty(readconfig.ConnString))
                {
                    Database.setconnstring(readconfig.ConnString);
                }
                else
                {
                    Uri tmp = new Uri(Environment.GetEnvironmentVariable("DATABASE_URL"));
                    string[] usernameandpass = tmp.UserInfo.Split(":");
                    string ConnString = $"Host={tmp.Host};Username={usernameandpass[0]};Password={usernameandpass[1]};Database={HttpUtility.UrlDecode(tmp.AbsolutePath).Remove(0, 1)}";
                    Database.setconnstring(ConnString);
                }
                GiphyO.Set(new GiphyDotNet.Manager.Giphy(readconfig.Gtoken));
                return readconfig;
            }
            catch (FileNotFoundException e)
            {
                using (StreamWriter streamWriter = new StreamWriter("silverbot.xml"))
                {
                    System.Xml.XmlDocument xmlDocument = Xmlutils.SerializeToXmlDocument(new_config);
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
                    xmlDocument.Save(streamWriter);
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start("notepad.exe", "silverbot.xml");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.xml should have opened in notepad, edit it, save it and restart silverbot");
                    Console.WriteLine("Press any key to continue...");
                    Console.WriteLine(e);
                    Console.ReadKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("silverbot.xml should exist, edit it, save it and restart silverbot");
                    Console.WriteLine("Press any key to continue...");
                    Console.WriteLine(e);
                    Console.ReadKey();
                }
                System.Environment.Exit(420);
                Config asdf = new Config();
                return asdf;
            }
        }
    }
}