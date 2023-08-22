using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects.Database;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Language;
using SilverBot.Shared.Utils;

namespace SilverBotDS.Commands
{
    [SilverBot.Shared.Attributes.Category("Bing")]
    [RequireModuleGuildEnabled(EnabledModules.Bing, true)]
    public class BingCommandModule : BaseCommandModule, ISpecialModuleRegistration
    {
        public DatabaseContext Database { private get; set; }
        public LanguageService LanguageService { private get; set; }
        private static DiscordWebhookClient _webhookClient = new();

        [Command("addBingChannel")]
        [RequireUserPermissions(Permissions.ManageGuild)]
        [RequireGuild]
        public async Task AddBingChannel(CommandContext ctx, DiscordChannel bingchannel)
        {
            var lang = await LanguageService.FromCtxAsync(ctx);
            try
            {
                var webhook = await bingchannel.CreateWebhookAsync("silverbot",
                    reason: "addBingChannel executed by " + ctx.User.Id);
                Database.UpsertServerSettings(ctx.Guild.Id,
                    (settings, _) =>
                    {
                        settings.BingWebhooks.Add(new DatabaseWebhookData(webhook.Id, webhook.Token));
                    });
                await new DiscordMessageBuilder().WithReply(ctx.Message.Id)
                    .WithContent(string.Format(lang.AddedBingToChannel, bingchannel.Mention))
                    .WithAllowedMentions(Mentions.None)
                    .SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static HashSet<Tuple<ulong, ulong>> HandeledBings { get; set; } = new();
        public static bool Allow2PeopleToGetBing { get; set; } = false;
        public static bool AllowPastBings { get; set; } = false;
        public static async Task HandleButton(DiscordClient sender, ComponentInteractionCreateEventArgs args,
            ServiceProvider services)
        {
            var bing = new Tuple<ulong, ulong>(args.Channel.Id, args.Message.Id);
            if (args.Id.StartsWith("BING") && 
                (Allow2PeopleToGetBing ||!HandeledBings.Contains(bing)) 
                &&( AllowPastBings || (args.Message.Timestamp.Hour == DateTime.Now.Hour && args.Message.Timestamp.Date == DateTime.Now.Date)))
            {
                HandeledBings.Add(bing);
                await args.Interaction.DeferAsync();
                var databaseContext = services.GetRequiredService<DatabaseContext>();
                var x = databaseContext.UpsertIncrementUserBing(args.User.Id, args.Guild.Id);
                await args.Interaction.CreateFollowupMessageAsync(
                    new DiscordFollowupMessageBuilder()
                        .WithContent(string.Format(bings.GetAppropriate(x), args.User.Mention)));
                var webhook = _webhookClient.Webhooks.FirstOrDefault(y => y.ChannelId == args.Message.ChannelId);
                await webhook?.EditMessageAsync(args.Message.Id,
                    new DiscordWebhookBuilder().WithContent(args.Message.Content))!;
            }
        }
        private async Task AddWebHooksToClientAsync(DatabaseContext dbCtx)
        {
            var webhooks = dbCtx.serverSettings.SelectMany(x => x.BingWebhooks).ToList();
            webhooks = webhooks.Where(x => _webhookClient.Webhooks.All(y => y.Id != x.Id)).ToList();
            foreach (var webhook in webhooks)
            {
                await _webhookClient.AddWebhookAsync(webhook.Id, webhook.Token);
            }
        }
        public async Task Register(CommandsNextExtension commandsNextExtension)
        {
            var taskScheduler = commandsNextExtension.Services.GetRequiredService<ICallBack>() ;
            var dbCtx = commandsNextExtension.Services.GetRequiredService<DatabaseContext>();
            await AddWebHooksToClientAsync(dbCtx);
            taskScheduler.Add(async () =>
            {
                await AddWebHooksToClientAsync(dbCtx);
                _ = Task.Run(async () => await DoBong());
                return DateTime.Now.AddMinutes(60 - DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second);
            }, DateTime.Now.AddMinutes(60 - DateTime.Now.Minute).AddSeconds(-DateTime.Now.Second));
        }

        private static string _bongContent =
            "<:micorsoft_bing_0_0:779071679271010324><:micorsoft_bing_1_0:779071679125127190><:micorsoft_bing_2_0:779071680064126999><:micorsoft_bing_3_0:779071679598034945><:micorsoft_bing_4_0:779071680017858590><:micorsoft_bing_5_0:779071680383156264><:micorsoft_bing_6_0:779071679908937749><:micorsoft_bing_7_0:779071680131629086>" +
            Environment.NewLine +
            "<:micorsoft_bing_0_1:779071679019876393><:micorsoft_bing_1_1:779071679862538313><:micorsoft_bing_2_1:779071679879446628><:micorsoft_bing_3_1:779071679870795847><:micorsoft_bing_4_1:779071680114196480><:micorsoft_bing_5_1:779071680064258079><:micorsoft_bing_6_1:779071680277774336><:micorsoft_bing_7_1:779071680151683083>";

        private static BingsList bings = new();

        private static async Task DoBong()
        {
            await _webhookClient.BroadcastMessageAsync(new DiscordWebhookBuilder().WithContent(_bongContent)
                .AddComponents(new DiscordButtonComponent(ButtonStyle.Primary, "BING", null, false,
                    new DiscordComponentEmoji(783096213875851304))));
        }
    }

    public class BingsList
    {
        public string GetAppropriate(ulong NumberOfBings)
        {
            var list = Texts.Where(b =>
                (b.Day == DateTime.Today.Day || b.Day == null) &&
                (b.Month == DateTime.Today.Month || b.Month == null) &&
                (b.Year == DateTime.Today.Year || b.Year == null) && (b.Hour == DateTime.Now.Hour || b.Hour == null) &&
                (b.Minute == DateTime.Now.Minute || b.Minute == null) &&
                (b.NumberOfBingsOfUser == NumberOfBings || b.NumberOfBingsOfUser == null) &&
                (b.DayOfWeek == (int)DateTime.Today.DayOfWeek || b.DayOfWeek == null)).ToArray();
            return list[0].Text;
        }

        private List<BingText> Texts = new()
        {
            new BingText("{0} happy christmas", 25, 12),
            new BingText("{0} happy christmas", 7, 1),
            new BingText("{0} reacted to the Microsoft bing first 69 times. NOICE", numberOfBingsOfUser: 69),
            new BingText(
                "{0} reacted to the Microsoft bing first 42 times. Microsoft bing is truly the meaning of life the universe and everything",
                numberOfBingsOfUser: 42),
            new BingText("{0} reacted to the Microsoft bing first 420 times.", numberOfBingsOfUser: 420),
            new BingText("{0} reacted to the Microsoft bing first cheese times.", numberOfBingsOfUser: 1420),
            new BingText("{0} reacted to the Microsoft bing first 9+10 times", numberOfBingsOfUser: 21),
            new BingText("{0} reacted to the Microsoft bing first 13 times and its the friday of 13.", 13, dayOfWeek: 5,
                numberOfBingsOfUser: 13),
            new BingText("{0} got the mothers day bing (and International Women's Day)", 13, dayOfWeek: 5,
                numberOfBingsOfUser: 13),
            new BingText("{0} reacted to the Microsoft bing first 69420 times. This is truly an achivement.",
                numberOfBingsOfUser: 69420),
            new BingText("{0} reacted to the Microsoft bing first")
        };
    }

    public class BingText
    {
        public BingText(string text, int? day = null, int? month = null, int? year = null, int? hour = null,
            int? minute = null, int? dayOfWeek = null, ulong? numberOfBingsOfUser = null)
        {
            Text = text;
            Day = day;
            Month = month;
            Year = year;
            Hour = hour;
            Minute = minute;
            DayOfWeek = dayOfWeek;
            NumberOfBingsOfUser = numberOfBingsOfUser;
        }

        [JsonPropertyName("text")] public string Text { get; set; } = "{0} reacted to the Microsoft bing first.";
        [JsonPropertyName("day")] public int? Day { get; set; } = null;
        [JsonPropertyName("month")] public int? Month { get; set; } = null;
        [JsonPropertyName("year")] public int? Year { get; set; } = null;
        [JsonPropertyName("hour")] public int? Hour { get; set; } = null;
        [JsonPropertyName("minute")] public int? Minute { get; set; } = null;
        [JsonPropertyName("day_of_week")] public int? DayOfWeek { get; set; } = null;

        [JsonPropertyName("number_of_bings_of_user")]

        public ulong? NumberOfBingsOfUser { get; set; } = null;
    }
}