using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SBDSODC = SilverBotDS.Objects.Database.Classes;
namespace SilverBotDS.Commands
{
    [Group("quotes"), Aliases("quote")]
    public class UserQuotesModule : BaseCommandModule
    {
        public DatabaseContext dctx { private get; set; }
        public async Task PresentQuote(CommandContext ctx, SBDSODC.UserQuote quote, Language lang)
        {
            var b = new DiscordEmbedBuilder()
            {
                Description = quote.QuoteContent,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                  Text = string.Format(lang.QuotePreviewQuoteID,quote.QuoteId)
                },
                Timestamp=quote.TimeStamp
            };
            var msg=await new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()).AddComponents(new DiscordButtonComponent(ButtonStyle.Danger,"deletequote",null,false,new DiscordComponentEmoji("🗑"))).SendAsync(ctx.Channel);
            var response = await msg.WaitForButtonAsync(ctx.User, TimeSpan.FromSeconds(300));
            if(!response.TimedOut)
            {
                dctx.userQuotes.Remove(quote);
                await response.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder().WithContent(lang.QuotePreviewDeleteSuccess));
            }
            else
            {
                await msg.ModifyAsync(new DiscordMessageBuilder().WithReply(ctx.Message.Id).WithEmbed(b.Build()));
            }    
        }
        [Command("add")]
        [Description("Add a new quote to your \"Quote Book\"")]
        public async Task Add(CommandContext ctx, [RemainingText] string content)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            using RandomGenerator rg = new();
            var uq = dctx.userQuotes.FirstOrDefault(x => x.UserId == ctx.User.Id);
            var quote = new SBDSODC.UserQuote()
            {
                QuoteContent = content,
                QuoteId = rg.RandomAbcString(6),
                TimeStamp = DateTime.UtcNow,
                UserId = ctx.User.Id
            };
            await dctx.userQuotes.AddAsync(quote);
            await dctx.SaveChangesAsync();
            await PresentQuote(ctx, quote,lang);
        }
        [Command("get")]
        [Description("Gets a quote from your \"Quote Book\" with it's ID")]
        public async Task Get(CommandContext ctx, string id)
        {
            var lang = await Language.GetLanguageFromCtxAsync(ctx);
            using RandomGenerator rg = new();
            var uq = dctx.userQuotes.Any(x => x.UserId == ctx.User.Id);
            if (uq)
            {
                var q = dctx.userQuotes.FirstOrDefault(x => x.QuoteId == id && x.UserId == ctx.User.Id);
                if(q is not null)
                {
                    await PresentQuote(ctx, q, lang);
                }
                else
                {
                    await ctx.RespondAsync(lang.QuoteGetNoQuoteWithId);
                    return;
                }
            }
            else
            {
                await ctx.RespondAsync(lang.QuoteGetNoBook);
                return;
            }
        }
    }
}