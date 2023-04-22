using System;
using System.Text.Json;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Humanizer;
using SilverBot.Shared;

namespace SilverBotDS
{
    public static class CodeEnvHelper
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        public static async Task SendBestRepresentationAsync(object ob, CommandContext ctx)
        {
            try
            {
                var str = ob.ToString();
                switch (ob)
                {
                    case TimeSpan span:
                        str = span.Humanize(20);
                        break;
                    case DateTime time:
                        str = Formatter.Timestamp(time);
                        break;
                    case string @string:
                        str = Formatter.BlockCode(@string);
                        break;
                    default:
                    {
                        if (ob.GetType().IsSerializable || ob.GetType().IsArray || ob.GetType().IsEnum ||
                            ob.GetType().FullName == ob.ToString())
                        {
                            str = JsonSerializer.Serialize(ob, Options);
                            if (str.Length >= 2000)
                            {
                                await ctx.SendStringFileWithContent(ob.GetType().FullName ?? "", str, "eval.txt");
                                return;
                            }

                            str = Formatter.BlockCode(str, "json");
                        }
                        else
                        {
                            str = Formatter.BlockCode(str);
                        }

                        break;
                    }
                }

                if (str.Length >= 2000)
                {
                    await ctx.SendStringFileWithContent(ob.GetType().FullName ?? "", str, "eval.txt");
                    return;
                }

                await new DiscordMessageBuilder().WithContent($"{ob.GetType().FullName} {str}")
                    .WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                await new DiscordMessageBuilder()
                    .WithContent(
                        $"Failed to cast `{ob.GetType().FullName}` to a string, using the generic ToString. {ob}")
                    .WithAllowedMentions(Mentions.None).SendAsync(ctx.Channel);
            }
        }

        public static string RemoveCodeBraces(string str)
        {
            if (str.StartsWith("```csharp"))
            {
                str = str.Remove(0, 9);
            }

            if (str.StartsWith("```cs"))
            {
                str = str.Remove(0, 5);
            }

            if (str.StartsWith("```js"))
            {
                str = str.Remove(0, 5);
            }

            if (str.StartsWith("```javascript"))
            {
                str = str.Remove(0, 14);
            }

            if (str.StartsWith("```"))
            {
                str = str.Remove(0, 3);
            }

            if (str.StartsWith("``"))
            {
                str = str.Remove(0, 2);
            }

            if (str.StartsWith("`"))
            {
                str = str.Remove(0, 1);
            }

            if (str.EndsWith("```"))
            {
                str = str.Remove(str.Length - 3, 3);
            }

            if (str.EndsWith("``"))
            {
                str = str.Remove(str.Length - 2, 2);
            }

            if (str.EndsWith("`"))
            {
                str = str.Remove(str.Length - 1, 1);
            }

            return str;
        }
    }
}