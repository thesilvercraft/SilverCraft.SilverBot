using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LLama;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBot.Shared;

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
            Model = new LLamaModel(new LLamaParams(model: "/devmnt/e4nhdd/gpt/ggml-mpt-7b-chat.bin", n_ctx: 512, repeat_penalty: 1.0f));
        }

        public ulong[] WhitelistedChannels { get; set; } = new ulong[] { 1096077860319547594 };
        private async Task NewMessage(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (WhitelistedChannels.Contains(args.Channel.Id))
            {
                if (Chats.TryGetValue(args.Channel.Id, out var chat))
                {
                    var response = chat.Chat(args.Author.Username + " userid " + args.Author.Id + ": " + args.Message.Content + "\n").ToList();
                    if (response.Count != 0)
                    {
                        var combinedresponse = string.Join('\n',response);
                        const int maxMessageLength = 2000;
                        var messages = new List<string>();

                        if (combinedresponse.Length <= maxMessageLength)
                        {
                            messages.Add(combinedresponse);
                            
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            foreach (var word in combinedresponse.Split(' '))
                            {
                                if (sb.Length + word.Length + 1 > maxMessageLength)
                                {
                                    messages.Add(sb.ToString());
                                    sb.Clear();
                                }

                                sb.Append(word).Append(' ');
                            }

                            if (sb.Length > 0)
                            {
                                messages.Add(sb.ToString());
                            }
                        }

                        DiscordMessage prev = args.Message;
                        foreach (var message in messages)
                        {
                            prev = await args.Channel.SendMessageAsync(new DiscordMessageBuilder().WithReply(prev.Id)
                                .WithContent(message));
                        }
                        
                    }
                }
                else
                {
                    Chats[args.Channel.Id]=new ChatSession<LLamaModel>(Model);
                }
            }
        }

        private Dictionary<ulong, ChatSession<LLamaModel>> Chats=new();

        public async Task Reload()
        {
        }

        public async Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
        }
        

        public bool IsLoaded => _isLoaded;
    }
}