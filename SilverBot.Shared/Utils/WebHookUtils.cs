/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
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