/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

//TODO
/*using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SilverBotDS.Converters;
using SilverBotDS.Exceptions;
using SilverBotDS.Objects;
using SilverBotDS.Utils;
using SixLabors.ImageSharp;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CategoryAttribute = SilverBotDS.Attributes.CategoryAttribute;

namespace SilverBotDS.Commands;

[Category("Emotes")]
public class Emotes : BaseCommandModule
{
    public DatabaseContext Database { private get; set; }
    public Config Config { private get; set; }
    public HttpClient HttpClient { private get; set; }

    [Command("addemote")]
    [Aliases("addemoji")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name,
        [Description("Url of the thing")] SdImage url)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        var bytes = await url.GetBytesAsync(HttpClient);
        var st = new MemoryStream(bytes);
        try
        {
            if (bytes.Length > 256000)
            {
                await ctx.RespondAsync(string.Format(lang.EmoteWasLargerThan256K, FileSizeUtils.FormatSize(st.Length)));
                st = (await ImageModule.ResizeAsync(bytes, new Size(128, 128))).Item1;
            }

            var emote = await ctx.Guild.CreateEmojiAsync(name, st,
                reason: "Added via silverbot by " + ctx.User.Username);

            await ctx.RespondAsync(emote.ToString());
        }
        finally
        {
            await st.DisposeAsync();
        }
    }

    [Command("addemote")]
    [RequireGuild]
    [RequirePermissions(Permissions.ManageEmojis)]
    public async Task AddEmote(CommandContext ctx, [Description("Name like `Kappa`")] string name)
    {
        var lang = await LanguageService.FromCtxAsync(ctx);
        try
        {
            var image = SdImage.FromContext(ctx);
            await AddEmote(ctx, name, image);
        }
        catch (AttachmentCountIncorrectException acie)
        {
            if (acie.AttachmentCount is AttachmentCountIncorrect.TooLittleAttachments)
            {
                await ctx.RespondAsync(lang.NoImageGeneric);
            }
            else if (acie.AttachmentCount is AttachmentCountIncorrect.TooManyAttachments)
            {
                await ctx.RespondAsync(lang.MoreThanOneImageGeneric);
            }
        }
    }
}*/