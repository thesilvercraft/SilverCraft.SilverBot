using System.Text.Json.Serialization;

namespace SilverBotDS.Commands
{
    public partial class CalculatorCommands
    {
        public class MathStep
        {
            [JsonPropertyName("oldval")]
            public string OldVal { get; set; }

            [JsonPropertyName("newval")]
            public string NewVal { get; set; }

            [JsonPropertyName("step")]
            public string Step { get; set; }
        }
    }
}