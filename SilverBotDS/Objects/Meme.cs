using System.Text.Json.Serialization;

namespace SilverBotDS
{
    public class Meme
    {
        /// <summary>
        /// the link of the post
        /// </summary>
        [JsonPropertyName("postLink")]
        public string PostLink { get; set; }

        /// <summary>
        /// the subreddit of the post
        /// </summary>
        [JsonPropertyName("subreddit")]
        public string Subreddit { get; set; }

        /// <summary>
        /// The title of the post
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// url address of the post
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// bool if the post is nsfw or not
        /// </summary>
        [JsonPropertyName("nsfw")]
        public bool Nsfw { get; set; }

        /// <summary>
        /// bool if the post is a spoiler or not
        /// </summary>
        [JsonPropertyName("spoiler")]
        public bool Spoiler { get; set; }

        /// <summary>
        /// author of post
        /// </summary>
        [JsonPropertyName("author")]
        public string Author { get; set; }

        /// <summary>
        /// number of upvotes of post
        /// </summary>
        [JsonPropertyName("ups")]
        public int Ups { get; set; }
    }
}