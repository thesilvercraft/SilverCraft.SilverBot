using DSharpPlus;
using DSharpPlus.Entities;
using SilverBotDS.PixelsArchiver.Objects;
using SilverBotDS.Utils;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SilverBotDS.PixelsArchiver
{
    public class PixelArchiverService : SilverBotDS.Objects.IService
    {
        private Timer? timer;
        private DiscordWebhookClient? webhookClient;
        public HttpClient? Client { set; private get; }
        
        public async Task Start()
        {
            PixelsArchiverConfig config = await PixelsArchiverConfig.GetAsync();
            if (config is null)
            {
                await Stop();
                return;
            }
            webhookClient = new();
            foreach (var aa in config.ArchiveWebhooks)
            {
                WebHookUtils.ParseWebhookUrlNullable(aa, out ulong? ida, out string tokena);
                if (ida is not null && !string.IsNullOrWhiteSpace(tokena))
                {
                    await webhookClient.AddWebhookAsync(ida.Value, tokena);
                }
            }
            timer = new(Tick, config, 0, 60000);
        }

        private async void Tick(object? gaming)
        {
            
            Dictionary<string, Stream> strm = new();
            foreach (var urls in ((PixelsArchiverConfig)gaming).ApisToArchivePicturesFrom)
            {   
                try
                {
                    using var request = new HttpRequestMessage(HttpMethod.Get, urls.Key);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", urls.Value);
                    var response = await Client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var stuffineed = JsonSerializer.Deserialize<Rootobject>(await response.Content.ReadAsStringAsync());
                        strm.Add(new Uri(urls.Key).DnsSafeHost + RandomGenerator.RandomAbcString(6) + ".png", new MemoryStream(Convert.FromBase64String(stuffineed.DataURL[22..])));
                    }
                }
                catch(Exception e)
                {
                    //the api's can get a bit quirky at night
                }
            }
            var zipFile = new MemoryStream();
            var zip = new ZipArchive(zipFile, ZipArchiveMode.Create, true);
            foreach (var (key, value) in strm)
            {
                var entry = zip.CreateEntry(key);
                var stream = entry.Open();
                value.Position = 0;
                await value.CopyToAsync(stream);
                stream.Close();
            }
            strm.Add("archive.zip", zipFile);
            var dwb = new DiscordWebhookBuilder();
            foreach (var file in strm)
            {
                file.Value.Position = 0;
                dwb.AddFile(file.Key, file.Value);
            }
            zipFile.Position = 0;
            await webhookClient.BroadcastMessageAsync(dwb);
            if (((PixelsArchiverConfig)gaming).SaveZip)
            {
                Directory.CreateDirectory("PixelsArchive/" + DateTime.Now.ToString("yyyy/MM/ddd/HH/mm/ss/"));
                using var file = File.OpenWrite("PixelsArchive/" + DateTime.Now.ToString("yyyy/MM/ddd/HH/mm/ss/") + "file.zip");
                zipFile.Position = 0;
                await zipFile.CopyToAsync(file);
            }
            await zipFile.DisposeAsync();
            foreach (var file in strm)
            {
                await file.Value.DisposeAsync();
            }
        }

        public Task Stop()
        {
            timer.Dispose();
            webhookClient = null;
            return Task.CompletedTask;
        }
    }

    public class Rootobject
    {
        public string Method { get; set; }
        public string DataURL { get; set; }
    }
}
