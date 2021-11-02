using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    public class RequireAttachmentAttribute : CheckBaseAttribute
    {
        public uint AttachmentCount { get; init; }

        public RequireAttachmentAttribute(uint attachmentcount = 1)
        {
            AttachmentCount = attachmentcount;
        }

        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            if (help)
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(ctx.Message.Attachments.Count == AttachmentCount);
        }
    }
}