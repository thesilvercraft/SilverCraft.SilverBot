using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using RestSharp.Serialization.Json;
using SilverBotDS.Attributes;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SilverBotDS.Commands
{
    [Attributes.Category("Stable diffusion")]
    [AiGenChannel(1051203568524341358)]
    public class StableDiff:BaseCommandModule
    {
        public HttpClient HttpClient { private get; set; }
        public Config Config { private get; set; }
        private string[] SafeModel = new[] { "sd-v1-4"};
        private string[] NotSafeModel = new[] { "HD-17" };

        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string prompt)
        {
            await TImagine(ctx,prompt:prompt);
        }
        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string prompt,int steps)
        {
            await TImagine(ctx, steps:steps, prompt:prompt);
        }
        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string model, string prompt)
        {
            await TImagine(ctx,model:model, prompt:prompt);
        }
        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string model, int seed, string prompt)
        {
            await TImagine(ctx,model:model, seed:seed, prompt:prompt);
        }
        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string model, string prompt,int steps)
        {
            await TImagine(ctx,model:model, steps:steps, prompt:prompt);
        }
        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string model, int seed, string prompt, int resolution)
        {
            await TImagine(ctx,model:model, seed:seed, prompt:prompt, resolution:resolution);
        }     
        [Command("imagine")]
        public async Task Imagine(CommandContext ctx, string model, int seed, string prompt, int steps, int resolution)
        {
            await TImagine(ctx, model: model, seed: seed, prompt: prompt, steps: steps, resolution:resolution);
        }
        [Command("imagine")]
        public async Task TImagine(CommandContext ctx, string model = "sd-v1-4", int? seed=null,string negative_prompt="",string prompt="space", int resolution=512, int steps = 25)
        {
            if (!ctx.Channel.IsNSFW && NotSafeModel.Contains(model))
            {
                //disappointment
                model = SafeModel[0];
            }
            input i = new()
            {
                prompt = prompt,
                use_stable_diffusion_model=model,
                negative_prompt = negative_prompt,
                seed = seed?? RandomGenerator.Next(0,Int32.MaxValue ),
                width = resolution.ToString(),
                height = resolution.ToString(),
                num_inference_steps=steps.ToString()
            };
            var sent = JsonSerializer.Serialize(i);
            Console.WriteLine(sent);
            var res = await HttpClient.PostAsync("http://localhost:9000/render", new StringContent(sent,Encoding.UTF8,"application/json"));
            var response = JsonSerializer.Deserialize<response>(await res.Content.ReadAsStringAsync());
            DiscordMessage og;
            if (string.IsNullOrWhiteSpace(response.stream))
            {
                og= await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder().WithTitle("bot broke try again?")
                        .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                        .WithColor(await ColorUtils.GetSingleAsync()).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
                return;
                
            }
            if (response.status == "Online")
            {
                og=await new DiscordMessageBuilder()
                    .WithEmbed(new DiscordEmbedBuilder().WithTitle("Rendering "+response.stream)
                        .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                        .WithColor(await ColorUtils.GetSingleAsync()).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
            }
            else
            {
                og= await new DiscordMessageBuilder()
                                    .WithEmbed(new DiscordEmbedBuilder().WithTitle("Added to generation queue "+response.stream)
                                        .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
                                        .WithColor(await ColorUtils.GetSingleAsync()).Build()).WithReply(ctx.Message.Id).SendAsync(ctx.Channel);
            }

          

            bool keeptrying = true;
            byte exc = 100;
            while (keeptrying)
            {
                try
                {
                    var res2 = await HttpClient.GetAsync("http://localhost:9000" + response.stream);
                    var rescont = await res2.Content.ReadAsStringAsync();
                    if (rescont.StartsWith("{\"step\":"))
                    {
                        if (rescont.Contains("}{"))
                        {
                            rescont = rescont[(rescont.LastIndexOf("}{", StringComparison.Ordinal) + 1)..];
                        }
                        var deserialized = JsonSerializer.Deserialize<partialresponse>(rescont);
                        await og.ModifyAsync(x =>
                        {
                            x.WithEmbed(new DiscordEmbedBuilder().WithTitle("Generating " + response.stream).WithDescription($"{deserialized.step} / {deserialized.total_steps} @ {deserialized.step_time}s per step")
                                .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png)));
                        });
                    }

                    if (rescont.StartsWith("{\"status\":\"failed\"") || rescont.StartsWith("{\"detail\":\"Request "))
                    {
                        keeptrying = false;
                    }

                    if (rescont.StartsWith("{\"status\":\"succeeded\""))
                    {
                        var deserialized = JsonSerializer.Deserialize<fullresponse>(rescont);
                        var start= deserialized.output[0].data.IndexOf("base64,", 0, StringComparison.Ordinal) + "base64,".Length;
                        byte[] bytes = Convert.FromBase64String(deserialized.output[0].data[start..]);
                        await og.ModifyAsync(x =>
                        {
                            x.AddFile("image.png",new MemoryStream(bytes))
                                .WithEmbed(new DiscordEmbedBuilder().WithTitle("Done with "+response.stream)
                                    .WithDescription($"Seed: `{i.seed}`")
                                    .WithFooter("Requested by " + ctx.User.Username, ctx.User.GetAvatarUrl(ImageFormat.Png))
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
    public class response
    {
        public string status { get; set; }
        public int queue { get; set; }
        public string stream { get; set; }
        public long task { get; set; }
    }
    public class partialresponse
    {
        public int step { get; set; }
        public double step_time { get; set; }
        public int total_steps { get; set; }
    }

    public class input
    {
        public string prompt { get; set; }
        public long session_id { get; set; } = 1670683499561;
        public int seed { get; set; }
        public string negative_prompt { get; set; }
        public int num_outputs { get; set; } = 1;
        public string num_inference_steps { get; set; } = "25";
        public string guidance_scale { get; set; } = "24.7";
        public string width { get; set; } = "1024";
        public string height { get; set; } = "1024";
        public bool turbo { get; set; } = true;
        public bool use_full_precision { get; set; } = false;
        public string use_stable_diffusion_model { get; set; } = "sd-v1-4";
        public string use_vae_model { get; set; } = "";
        public bool stream_progress_updates { get; set; } = false;
        public bool stream_image_progress { get; set; }= false;
        public bool show_only_filtered_image { get; set; } = true;
        public string output_format { get; set; } = "png";
        public string output_quality { get; set; } = "75";
        public string original_prompt { get; set; }
        public string sampler { get; set; } = "euler_a";
    }
    public class Request
    {
        public string session_id { get; set; }
        public string prompt { get; set; }
        public string negative_prompt { get; set; }
        public int num_outputs { get; set; }
        public int num_inference_steps { get; set; }
        public double guidance_scale { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int seed { get; set; }
        public double prompt_strength { get; set; }
        public string sampler { get; set; }
        public object use_face_correction { get; set; }
        public object use_upscale { get; set; }
        public string use_stable_diffusion_model { get; set; }
        public object use_vae_model { get; set; }
        public string output_format { get; set; }
        public int output_quality { get; set; }
    }

    public class Output
    {
        public string data { get; set; }
        public int seed { get; set; }
        public object path_abs { get; set; }
    }

    public class fullresponse
    {
        public string status { get; set; }
        public Request request { get; set; }
        public Output[] output { get; set; }
    }

}