using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBot.Shared;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Utils;

namespace SilverBotDS.ProgramExtensions
{
    /// <summary>
    /// The archiver archives messages from one channel to another using webhooks
    /// </summary>
    [ExpectedContextType(typeof(DiscordClient))]
    public  class Archiver : IProgramExtension
    {
        private bool _isLoaded;
        DiscordWebhookClient webhookClient = new();
        private Config config;
        private HttpClient httpClient;


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
        public async Task Register(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (additionalContext[0] is not DiscordClient client)
            {
                throw new ArgumentException(
                    "The additionalContext array must contain one element and that element must be the SlashCommandsExtension",
                    nameof(additionalContext));
            }
            ServiceProvider = sp;
            config = ServiceProvider.GetRequiredService<Config>();
            httpClient = ServiceProvider.GetRequiredService<HttpClient>();
            foreach (var aa in config.ArchiveWebhooks)
            {
                WebHookUtils.ParseWebhookUrlNullable(aa, out var ida, out var token);
                if (ida is not null && !string.IsNullOrWhiteSpace(token))
                {
                    await webhookClient.AddWebhookAsync(ida.Value, token);
                }
            }
            log.Information("Archive webhooks configured");


            client.MessageCreated += OnMessageCreated;
            client.MessageUpdated += OnMessageUpdated;
        }

        private Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs args) => OnMessage(args.Message);
        private Task OnMessageUpdated(DiscordClient sender, MessageUpdateEventArgs args) => OnMessage(args.Message);

        private  ServiceProvider ServiceProvider { get; set; }

        public Task Reload()
        {
            return Task.CompletedTask;
        }

        public Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (additionalContext[0] is not DiscordClient client)
            {
                throw new ArgumentException(
                    "The additionalContext array must contain one element and that element must be the SlashCommandsExtension",
                    nameof(additionalContext));
            }

            client.MessageCreated -= OnMessageCreated;
            client.MessageUpdated -= OnMessageUpdated;
            return Task.CompletedTask;
        }

        public bool IsLoaded => _isLoaded;
    }
}