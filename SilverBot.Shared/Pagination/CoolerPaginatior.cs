using DSharpPlus;
using DSharpPlus.Entities;
using System.Security.Cryptography.X509Certificates;

namespace SilverBot.Shared.Pagination
{
    public class CoolerPaginatior:IPaginator
    {
        public CoolerPaginatior(DiscordClient client)
        {
            client.ComponentInteractionCreated += (async (sender, args) =>
            {
                if (args.Id.StartsWith("CIP"))
                {
                    if(Guid.TryParse(args.Id[4..], out var guid))
                    {
                        async Task DoLogic(bool next)
                        {
                            if (Paginations.TryGetValue(guid, out var pagination))
                            {
                                if (!pagination.Shared &&pagination.User.Id != args.User.Id)
                                {
                                    return;
                                }
                                var page = pagination.Pagination.GetPageAtId(pagination.CurrentPage);
                                if (page.PreviousId != null)
                                {
                                    var nextPage = pagination.Pagination.GetPageAtId(next? (int)page.NextId : (int)page.PreviousId);
                                    await args.Interaction.CreateResponseAsync(
                                        InteractionResponseType.UpdateMessage,
                                        new DiscordInteractionResponseBuilder()
                                            .WithContent(nextPage.Content.HasValue ? nextPage.Content.Value : null)
                                            .AddEmbed(nextPage.Embed.HasValue ? nextPage.Embed.Value : null).AddComponents(GetButtons(nextPage, pagination.Pagination, guid)));
                                    pagination.CurrentPage = next ? (int)page.NextId : (int)page.PreviousId;
                                }
                            }
                        }
                        switch (args.Id[3])
                        {
                            case 'B':
                                await DoLogic(false);
                                break;
                            case 'N':
                                await DoLogic(true);
                                break;
                            default:
                                return;
                        }
                    }
                }
                
            });
        }
        public Dictionary<Guid, PaginationRecord> Paginations = new();

        public IEnumerable<DiscordButtonComponent> GetButtons(ILazyPage page,IPagination pagination, Guid id)
        {
            if (page.PreviousId != null && page.PreviousId < pagination.AllowedRange.End.Value && page.PreviousId >= pagination.AllowedRange.Start.Value)
            {
                yield return new DiscordButtonComponent(ButtonStyle.Primary, "CIPB" + id.ToString("N"), null, false,
                    new DiscordComponentEmoji("◀️"));
            }
            if (page.NextId != null && page.NextId < pagination.AllowedRange.End.Value && page.NextId >= pagination.AllowedRange.Start.Value)
            {
                yield return new DiscordButtonComponent(ButtonStyle.Primary, "CIPN" + id.ToString("N"), null, false,
                    new DiscordComponentEmoji("▶️"));
            }
        }
        public async Task<DiscordMessage> SendPage(ILazyPage page, DiscordChannel channel, IPagination pagination, Guid id)
        {
            return await channel.SendMessageAsync(builder =>
            {
                if (page.Embed.HasValue)
                {
                    builder.WithEmbed(page.Embed.Value);
                }
                if (page.Content.HasValue)
                {
                    builder.WithContent(page.Content.Value);
                }
                builder.AddComponents(GetButtons(page, pagination, id));
            });
        }
        public async Task<Guid> SendPaginatedMessage(DiscordChannel channel, DiscordUser user,
            bool shared, IPagination pagination)
        {
            var guid = Guid.NewGuid();
            Paginations[guid] = new PaginationRecord(pagination,pagination.DefaultId, user, shared);
            var firstPage =  pagination.GetPageAtId(pagination.DefaultId);
            await SendPage(firstPage, channel, pagination, guid);
            return guid;
        }
    }
}