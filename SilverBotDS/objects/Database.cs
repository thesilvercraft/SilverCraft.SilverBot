using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Objects
{
    public interface ISBDatabase
    {
        Task<bool?> IsOptedInEmotes(ulong serverid);

        Task<Tuple<string, Image>> RunSqlAsync(string sql);

        Task InsertEmoteOptinAsync(Serveroptin e);

        Task<List<Serveroptin>> ServersOptedInEmotesAsync();
    }
}