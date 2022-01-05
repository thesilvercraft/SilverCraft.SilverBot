using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Objects;

namespace SilverBotDS.Attributes;

public class RequireAttachmentAttribute : CheckBaseAttribute
{
    public RequireAttachmentAttribute(uint attachmentcount = 1, string lessthen = "NoImageGeneric",
        string morethen = "MoreThanOneImageGeneric", int argumentCountThatOverloadsCheck = -1)
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

        OverloadCount = argumentCountThatOverloadsCheck;
    }

    public uint AttachmentCount { get; init; }
    public string LessThenLang { get; set; }
    public string MoreThenLang { get; set; }
    public int OverloadCount { get; set; }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return help || OverloadCount == ctx.RawArgumentString.Split(" ").Length
            ? Task.FromResult(true)
            : Task.FromResult(ctx.Message.Attachments.Count == AttachmentCount);
    }
}