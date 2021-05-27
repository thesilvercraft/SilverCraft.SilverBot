namespace SilverBotDS.Objects
{
    public partial class Language
    {
        public class SteamCommands
        {
            public string FreeToPlayGameType { get; set; } = "F2P";
            public string NotAvailableGameType { get; set; } = "Not Available";
            public string CostsMoneyGameTypeBug { get; set; } = "It costs merica bucks but idk how much";
            public string NoGamesWereReturned { get; set; } = "No Games were returned";
            public string NoGamesWereReturnedDescription { get; set; } = "Try again later, or try changing your search term";
            public string AmericanMoney { get; set; } = "{0} merica bucks";
        }
    }
}