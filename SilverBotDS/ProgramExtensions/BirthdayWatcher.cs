using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using SilverBot.Shared;

namespace SilverBotDS.ProgramExtensions
{
    /// <summary>
    /// The Simple Birthday Watcher™️ listens for any channel changes,
    /// if it hears anything related to a specified channel id it checks if that channel previously had been hidden/read only
    /// and if now it can send a message, if conditions are met it sends a message.
    /// </summary>
    [ExpectedContextType(typeof(DiscordClient))]
    public class BirthdayWatcher : IProgramExtension
    {
        public ulong ChannelId { get; set; } = 0;
        public string Content { get; set; } = "";
        public int DayOfMonth { get; set; } = 0;
        
        private bool _isLoaded;
        
        public Task Register(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (_isLoaded)
            {
                throw new ProgramExtensionAlreadyLoadedException();
            }

            if (additionalContext[0] is DiscordClient client)
            {
                client.ChannelUpdated += ClientOnChannelUpdatedAsync;
            }

            _isLoaded = true;
            return Task.CompletedTask;
        }

        private async Task ClientOnChannelUpdatedAsync(DiscordClient sender, ChannelUpdateEventArgs args)
        {
            if (args.ChannelAfter.Id == ChannelId &&
                (args.ChannelBefore.PermissionOverwrites.FirstOrDefault(x => x.Id == args.Guild.EveryoneRole.Id)
                    .Denied & (Permissions.SendMessages | Permissions.AccessChannels)) != 0 && args.ChannelAfter
                    .PermissionOverwrites.FirstOrDefault(x => x.Id == args.Guild.EveryoneRole.Id).Allowed
                    .HasFlag(Permissions.SendMessages) && DateTime.Now.Day==DayOfMonth)
            {
                await args.ChannelAfter.SendMessageAsync(Content);
            }
        }

        public Task Reload()
        {
            return Task.CompletedTask;
        }

        public Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext)
        {
            if (!_isLoaded)
            {
                throw new ProgramExtensionNotLoadedException();
            }

            if (additionalContext[0] is DiscordClient client)
            {
                client.ChannelUpdated -= ClientOnChannelUpdatedAsync;
            }

            _isLoaded = false;
            return Task.CompletedTask;
        }

        public bool IsLoaded => _isLoaded;
    }
}