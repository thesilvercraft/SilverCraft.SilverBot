using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace SilverBot.Shared
{
    public interface IProgramExtension
    {
        public Task Register(ServiceProvider sp, Logger log, params object[] additionalContext);
        public Task Reload();
        public Task Unregister(ServiceProvider sp, Logger log, params object[] additionalContext);
       public bool IsLoaded { get; } 
    }

    public class ExpectedContextTypeAttribute : Attribute
    {
        public Type[] Types { get; private set; }
        public ExpectedContextTypeAttribute(params Type[] types)
        {
            Types = types;
        }
    }

    public class ProgramExtensionException : Exception
    {
        public ProgramExtensionException()
        {
        }
        public ProgramExtensionException(string message) : base(message)
        {
        }
    }
    public class ProgramExtensionAlreadyLoadedException : ProgramExtensionException
    {
        public ProgramExtensionAlreadyLoadedException() : base("The program extension has loaded already.")
        {
            
        }
        public ProgramExtensionAlreadyLoadedException(string name) : base($"The program extension {name} has loaded already.")
        {
            
        }
    }
    public class ProgramExtensionNotLoadedException : ProgramExtensionException
    {
        public ProgramExtensionNotLoadedException() : base("The program extension hasn't been loaded yet.")
        {
            
        }
        public ProgramExtensionNotLoadedException(string name) : base($"The program extension {name} hasn't been loaded yet.")
        {
            
        }
    }
}