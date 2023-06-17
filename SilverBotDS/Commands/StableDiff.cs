/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Buffers.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using RestSharp;
using SilverBot.Shared;
using SilverBot.Shared.Attributes;
using SilverBot.Shared.Objects;
using SilverBot.Shared.Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SilverBotDS.Commands
{
    [SilverBot.Shared.Attributes.Category("Stable diffusion")]
    [AiGenChannel(1119373378688655420)]
    public class StableDiff : BaseCommandModule
    {
        public HttpClient HttpClient { private get; set; }
        public Config Config { private get; set; }


        [Command("imagine")]
        public async Task TImagine(CommandContext ctx, string prompt = "space", string model = "sd-v1-4",
            int? seed = null, string negative_prompt = "",
            int resolution = 512, int steps = 25)
        {
            if (!ctx.Channel.IsNSFW && (Config.ExtraParams["StableDiff.NotSafeModel"].Contains(model.ToLower()) ||
                                        Config.ExtraParams["StableDiff.NotSafeModel"]
                                            .Contains(model.ToLower() + ".ckpt")))
            {
                //disappointment
                model = Config.ExtraParams["StableDiff.SafeModel"].Split(",").Select(x => x.Trim()).ToArray()[0];
            }

            var language = await ctx.GetLanguageAsync();
            string? promptimg = null;
            if (ctx.Message.Attachments.Count == 1)
            {
                promptimg = ctx.Message.Attachments[0].Url;
            }
            else if (ctx.Message.ReferencedMessage != null && ctx.Message.ReferencedMessage.Attachments.Count == 1)
            {
                promptimg = ctx.Message.ReferencedMessage.Attachments[0].Url;
            }

            Input i = new()
            {
                prompt = prompt,
                negative_prompt = negative_prompt,
                use_stable_diffusion_model = model,
                seed = seed ?? RandomGenerator.Next(0, Int32.MaxValue),
                width = resolution,
                height = resolution,
                num_inference_steps = steps
            };
            if (promptimg != null &&
                (promptimg.EndsWith(".png") || promptimg.EndsWith(".jpeg") || promptimg.EndsWith(".jpg")))
            {
                try
                {
                    var promptImgResponseMessage = await HttpClient.GetAsync(promptimg);
                    var bytearray = await promptImgResponseMessage.Content.ReadAsByteArrayAsync();

                    bool Matches(byte?[] x)
                    {
                        return !x.Where((t, j) => t != null && t != bytearray[j]).Any();
                    }

                    var jpgs1 = new byte?[] { 0xFF, 0xD8, 0xFF, 0xDB };
                    var jpgs2 = new byte?[]
                        { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };
                    var jpgs3 = new byte?[] { 0xFF, 0xD8, 0xFF, 0xEE };
                    var jpgs4 = new byte?[]
                        { 0xFF, 0xD8, 0xFF, 0xE1, null, null, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00 };
                    var png = new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };


                    if (Matches(jpgs1) || Matches(jpgs2) || Matches(jpgs3) || Matches(jpgs4))
                    {
                        i.init_image = "data:image/jpeg;base64," + Convert.ToBase64String(bytearray);
                    }
                    else if (Matches(png))
                    {
                        i.init_image = "data:image/png;base64," + Convert.ToBase64String(bytearray);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            var sent = JsonSerializer.Serialize(i);
            Console.WriteLine(sent);
            var interactivity = ctx.Client.GetInteractivity();
            if (!(ctx.Client.CurrentApplication.Owners.Any(y => y.Id == ctx.User.Id) ||
                  Config.ExtraParams["StableDiff.Trusted"].Split(",").Select(z => Convert.ToUInt64(z.Trim()))
                      .Contains(ctx.User.Id)))
            {
                var r = await interactivity.WaitForReactionAsync(
                    (x) =>
                    {
                        return x.Message == ctx.Message &&
                               (ctx.Client.CurrentApplication.Owners.Any(y => y.Id == x.User.Id) ||
                                Config.ExtraParams["StableDiff.Trusted"].Split(",")
                                    .Select(z => Convert.ToUInt64(z.Trim())).Contains(x.User.Id)) &&
                               x.Emoji.Name == "upvote";
                    }, TimeSpan.FromMinutes(5));
                if (r.TimedOut)
                {
                    return;
                }
            }

            var res = await HttpClient.PostAsync(Config.ExtraParams["StableDiff.BaseUrl"] + "/render",
                new StringContent(sent, Encoding.UTF8, "application/json"));
            var response = JsonSerializer.Deserialize<Response>(await res.Content.ReadAsStringAsync());
            DiscordMessage og;
            if (string.IsNullOrWhiteSpace(response.Stream))
            {
                await Task.Delay(100);
                await TImagine(ctx, prompt, model, seed, negative_prompt, resolution, steps);
                return;
            }

            if (response.Status == "Online")
            {
                og = await new DiscordMessageBuilder()
                    .WithEmbed(ctx.GetNewBuilder(language).WithTitle("Rendering " + response.Stream)
                        .Build()).WithReply(ctx.Message.Id)
                    .SendAsync(ctx.Channel);
            }
            else
            {
                og = await new DiscordMessageBuilder()
                    .WithEmbed(ctx.GetNewBuilder(language).WithTitle("Added to generation queue " + response.Stream)
                        .Build()).WithReply(ctx.Message.Id)
                    .SendAsync(ctx.Channel);
            }

            var keeptrying = true;
            byte exc = 100;
            while (keeptrying)
            {
                try
                {
                    var res2 = await HttpClient.GetAsync(Config.ExtraParams["StableDiff.BaseUrl"] + response.Stream);
                    var rescont = await res2.Content.ReadAsStringAsync();
                    if (rescont.StartsWith("{\"step\":"))
                    {
                        if (rescont.Contains("}{"))
                        {
                            rescont = rescont[(rescont.LastIndexOf("}{", StringComparison.Ordinal) + 1)..];
                        }

                        var deserialized = JsonSerializer.Deserialize<PartialResponse>(rescont);
                        await og.ModifyAsync(x =>
                        {
                            x.WithEmbed(ctx.GetNewBuilder(language).WithTitle("Generating " + response.Stream)
                                .WithDescription(
                                    $"{deserialized.Step} / {deserialized.TotalSteps} @ {deserialized.StepTime}s per step")
                            );
                        });
                    }

                    if (rescont.StartsWith("{\"status\":\"failed\"") || rescont.StartsWith("{\"detail\":\"Request "))
                    {
                        keeptrying = false;
                    }

                    if (rescont.StartsWith("{\"status\":\"succeeded\""))
                    {
                        var deserialized = JsonSerializer.Deserialize<Fullresponse>(rescont);
                        var start = deserialized.Output[0].Data.IndexOf("base64,", 0, StringComparison.Ordinal) +
                                    "base64,".Length;
                        var bytes = Convert.FromBase64String(deserialized.Output[0].Data[start..]);
                        await og.ModifyAsync(x =>
                        {
                            x.AddFile("SPOILER_image.png", new MemoryStream(bytes))
                                .WithEmbed(new DiscordEmbedBuilder().WithTitle("Done with " + response.Stream)
                                    .WithDescription($"Seed: `{i.seed}`")
                                    .WithFooter("Requested by " + ctx.User.Username,
                                        ctx.User.GetAvatarUrl(ImageFormat.Png))
                                    .Build()).WithReply(ctx.Message.Id);
                        });
                        keeptrying = false;
                    }

                    Console.WriteLine(rescont);
                    exc = 3;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    if (exc > 0)
                    {
                        exc--;
                    }
                    else
                    {
                        keeptrying = false;
                    }
                }

                await Task.Delay(1000);
            }
        }
    }

    public class Response
    {
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("queue")] public int Queue { get; set; }
        [JsonPropertyName("stream")] public string Stream { get; set; }
        [JsonPropertyName("task")] public long Task { get; set; }
    }


    public class PartialResponse
    {
        [JsonPropertyName("step")] public int Step { get; set; }
        [JsonPropertyName("step_time")] public double StepTime { get; set; }
        [JsonPropertyName("total_steps")] public int TotalSteps { get; set; }
    }

    public class Input
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string init_image { get; set; }

        public string prompt { get; set; }
        public int seed { get; set; }
        public bool used_random_seed { get; set; }
        public string negative_prompt { get; set; }
        public int num_outputs { get; set; } = 1;
        public int num_inference_steps { get; set; } = 25;
        public double guidance_scale { get; set; } = 7.5;
        public int width { get; set; }
        public int height { get; set; }
        public string vram_usage_level { get; set; } = "balanced";
        public string use_stable_diffusion_model { get; set; }
        public string use_vae_model { get; set; }
        public bool stream_progress_updates { get; set; }
        public bool stream_image_progress { get; set; }
        public bool show_only_filtered_image { get; set; } = true;
        public bool block_nsfw { get; set; }
        public string output_format { get; set; } = "png";
        public int output_quality { get; set; }
        public string metadata_output_format { get; set; } = "none";
        public string original_prompt { get; set; }
        public object[] active_tags { get; set; } = new Object[] { };
        public object[] inactive_tags { get; set; } = new Object[] { };
        public string sampler_name { get; set; } = "euler_a";
        public string session_id { get; set; } = "1678465790977";
    }


    public class Output
    {
        [JsonPropertyName("data")] public string Data { get; set; }
        [JsonPropertyName("seed")] public int Seed { get; set; }
        [JsonPropertyName("path_abs")] public object PathAbs { get; set; }
    }

    public class Fullresponse
    {
        [JsonPropertyName("status")] public string Status { get; set; }
        [JsonPropertyName("request")] public Input Request { get; set; }
        [JsonPropertyName("output")] public Output[] Output { get; set; }
    }
}