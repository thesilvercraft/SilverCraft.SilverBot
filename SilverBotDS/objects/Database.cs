using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverBotDS
{
    internal class Database
    {
        private static string connstring;

        /// <summary>
        /// Set the connection string to a string
        /// </summary>
        /// <param name="thingtosetitto">The string to set it to</param>
        public static void setconnstring(string thingtosetitto)
        {
            connstring = thingtosetitto;
        }

        public static NpgsqlConnection NewConnection()
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
        public static async Task<bool?> Isoptedin(ulong serverid)
        {
            bool? e = null;
            using (NpgsqlConnection conn = NewConnection())
            {
                try
                {
                    Debug.WriteLine("opt ");
                    conn.Open();
                    await using var cmd = new NpgsqlCommand("SELECT optedin FROM serveroptin WHERE serverid = @id", conn);
                    cmd.Parameters.AddWithValue("id", Convert.ToInt64(serverid));
                    await using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        e = reader.GetBoolean(0);
                    }
                    conn.Close();
                    Debug.WriteLine("opt2");
                }
                catch (Exception exep)
                {
                    Debug.WriteLine(exep);
                    Console.WriteLine(exep);
                }
            }
            return e;
        }

        /// <summary>
        /// Runs sql and returns a string or an image
        /// </summary>
        /// <param name="sql">the sql to run</param>
        /// <returns>A string and a image one of them is a null</returns>
        //TODO make thing not use string but use string builder
        public static async Task<Tuple<string, Image>> RunSQLAsync(string sql)
        {
            using NpgsqlConnection conn = NewConnection();
            conn.Open();
            await using var cmd = new NpgsqlCommand(sql, conn);

            DataTable dataTable = new DataTable();

            // create data adapter
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            string thing = "<html>" +
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
            if (dataTable.Rows.Count == 0)
            {
                return new Tuple<string, Image>("nodata", null);
            }
            else
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    thing += "<tr>" + Environment.NewLine;
                    foreach (var item in dataRow.ItemArray)
                    {
                        thing += "<td>" + item + "</td>";
                    }
                    thing += "</tr>" + Environment.NewLine;
                }
                thing += "</table></body></html>";
            }

            return new Tuple<string, Image>(null, await Browser.ScreenshotThingAsync(thing));
        }

        public static async Task InsertAsync(Serveroptin e)
        {
            using NpgsqlConnection conn = NewConnection();
            conn.Open();
            await using var cmd = new NpgsqlCommand("INSERT INTO serveroptin (serverid, optedin) VALUES (@p1, @p2)", conn);
            cmd.Parameters.AddWithValue("p1", Convert.ToInt64(e.ServerId));
            cmd.Parameters.AddWithValue("p2", e.optedin);
            await cmd.ExecuteNonQueryAsync();
            conn.Close();
        }

        public static async IAsyncEnumerable<Serveroptin> ServersoptedinAsync()
        {
            using NpgsqlConnection conn = NewConnection();
            conn.Open();
            await using var cmd = new NpgsqlCommand("SELECT serverid, optedin FROM serveroptin WHERE(optedin = True)", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                yield return new Serveroptin
                {
                    ServerId = Convert.ToUInt64(reader.GetInt64(0)),
                    optedin = reader.GetBoolean(1)
                };
            }
            conn.Close();
        }
    }

    public class serverthing
    {
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
    }

    public class Serveroptin
    {
        public ulong ServerId { get; set; }
        public bool optedin { get; set; }
    }
}