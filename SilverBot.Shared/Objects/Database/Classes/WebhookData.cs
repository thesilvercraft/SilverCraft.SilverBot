using System.ComponentModel.DataAnnotations;
using SilverBot.Shared.Utils;

namespace SilverBot.Shared.Objects.Database.Classes
{
    public class WebhookData
    {
        public WebhookData()
        {
        }

        public WebhookData(string webhookUrl)
        {
            WebHookUtils.ParseWebhookUrlNullable(webhookUrl, out var nullableId, out var token);
            if (nullableId is { } id)
            {
                Id = id;
                Token = token;
            }
            else
            {
                throw new ArgumentException(null, nameof(webhookUrl));
            }
        }

        public WebhookData(ulong id, string token)
        {
            Id = id;
            Token = token;
        }

        public ulong Id { get; set; }

        public string Token { get; set; }

        public string URL =>
            $"https://discord.com/api/webhooks/{Id}/{Token}";
    }

    public class DatabaseWebhookData : WebhookData
    {
        public DatabaseWebhookData()
        {
        }

        public DatabaseWebhookData(string webhookUrl)
        {
            WebHookUtils.ParseWebhookUrlNullable(webhookUrl, out var nullableId, out var token);
            if (nullableId is { } id)
            {
                Id = id;
                Token = token;
            }
            else
            {
                throw new ArgumentException(null, nameof(webhookUrl));
            }
        }

        public DatabaseWebhookData(ulong id, string token)
        {
            Id = id;
            Token = token;
        }

        [Key] public Guid DatabaseWebhookDataId { get; set; }
        public virtual Guid ServerSettingsId { get; set; }
    }
}