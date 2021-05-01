using System;

namespace SilverBotDS.Exceptions
{
    [Serializable]
    public class TemplateReturningNullException : Exception
    {
        public TemplateReturningNullException()
        {
        }

        public TemplateReturningNullException(string template)
            : base($"Assembly.GetExecutingAssembly().GetManifestResourceStream({template}) returned null")
        {
        }
    }
}