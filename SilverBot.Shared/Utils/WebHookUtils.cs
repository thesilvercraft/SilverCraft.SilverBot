using System.Globalization;
using System.Text.RegularExpressions;

namespace SilverBotDS.Utils;

public static class WebHookUtils
{
    private static readonly Regex WebhookUrlRegex =
        new(@"^.*(discord|discordapp)\.com\/api\/webhooks\/([\d]+)\/([a-z0-9_-]+)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static void ParseWebhookUrlNullable(string webhookUrl, out ulong? webhookIdnullable, out string webhookToken)
    {
        webhookIdnullable = null;
        webhookToken = null;
        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            return;
        }
        var match = WebhookUrlRegex.Match(webhookUrl);
        if (match?.Groups[2].Success == true && ulong.TryParse(match.Groups[2].Value, NumberStyles.None,
                    CultureInfo.InvariantCulture, out var whid))
        {
            webhookIdnullable = whid;
            if (match.Groups[3].Success)
            {
                webhookToken = match.Groups[3].Value;
            }
        }
    }
}