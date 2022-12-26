/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
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
        if (typeof(Language)?.GetProperty(lessthen)?.CanRead==true)
        {
            LessThenLang = lessthen;
        }
        else
        {
            throw new InvalidOperationException(nameof(lessthen)+" cannot be read");
        }

        if (typeof(Language)?.GetProperty(morethen)?.CanRead==true)
        {
            MoreThenLang = morethen;
        }
        else
        {
            throw new InvalidOperationException(nameof(morethen)+" cannot be read");
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
public class AiGenChannelAttribute : CheckBaseAttribute
{
    public AiGenChannelAttribute(ulong id)
    {
        Id = id;
    }

    public ulong Id { get; init; }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return
            Task.FromResult(ctx.Channel.Id==Id);

    }
}