using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class splashes
    {
        private static DiscordActivity[] Internal { get; } = {
new ("D̶U̶K̶T̶  Silver Hosting", ActivityType.Watching),
new ("As seen on TV!", ActivityType.Playing),
new ("100% pure!", ActivityType.Playing),
new ("20 GOTO 10!", ActivityType.Playing),
new ("Déjà vu!",ActivityType.Playing),
new ("Déjà vu!",ActivityType.Playing),
new ("Does barrel rolls!",ActivityType.Playing),
new ("Follow the train, CJ!",ActivityType.Playing),
new ("NOT Google anlyticsed!",ActivityType.Playing),
new ("Beta!",ActivityType.Playing),
new ("Internet enabled!",ActivityType.Playing),
new ("David please look at your table",ActivityType.Playing),
new ("Its a bot!",ActivityType.Playing),
new ("Made in Macedonia",ActivityType.Playing),
new ("Now supports emoji :wink:",ActivityType.Playing),
new ("Creeper aw man.",ActivityType.Playing),
new ("if not ok then return end",ActivityType.Playing),
new ("Now on [insert platform here].",ActivityType.Playing),
new ("String[] splashes",ActivityType.Playing),
new ("Limited edition!",ActivityType.Playing),
new ("Should not be texted while driving",ActivityType.Playing),
new ("Try it!",ActivityType.Playing),
new ("Have you finished your homework?",ActivityType.Playing),
new ("UwU whats dis",ActivityType.Playing),
new ("Digital Dancing!",ActivityType.Playing),
new ("5 is best number!",ActivityType.Playing),
new ("MCROC++!",ActivityType.Playing),
new ("Plastic Guns!",ActivityType.Playing),
new ("Lines from 2004!",ActivityType.Playing),
new ("Absolutely no sarcasm!",ActivityType.Playing),
new ("Thinking involved!",ActivityType.Playing),
new ("huehuehue",ActivityType.Playing),
new ("!gnilleps sdrawkcab",ActivityType.Playing),
new ("xXPr0$n1p3$Xx",ActivityType.Playing),
new ("Sharpshooter!",ActivityType.Playing),
new ("Tsar Bomba!",ActivityType.Playing),
new ("Vive l'atome!",ActivityType.Playing),
new ("10% Milk!",ActivityType.Playing),
new ("Available in stores!",ActivityType.Playing),
new ("Gluten Free!",ActivityType.Playing),
new ("But wait there's more!",ActivityType.Playing),
new ("How much do you think SilverBot is worth. Think one minute before you anwser.",ActivityType.Playing),
new ("RREEEEE",ActivityType.Playing),
new ("except in nebraska",ActivityType.Playing),
new ("Above Average Man!",ActivityType.Playing),
new ("ThinThick is better than Swag",ActivityType.Playing),
new ("Glorified Drift Wood!",ActivityType.Playing),
new ("T0P K3K",ActivityType.Playing),
new ("Maximizing Profits!",ActivityType.Playing),
new ("Somewhat Persuasive!",ActivityType.Playing),
new ("am vary smert",ActivityType.Playing),
new ("Blue!",ActivityType.Playing),
new ("splashes.json",ActivityType.Playing),
new ("Green!",ActivityType.Playing),
new ("1nd Floor!",ActivityType.Playing),
new ("wait this isn't mee6",ActivityType.Playing),
new ("fReE cAkE>>",ActivityType.Playing),
new ("Exclamation Mark!",ActivityType.Playing),
new ("Strongly Dislikes Birch!",ActivityType.Playing),
new ("Bestest Grammar!",ActivityType.Playing),
new ("feedback plz",ActivityType.Playing),
new ("Relevant Text!",ActivityType.Playing),
new ("finally in beta",ActivityType.Playing),
new ("needs update",ActivityType.Playing),
new ("Woah!",ActivityType.Playing),
new ("fruit salad is best salad",ActivityType.Playing),
new ("75% less random!",ActivityType.Playing),
new ("Move now, think later!",ActivityType.Playing),
new ("*teleports behind you*",ActivityType.Playing),
new ("4 Letters!",ActivityType.Playing),
new ("Brandon yes",ActivityType.Playing),
new ("Show your face pixels!",ActivityType.Playing),
new ("Less than Minimum!",ActivityType.Playing),
new ("****need copy and link****",ActivityType.Playing),
new ("Flatter than Kansas!",ActivityType.Playing),
new ("Not a Fan of Fuzzies!",ActivityType.Playing),
new ("Consider Fullscreen!",ActivityType.Playing),
new ("The Exits are... Irrelevant!",ActivityType.Playing),
new ("The facts they dont want you to know",ActivityType.Playing),
new ("max should not be trusted",ActivityType.Playing),
new ("6.9' is noice",ActivityType.Playing),
new ("LE:L",ActivityType.Playing),
new ("requires additional pylons",ActivityType.Playing),
new ("THAT FACE DOWN",ActivityType.Playing),
new ("Heterogeneous Greens!",ActivityType.Playing),
new ("Old White French House!",ActivityType.Playing),
new ("Really Dark Purples!",ActivityType.Playing),
new ("Two Ups Thumb!",ActivityType.Playing),
new ("Gilden Ultra Cotton Pocket Tee!",ActivityType.Playing),
new ("Crazy Cool Coincidence!",ActivityType.Playing),
new ("Over the mountains, into Terrellia!",ActivityType.Playing),
new ("Your content is good!",ActivityType.Playing),
new ("striker disapproves",ActivityType.Playing),
new ("Fools' Gold!",ActivityType.Playing),
new ("Powered by Wind!",ActivityType.Playing),
new ("Globular Resources!",ActivityType.Playing),
new ("Falcon Heavy!",ActivityType.Playing),
new ("shoutout to me beta testers",ActivityType.Playing),
new ("Acid Trips!",ActivityType.Playing),
new ("lttstore.com",ActivityType.Playing),
new ("Wonder-filly-ful!",ActivityType.Playing),
new ("Nostalgic!",ActivityType.Playing),
new ("Giv quadratic formula or I kill",ActivityType.Playing),
new ("T-shape is cancer",ActivityType.Playing),
new ("som splashes for u ^_^",ActivityType.Playing),
new ("0% less memory!",ActivityType.Playing),
new ("ThePajamaSlime has invited you to play Minecraft",ActivityType.Playing),
new ("ThePajamaSlime! ThePajamaSlime! ThePajamaSlime!",ActivityType.Playing),
new ("Xbox 360 Edition",ActivityType.Playing),
new ("sign in with your microsoft account",ActivityType.Playing),
new ("Now with 69% more ThePajamaSlime!",ActivityType.Playing),
new ("Teach me the stats!",ActivityType.Playing),
new ("capped image of caps with captions in caps",ActivityType.Playing),
new ("buy shark tale on dvd",ActivityType.Playing),
new ("subscribe to idots",ActivityType.Playing),
new ("Cinnamon oat cereal!",ActivityType.Playing),
new ("Silver chest unlocked!",ActivityType.Playing),
new ("Flame-Headed Boy!",ActivityType.Playing),
new ("lamo",ActivityType.Playing),
new ("Non-Newtonian Fluids!",ActivityType.Playing),
new ("Intensive and Extensive!",ActivityType.Playing),
new ("Aquatic!",ActivityType.Playing),
new ("4TH DOWN AND 9!",ActivityType.Playing),
new ("pls r8 5",ActivityType.Playing),
new ("dabbing on them endermans",ActivityType.Playing),
new ("GIMP FTW",ActivityType.Playing),
new ("Bush hid the facts",ActivityType.Playing),
new ("ugh, finally, no more sugar",ActivityType.Playing),
new ("-1 splash per update?",ActivityType.Playing),
new ("uploading...41%",ActivityType.Playing),
new ("my stuff stuff",ActivityType.Playing),
new ("teleporting-porter-pearl",ActivityType.Playing),
new ("bot now has graphics, you're welcome",ActivityType.Playing),
new ("(it's a list, not a method)",ActivityType.Playing),
new ("superb bus service",ActivityType.Playing),
new ("SELCET? AWESMOE!",ActivityType.Playing),
new ("pretty pink spigots",ActivityType.Playing),
new ("you're uh winner",ActivityType.Playing),
new ("wifi killed the dinosaurs",ActivityType.Playing),
new ("60 layers of wax",ActivityType.Playing),
new ("12 years later",ActivityType.Playing),
new ("it came in teal!",ActivityType.Playing),
new ("pretend the sun is real",ActivityType.Playing),
new ("people hate people",ActivityType.Playing),
new ("north of what?",ActivityType.Playing),
new ("non-lethal stabbing",ActivityType.Playing),
new ("pertrify yourself!",ActivityType.Playing),
new ("pay the poor",ActivityType.Playing),
new ("check your spam!",ActivityType.Playing),
new ("say PLEASE next time!",ActivityType.Playing),
new ("see it with your ears!",ActivityType.Playing),
new ("hear it with your eyes!",ActivityType.Playing),
new ("it'll change, soon!",ActivityType.Playing),
new ("space is key!",ActivityType.Playing),
new ("for the love of god dont use dukt hosting",ActivityType.Playing),
new ("max is bad?",ActivityType.Playing),
new ("Smash Mouth - All Star",ActivityType.Playing),
new ("indentetion error",ActivityType.Watching),
new ("in 5,4,3,2,1 I know you see",ActivityType.Playing),
new ("the world will change for me",ActivityType.Playing),
new ("and be so wonderful",ActivityType.Playing),
new ("https://www.youtube.com/watch?v=HPM05dQgdig",ActivityType.Playing),
new ("RIP XBOX360",ActivityType.Playing),
new ("https://catjam.tk/",ActivityType.Playing),
new ("Leave it all to me",ActivityType.ListeningTo),
new ("Thanks ThePajamaSlime!",ActivityType.Playing),
new ("10000000% c# by 1969",ActivityType.Playing),
new ("Prouldly europian",ActivityType.Playing),
new ("@everyone",ActivityType.Playing),
new ("2 meters apart",ActivityType.Playing),
new ("ERROR",ActivityType.Playing),
new ("THE CAKE IS A LIE",ActivityType.Playing),
new ("ThE CaKe Is A lIe",ActivityType.Playing),
        };

        private static DiscordActivity[] Cache;

        public static DiscordActivity[] Get(bool ignorecache = false)
        {
            if (ignorecache || Cache == null)
            {
                if (File.Exists("splashes.json"))
                {
                    using StreamReader reader = new("splashes.json");
                    var arrays = JsonSerializer.Deserialize<DiscordActivity[]>(reader.ReadToEnd());
                    Cache = arrays;
                    return Cache;
                }
                else
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    using StreamWriter writer = new("splashes.json");
                    writer.Write(JsonSerializer.Serialize(Internal, options));
                    return Internal;
                }
            }
            else
            {
                return Cache;
            }
        }

        public static DiscordActivity GetSingle(bool ignorecache = false)
        {
            RandomGenerator rg = new();
            var arr = Get(ignorecache);
            return arr[rg.Next(0, arr.Length)];
        }
    }
}