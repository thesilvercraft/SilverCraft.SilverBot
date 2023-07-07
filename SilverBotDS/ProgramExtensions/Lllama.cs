using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Jellyfin.Sdk;
using LLama;
using LLama.Common;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBot.Shared;
using SilverBot.Shared.Utils;

namespace SilverBotDS.ProgramExtensions
{
    public class Lllama : IProgramExtension
    {
        private bool _isLoaded;
        private LLamaModel Model;
        public async Task Register(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (_isLoaded)
            {
                throw new ProgramExtensionAlreadyLoadedException();
            }
            if (additionalContext[0] is DiscordClient client)
            {
                client.MessageCreated += NewMessage;
            }
            Model = new LLamaModel(new ModelParams("/devmnt/e4nhdd/gpt/ggml-model-f32-q5_1.bin", contextSize: 1024, seed: 1337, gpuLayerCount: 5));
        }

        public ulong[] WhitelistedChannels { get; set; } = new ulong[] { 1119373378688655420 };
        private async Task NewMessage(DiscordClient sender, MessageCreateEventArgs args)
        {
            if ( args.Author!=sender.CurrentUser &&  WhitelistedChannels.Contains(args.Channel.Id) )
            {
                if (Chats.TryGetValue(args.Channel.Id, out var chat))
                {
                    var sb = new StringBuilder();
                    const int maxMessageLength = 500;
                    DiscordMessage prev = args.Message;

                    async Task Send()
                    {
                        var x = sb.ToString();
                        if (string.IsNullOrWhiteSpace(x))
                        {
                            return;
                        }
                        foreach (var part in x.SplitInParts(1987))
                        {
                            prev = await args.Channel.SendMessageAsync(new DiscordMessageBuilder().WithReply(prev.Id)
                                .WithContent(part));
                        }
                        sb.Clear();
                    }

                    var prompt = args.Author.Username + ": " + args.Message.Content +
                                 "\r\n";
                    Console.WriteLine(prompt);
                    foreach (var response in chat.Chat(prompt, new InferenceParams(){Temperature = 0.6f, RepeatPenalty =1.16f, AntiPrompts = new[]{args.Author.Username + ":"}}))
                    {
                        sb.Append(response);
                        Console.Write(response);
                        if (sb.Length + 10 > maxMessageLength)
                        {
                            await Send();
                            break;
                        }
                    }
                    if (sb.Length > 0)
                    {
                        await Send();
                    }
                  
                }
                else
                {
                    var ex = new InteractiveExecutor(Model);
                    Chats[args.Channel.Id]=new ChatSession(ex);
                    foreach (var a in Chats[args.Channel.Id].Chat(
                                 "Transcript of a conversation, where silverdiamond and his friends interact with an Assistant named silverbot. silverbot is good at writing, and never fails to answer the Users requests immediately and with precision, uses the markdown syntax, **thing** for bold, *thing* for italic, ***thing*** for bold and italic, ```code``` for a code block.\r\n\r\nsilverdiamond: how are you doing\r\n silverbot: still alive\r\nsilverdiamond: :3\r\n silverbot: :3\r\nsilverdiamond: UwU\r\n silverbot: UwU\r\n",  
                                 new InferenceParams(){Temperature = 0.6f, RepeatPenalty =1.16f, AntiPrompts = new List<string>{"silverdiamond:"}}).Take(10))
                    {
                        Console.Write(a);
                    }
                }
            }
        }

        public Dictionary<ulong, ChatSession> Chats=new();
        
        public async Task Reload()
        {
        }

        public async Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
        }
        

        public bool IsLoaded => _isLoaded;
    }
}