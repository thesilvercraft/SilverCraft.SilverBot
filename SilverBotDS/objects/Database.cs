using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SilverBotDS.Objects
{
    public interface ISBDatabase
    {
        public readonly static string HtmlStart = "<html>" +
                     "<head>" +
                     "<style>" +
                     "table, th, td {" +
                     "border: 2px solid white;" +
                     "border-collapse: collapse;" +
                     "}" +
                     "table{" +
                     "width: 100%;" +
                     "height: 100%;" +
                     "}" +
                     "th,tr{" +
                     "color:#ffffff;" +
                     "font-size: 25px;" +
                     "}" +
                     "body{" +
                     "background-color:2C2F33;" +
                     "}" +
                     "</style>" +
                     "</head>" +
                     "<body>" +
                     "<table style=\"width: 100 % \">";

        /// <summary>
        /// See if a guild is opted in to emotes
        /// </summary>
        /// <param name="serverid">the guild id</param>
        /// <returns>a nullable bool, true if opted in, false if manualy disabled, null if no info</returns>
        async Task<bool?> IsOptedInEmotes(ulong serverid) => (await ServersOptedInEmotesAsync()).Find(x => x.ServerId == serverid)?.Optedin;

        /// <summary>
        /// Run some sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>a tuple with an image or a string, one will be null</returns>
        Task<Tuple<string, Image>> RunSqlAsync(string sql);

        /// <summary>
        /// Opt in a guild
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        Task InsertEmoteOptinAsync(Serveroptin e);

        /// <summary>
        /// Get all the servers opted in to emotes
        /// </summary>
        /// <returns></returns>
        Task<List<Serveroptin>> ServersOptedInEmotesAsync();

        Task<string> GetLangCodeGuild(ulong id);

        Task<string> GetLangCodeUser(ulong id);

        Task InserOrUpdateLangCodeUser(DbLang l);

        Task InserOrUpdateLangCodeGuild(DbLang l);
    }
}