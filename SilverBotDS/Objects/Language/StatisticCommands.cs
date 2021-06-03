namespace SilverBotDS.Objects
{
    public partial class Language
    {
        public class StatisticCommands
        {
            public string SetToProvidedStrings { get; set; } = "Set them to the provided things";
            public string SetToDefaultStrings { get; set; } = "Set them to the default things";
            public string NoPerm { get; set; } = "Please give me permission to manage channels in the category";
            public string CategorySetSuccess { get; set; } = "Set the category to {0}, you might want to add atleast 4 channels of any kind in it and then run the `setstatstrings` command";
            public string EmojiMessageDownloadStart { get; set; } = "Downloading at most {0} messages which will take around {1} cause of discord ratelimit";
            public string EmojiMessageDownloadEnd { get; set; } = "Downloaded {0} while expecting {1}, estimated time was: {2} actual time was {3} expected time if provided correct limit would be {4}\nWell anyways processing messages";
            public string EmojiEnd { get; set; } = "i went through {0} messages in {1}(including download) {2}(excluding download)\nEmote usage data:";
        }
    }
}