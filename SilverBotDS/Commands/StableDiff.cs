/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Buffers.Text;
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
    [AiGenChannel(1055151462952480909)]
    public class StableDiff : BaseCommandModule
    {
        public HttpClient HttpClient { private get; set; }
        public Config Config { private get; set; }

        [Command("imagine")]
        public async Task TImagine(CommandContext ctx, string prompt = "space", string model = "sd-v1-4", int? seed = null, string negative_prompt = "", 
           int resolution = 512, int steps = 25)
        {
            if (!ctx.Channel.IsNSFW && (Config.ExtraParams["StableDiff.NotSafeModel"].Contains(model.ToLower()) ||
                                        Config.ExtraParams["StableDiff.NotSafeModel"].Contains(model.ToLower() + ".ckpt")))
            {
                //disappointment
                model = Config.ExtraParams["StableDiff.SafeModel"].Split(",").Select(x=>x.Trim()).ToArray()[0];
            }
            var language = await ctx.GetLanguageAsync();
            string promptimg = null;
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
                Prompt = prompt,
                UseStableDiffusionModel = model,
                NegativePrompt = negative_prompt,
                Seed = seed ?? RandomGenerator.Next(0, Int32.MaxValue),
                Width = resolution,
                Height = resolution,
                NumInferenceSteps = steps
            };
            if (promptimg != null && (promptimg.EndsWith(".png") || promptimg.EndsWith(".jpeg")|| promptimg.EndsWith(".jpg")))
            {
                try
                {
                    var promptImgResponseMessage = await HttpClient.GetAsync(promptimg);
                    var bytearray = await promptImgResponseMessage.Content.ReadAsByteArrayAsync();

                    bool Matches(byte?[] x)
                    {
                        return !x.Where((t, j) => t != null && t != bytearray[j]).Any();
                    }

                    byte?[] jpgs1 = new byte?[] { 0xFF, 0xD8, 0xFF, 0xDB };
                    byte?[] jpgs2 = new byte?[]
                        { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };
                    byte?[] jpgs3 = new byte?[] { 0xFF, 0xD8, 0xFF, 0xEE };
                    byte?[] jpgs4 = new byte?[]
                        { 0xFF, 0xD8, 0xFF, 0xE1, null, null, 0x45, 0x78, 0x69, 0x66, 0x00, 0x00 };
                    byte?[] png = new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };


                    if (Matches(jpgs1) || Matches(jpgs2) || Matches(jpgs3) || Matches(jpgs4))
                    {
                        i.InitImage = "data:image/jpeg;base64," + Convert.ToBase64String(bytearray);
                    }
                    else if (Matches(png))
                    {
                        i.InitImage = "data:image/png;base64," + Convert.ToBase64String(bytearray);
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
            var r = await interactivity.WaitForReactionAsync(
                (x) =>
                {
                    return x.Message == ctx.Message &&
                           (ctx.Client.CurrentApplication.Owners.Any(y => y.Id == x.User.Id) ||
                            Config.ExtraParams["StableDiff.Trusted"].Split(",").Select(z=> Convert.ToUInt64(z.Trim())).Contains(x.User.Id)) && x.Emoji.Name == "upvote";
                }, TimeSpan.FromMinutes(5));
            if (r.TimedOut)
            {
                return;
            }

            var res = await HttpClient.PostAsync( Config.ExtraParams["StableDiff.BaseUrl"]+"render",
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
                    var res2 = await HttpClient.GetAsync(Config.ExtraParams["StableDiff.BaseUrl"]+ response.Stream);
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
                        var deserialized = JsonSerializer.Deserialize<fullresponse>(rescont);
                        var start = deserialized.Output[0].Data.IndexOf("base64,", 0, StringComparison.Ordinal) +
                                    "base64,".Length;
                        var bytes = Convert.FromBase64String(deserialized.Output[0].Data[start..]);
                        await og.ModifyAsync(x =>
                        {
                            x.AddFile("image.png", new MemoryStream(bytes))
                                .WithEmbed(new DiscordEmbedBuilder().WithTitle("Done with " + response.Stream)
                                    .WithDescription($"Seed: `{i.Seed}`")
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
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("queue")]
        public int Queue { get; set; }
        [JsonPropertyName("stream")]
        public string Stream { get; set; }
        [JsonPropertyName("task")]
        public long Task { get; set; }
    }


    public class PartialResponse
    {
        [JsonPropertyName("step")]
        public int Step { get; set; }
        [JsonPropertyName("step_time")]
        public double StepTime { get; set; }
        [JsonPropertyName("total_steps")]
        public int TotalSteps { get; set; }
    }

    public class Input
    {
        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; } = "1670683499561";
        [JsonPropertyName("init_image")]
        public string InitImage { get; set; } = null;
        [JsonPropertyName("seed")]
        public int Seed { get; set; }
        [JsonPropertyName("negative_prompt")]
        public string NegativePrompt { get; set; }
        [JsonPropertyName("num_outputs")]
        public int NumOutputs { get; set; } = 1;
        [JsonPropertyName("num_inference_steps")]
        public long NumInferenceSteps { get; set; } = 25;
        [JsonPropertyName("guidance_scale")]
        public float GuidanceScale { get; set; } = 12.5f;
        [JsonPropertyName("prompt_strength")]
        public float PromptStrength { get; set; } = 0.34f;
        [JsonPropertyName("width")]
        public int Width { get; set; } = 1024;
        [JsonPropertyName("height")]
        public int Height { get; set; } = 1024;
        [JsonPropertyName("turbo")]
        public bool Turbo { get; set; } = true;
        [JsonPropertyName("use_full_precision")]
        public bool UseFullPrecision { get; set; } = false;
        [JsonPropertyName("use_stable_diffusion_model")]
        public string UseStableDiffusionModel { get; set; } = "sd-v1-4";
        [JsonPropertyName("use_vae_model")]
        public string UseVaeModel { get; set; } = "";
        [JsonPropertyName("stream_progress_updates")]
        public bool StreamProgressUpdates { get; set; } = false;
        [JsonPropertyName("stream_image_progress")]
        public bool StreamImageProgress { get; set; } = false;
        [JsonPropertyName("show_only_filtered_image")]
        public bool ShowOnlyFilteredImage { get; set; } = true;
        [JsonPropertyName("output_format")]
        public string OutputFormat { get; set; } = "png";
        [JsonPropertyName("output_quality")]
        public int OutputQuality { get; set; } = 75;
        [JsonPropertyName("original_prompt")]
        public string OriginalPrompt { get; set; }
        [JsonPropertyName("sampler")]
        public string Sampler { get; set; } = "euler_a";

    }


    public class Output
    {
        [JsonPropertyName("data")]

        public string Data { get; set; }
        [JsonPropertyName("seed")]

        public int Seed { get; set; }
        [JsonPropertyName("path_abs")]

        public object PathAbs { get; set; }
    }

    public class fullresponse
    {
        [JsonPropertyName("status")]

        public string Status { get; set; }
        [JsonPropertyName("request")]

        public Input Request { get; set; }
        [JsonPropertyName("output")]

        public Output[] Output { get; set; }
    }
}