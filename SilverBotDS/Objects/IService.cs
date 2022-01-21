using System.Threading.Tasks;

namespace SilverBotDS.Objects
{
    /// <summary>
    /// A service that does stuff, basically has all of the access to silverbots interfaces while doing something by itself (example would be a health monitor, pixels like archiver)
    /// Services should preferably use their own config stored in a different folder.
    /// </summary>
    public interface IService
    {
        public Task Start();

        public Task Stop();
    }
}