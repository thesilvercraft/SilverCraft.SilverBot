using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using Serilog.Core;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Utils;

namespace SilverBotDS.ProgramExtensions
{
    public static class Archiver
    {
        public static async Task AddArchiverAsync(this ProgramHelper _, Config config, Logger log,
            HttpClient httpClient, DiscordClient discord)
        {
            DiscordWebhookClient webhookClient = new();
            foreach (var aa in config.ArchiveWebhooks)
            {
                WebHookUtils.ParseWebhookUrlNullable(aa, out var ida, out var token);
                if (ida is not null && !string.IsNullOrWhiteSpace(token))
                {
                    await webhookClient.AddWebhookAsync(ida.Value, token);
                }
            }

            log.Information("Archive webhooks configured");

            async Task OnMessage(DiscordMessage a)
            {
                if (config.ChannelsToArchivePicturesFrom.Contains(a.Channel.Id))
                {
                    Dictionary<string, Stream> files = new();
                    foreach (var attachment in a.Attachments)
                    {
                        await httpClient.GetAsync(attachment.Url)
                            .ContinueWith(async (t) =>
                            {
                                if (t.IsCompletedSuccessfully)
                                {
                                    files.Add(attachment.FileName, await t.Result.Content.ReadAsStreamAsync());
                                }
                            });
                    }

                    if (a.Embeds.Count > 0)
                    {
                        foreach (var embed in a.Embeds)
                        {
                            if (embed.Image != null)
                            {
                                await httpClient.GetAsync(embed.Image.Url.ToUri())
                                    .ContinueWith(async (t) =>
                                    {
                                        if (t.IsCompletedSuccessfully)
                                        {
                                            files.Add(Path.GetFileName(embed.Image.Url.ToUri().AbsolutePath),
                                                await t.Result.Content.ReadAsStreamAsync());
                                        }
                                    });
                            }

                            if (embed.Thumbnail != null)
                            {
                                await httpClient.GetAsync(embed.Thumbnail.Url.ToUri())
                                    .ContinueWith(async (t) =>
                                    {
                                        if (t.IsCompletedSuccessfully)
                                        {
                                            files.Add(Path.GetFileName(embed.Thumbnail.Url.ToUri().AbsolutePath),
                                                await t.Result.Content.ReadAsStreamAsync());
                                        }
                                    });
                            }
                        }
                    }

                    if (a.Content.Length > 0)
                    {
                        files.Add("message.txt", new MemoryStream(Encoding.UTF8.GetBytes(a.Content)));
                    }

                    files.Add("message.json",
                        new MemoryStream(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(a))));
                    var zipFile = new MemoryStream();
                    var zip = new ZipArchive(zipFile, ZipArchiveMode.Create, true);
                    foreach (var (key, value) in files)
                    {
                        var entry = zip.CreateEntry(key);
                        var stream = entry.Open();
                        value.Position = 0;
                        await value.CopyToAsync(stream);
                        stream.Close();
                    }

                    var dwb = new DiscordWebhookBuilder();
                    foreach (var file in files)
                    {
                        file.Value.Position = 0;
                        dwb.AddFile(file.Key, file.Value);
                    }

                    zipFile.Position = 0;
                    dwb.AddFile("message.zip", zipFile);
                    dwb.AddEmbeds(a.Embeds);
                    dwb.WithContent(a.Content);
                    await webhookClient.BroadcastMessageAsync(dwb);
                    await zipFile.DisposeAsync();
                    foreach (var file in files)
                    {
                        await file.Value.DisposeAsync();
                    }
                }
            }

            discord.MessageCreated += (e, a) => OnMessage(a.Message);
            discord.MessageUpdated += (e, a) => OnMessage(a.Message);
        }
    }
}