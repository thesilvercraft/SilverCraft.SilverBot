using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Attributes
{
    public class RequireAttachmentAttribute : CheckBaseAttribute
    {
        public uint AttachmentCount { get; init; }
        public string LessThenLang { get; set; }
        public string MoreThenLang { get; set; }

        public RequireAttachmentAttribute(uint attachmentcount = 1, string lessthen = "NoImageGeneric", string morethen = "MoreThanOneImageGeneric")
        {
            AttachmentCount = attachmentcount;
            if (typeof(Language).GetProperty(lessthen).CanRead)
            {
                LessThenLang = lessthen;
            }
            else
            {
                throw new InvalidOperationException("LessThen must be readable");
            }
            if (typeof(Language).GetProperty(morethen).CanRead)
            {
                MoreThenLang = morethen;
            }
            else
            {
                throw new InvalidOperationException("MoreThen must be readable");
            }
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