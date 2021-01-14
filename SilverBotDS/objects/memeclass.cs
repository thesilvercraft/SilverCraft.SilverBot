namespace SilverBotDS
{
#pragma warning disable IDE1006 // Naming Styles

    public class meme
    {
        /// <summary>
        /// the link of the post
        /// </summary>
        public string postLink { get; set; }

        /// <summary>
        /// the subreddit of the post
        /// </summary>
        public string subreddit { get; set; }

        /// <summary>
        /// The title of the post
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// url address of the post
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// bool if the post is nsfw or not
        /// </summary>
        public bool nsfw { get; set; }

        /// <summary>
        /// bool if the post is a spoiler or not
        /// </summary>
        public bool spoiler { get; set; }

        /// <summary>
        /// author of post
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// number of upvotes of post
        /// </summary>
        public int ups { get; set; }

#pragma warning restore IDE1006 // Naming Styles
    }
}