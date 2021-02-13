using System;
using System.Net.Http;
using System.Text;

namespace SilverBotDS
{
    public static class WebClient
    {
        private static readonly HttpClient HttpClient = NewhttpClientwithstrign();

        public static HttpClient NewhttpClientwithstrign()
        {
            HttpClient e = new HttpClient();
            e.DefaultRequestHeaders.UserAgent.TryParseAdd($"Mozilla/5.0 (compatible; SilverBot/{VersionInfo.VNumber}; {ThisAssembly.Git.RepositoryUrl})");
            Console.WriteLine(UserAgent());
            Console.WriteLine($"Mozilla/5.0 (compatible; SilverBot/{VersionInfo.VNumber}; {ThisAssembly.Git.RepositoryUrl})");
            return e;
        }

        private static string UserAgent()
        {
            StringBuilder builder = new();
            builder.Append("Mozilla/5.0 ");
            builder.Append('(');
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    builder.Append($"Windows NT {Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor};");
                    if (Environment.Is64BitOperatingSystem)
                    {
                        builder.Append("Win64; x64");
                    }
                    else
                    {
                        builder.Append("Win32; x86");
                    }
                    break;

                case PlatformID.Unix:
                    builder.Append($"LiNUX ");
                    if (Environment.Is64BitOperatingSystem)
                    {
                        builder.Append("x64");
                    }
                    else
                    {
                        builder.Append("x86");
                    }
                    break;

                default:
                    break;
            }
            builder.Append(')');
            builder.Append(' ');
            builder.Append($"SilverBot/{VersionInfo.VNumber};");
            return builder.ToString();
        }

        public static HttpClient Get()
        {
            return HttpClient;
        }
    }
}