using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SilverBotDS.Utils
{
    internal class WebHookUtilz
    {
        private static readonly Regex WebhookUrlRegex = new(@"^.*(discord|discordapp)\.com\/api\/webhooks\/([\d]+)\/([a-z0-9_-]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        protected WebHookUtilz()
        {
        }

        public static void ParseWebhookUrl(string webhookUrl, out ulong webhookId, out string webhookToken)
        {
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                throw new ArgumentNullException(paramName: nameof(webhookUrl), message:
                    "The given webhook Url cannot be null or whitespace.");
            }

            // thrown when groups are not populated/valid, or when there is no match
            ArgumentException ex(string reason = null)
            {
                return new(paramName: nameof(webhookUrl), message:
                               $"The given webhook Url was not in a valid format. {reason}");
            }

            var match = WebhookUrlRegex.Match(webhookUrl);
            if (match != null)
            {
                // ensure that the first group is a ulong, set the _webhookId
                // 0th group is always the entire match, and 1 is the domain; so start at index 2
                if (!(match.Groups[2].Success && ulong.TryParse(match.Groups[2].Value, NumberStyles.None, CultureInfo.InvariantCulture, out webhookId)))
                {
                    throw ex("The webhook Id could not be parsed.");
                }

                if (!match.Groups[3].Success)
                {
                    throw ex("The webhook token could not be parsed.");
                }

                webhookToken = match.Groups[3].Value;
            }
            else
            {
                throw ex();
            }
        }
    }
}