using LiteDB;

namespace SilverBotDS.Objects
{
    public class ServerOptin
    {
        [BsonId(false)]
        public ulong ServerId { get; init; }

        public bool Optedin { get; init; }
    }
}