using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS.Objects
{
    internal class PostgreDatabase : ISBDatabase
    {
        private string connstring;

        /// <summary>
        /// Set the connection string to a string
        /// </summary>
        /// <param name="thingtosetitto">The string to set it to</param>
        public void Setconnstring(string thingtosetitto)
        {
            connstring = thingtosetitto;
        }

        public PostgreDatabase(string conn)
        {
            connstring = conn;
        }

        public NpgsqlConnection NewConnection()
        {
            return new NpgsqlConnection(connstring);
        }

        /// <summary>
        /// Checks if guild is opted in
        /// </summary>
        /// <param name="serverid">the guild id</param>
        /// <returns><c>True</c> if opted in
        /// <c>False</c> if can not be opted in (ShadowBanned)
        /// <c>Null</c> if not opted in (no data)
        /// </returns>
        public async Task<bool?> IsOptedInEmotes(ulong serverid)
        {
            bool? e = null;
            await using var conn = NewConnection();
            try
            {
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand("SELECT optedin FROM serveroptin WHERE serverid = @id", conn);
                cmd.Parameters.AddWithValue("id", Convert.ToInt64(serverid));
                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    e = reader.GetBoolean(0);
                }
                await conn.CloseAsync();
            }
            catch (Exception exep)
            {
                Program.SendLog(exep);
            }

            return e;
        }

        /// <summary>
        /// Runs sql and returns a string or an image
        /// </summary>
        /// <param name="sql">the sql to run</param>
        /// <returns>A string and a image one of them is a null</returns>
        public async Task<Tuple<string, Image>> RunSqlAsync(string sql)
        {
            try
            {
                await using var conn = NewConnection();
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand(sql, conn);
                var dataTable = new DataTable();
                // create data adapter
                var da = new NpgsqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dataTable);
                await conn.CloseAsync();
                StringBuilder thing = new("<html>" +
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
                    "<table style=\"width: 100 % \">");
                if (dataTable.Rows.Count == 0)
                {
                    return new Tuple<string, Image>("nodata", null);
                }
                else
                {
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        thing.AppendLine("<tr>");
                        foreach (var item in dataRow.ItemArray)
                        {
                            thing.AppendLine("<td>" + item + "</td>");
                        }
                        thing.AppendLine("</tr>");
                    }
                    thing.AppendLine("</table></body></html>");
                }
                return new Tuple<string, Image>(null, await Browser.ScreenshotHtmlAsync(thing.ToString()));
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                return new Tuple<string, Image>("Error", null);
            }
        }

        public async Task InsertEmoteOptinAsync(Serveroptin e)
        {
            await using var conn = NewConnection();
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand("INSERT INTO serveroptin (serverid, optedin) VALUES (@p1, @p2)", conn);
            cmd.Parameters.AddWithValue("p1", Convert.ToInt64(e.ServerId));
            cmd.Parameters.AddWithValue("p2", e.Optedin);
            await cmd.ExecuteNonQueryAsync();
            await conn.CloseAsync();
        }

        public async Task<List<Serveroptin>> ServersOptedInEmotesAsync()
        {
            List<Serveroptin> enuma = new();
            try
            {
                await using var conn = NewConnection();
                await conn.OpenAsync();
                await using var cmd = new NpgsqlCommand("SELECT serverid, optedin FROM serveroptin WHERE(optedin = True)", conn);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    enuma.Add(new Serveroptin
                    {
                        ServerId = Convert.ToUInt64(reader.GetInt64(0)),
                        Optedin = reader.GetBoolean(1)
                    });
                }
                await conn.CloseAsync();
                return enuma;
            }
            catch (Exception e)
            {
                Program.SendLog(e);
                throw;
            }
        }
    }
}