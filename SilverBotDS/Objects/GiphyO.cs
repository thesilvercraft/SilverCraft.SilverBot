namespace SilverBotDS
{
    internal class GiphyO
    {
        private static GiphyDotNet.Manager.Giphy giphy = new GiphyDotNet.Manager.Giphy();

        /// <summary>
        /// sets the giphy to the specified object
        /// </summary>
        /// <param name="x">the object to set it to</param>
        public static void Set(GiphyDotNet.Manager.Giphy x)
        {
            giphy = x;
        }

        /// <summary>
        /// Get the giphy
        /// </summary>
        /// <returns>The giphy</returns>
        public static GiphyDotNet.Manager.Giphy Get()
        {
            return giphy;
        }
    }
}