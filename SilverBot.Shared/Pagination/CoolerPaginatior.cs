using DSharpPlus;
using DSharpPlus.Entities;

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
                    var guid = Guid.Parse(args.Id[4..]);
                    switch (args.Id[3])
                    {
                        
                        case 'B':
                            if (Paginations.TryGetValue(guid, out var v1))
                            {
                                var page = v1.Pagination.GetPageAtId(v1.CurrentPage);
                                if (page.PreviousId != null)
                                {
                                    var prevPage = v1.Pagination.GetPageAtId((int)page.PreviousId);
                                    await args.Interaction.CreateResponseAsync(
                                        InteractionResponseType.UpdateMessage,
                                        new DiscordInteractionResponseBuilder()
                                            .WithContent(prevPage.Content.HasValue ? prevPage.Content.Value : null)
                                            .AddEmbed(prevPage.Embed.HasValue ? prevPage.Embed.Value : null).AddComponents(GetButtons(prevPage, v1.Pagination, guid)));
                                    v1.CurrentPage = (int)page.PreviousId;
                                }
                            }
                            break;
                        case 'N':
                            if (Paginations.TryGetValue(guid, out var v2))
                            {
                                var page = v2.Pagination.GetPageAtId(v2.CurrentPage);
                                if (page.NextId != null)
                                {
                                    var nextPage = v2.Pagination.GetPageAtId((int)page.NextId);
                                    await args.Interaction.CreateResponseAsync(
                                        InteractionResponseType.UpdateMessage,
                                        new DiscordInteractionResponseBuilder()
                                            .WithContent(nextPage.Content.HasValue ? nextPage.Content.Value : null)
                                            .AddEmbed(nextPage.Embed.HasValue ? nextPage.Embed.Value : null).AddComponents(GetButtons(nextPage, v2.Pagination, guid)));
                                    v2.CurrentPage = (int)page.NextId;
                                }
                            }
                            break;
                    }
                }
                
            });
        }
        public Dictionary<Guid, PaginationRecord> Paginations = new();

        public IEnumerable<DiscordButtonComponent> GetButtons(ILazyPage page,IPagination pagination, Guid id)
        {
            if (page.PreviousId != null && page.PreviousId < pagination.AllowedRange.End.Value && page.PreviousId >= pagination.AllowedRange.Start.Value)
            {
                yield return new DiscordButtonComponent(ButtonStyle.Primary, "CIPB" + id.ToString("N"), null,false,
                    new DiscordComponentEmoji("◀️"));
            }
            if (page.NextId != null && page.NextId < pagination.AllowedRange.End.Value && page.NextId >= pagination.AllowedRange.Start.Value)
            {
                yield return new DiscordButtonComponent(ButtonStyle.Primary, "CIPN" + id.ToString("N"), null,false,
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
            bool allowOtherUsersToStartTheirOwn, IPagination pagination)
        {
            var guid = Guid.NewGuid();
            Paginations[guid] = new PaginationRecord(pagination,pagination.DefaultId);
            var firstPage =  pagination.GetPageAtId(pagination.DefaultId);
            var message = await SendPage(firstPage, channel, pagination, guid);
            return guid;
        }
    }
}