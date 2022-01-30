using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SDBrowser;
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Objects.Database.Classes.ReactionRole;

namespace SilverBotDS.Objects;

public class DatabaseContext : DbContext
{
    public static readonly string HtmlStart = "<html>" +
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

    private readonly ServerStatString[] StatsTemplates =
    {
        new("All Members: {AllMembersCount}"), new("Members: {MemberCount}"), new("Bots: {BotsCount}"),
        new("Boosts: {BoostCount}")
    };

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    /// <summary>
    ///     This method is used to delete all references of a user from the database.
    /// </summary>
    public async Task RemoveUser(ulong userId)
    {
        var user = await userSettings.FirstOrDefaultAsync(x => x.Id == userId);
        if (user != null)
        {
            userSettings.Remove(user);
            await SaveChangesAsync();
        }

        var userExp = await userExperiences.FirstOrDefaultAsync(x => x.Id == userId);
        if (userExp != null)
        {
            userExperiences.Remove(userExp);
            await SaveChangesAsync();
        }

        var userQuote = await userQuotes.FirstOrDefaultAsync(x => x.UserId == userId);
        if (userQuote != null)
        {
            userQuotes.Remove(userQuote);
            await SaveChangesAsync();
        }

        var translatorSetting = await translatorSettings.FirstOrDefaultAsync(x => x.Id == userId);
        if (translatorSetting != null)
        {
            translatorSettings.Remove(translatorSetting);
            await SaveChangesAsync();
        }

        var plannedevents = await plannedEvents.Where(x => x.UserID == userId).ToListAsync();
        if (plannedevents?.Count > 0)
        {
            plannedEvents.RemoveRange(plannedevents);
            await SaveChangesAsync();
        }
    }

    public List<ulong> GetIdsOfEmoteOptedInServers()
    {
        return serverSettings.Where(a => a.EmotesOptin).Select(x => x.ServerId).AsEnumerable().Reverse().ToList();
    }

    public string GetLangCodeUser(ulong id)
    {
        return userSettings.FirstOrDefault(x => x.Id == id)?.LangName ?? "en";
    }

    public Tuple<ulong, ulong?, ServerStatString[]>[] GetStatisticSettings()
    {
        return serverSettings.Where(x => x.ServerStatsCategoryId != null).Select(x =>
                new Tuple<ulong, ulong?, ServerStatString[]>(x.ServerId, x.ServerStatsCategoryId,
                    x.ServerStatsTemplates))
            .ToArray();
    }

    public ServerSettings GetServerSettings(ulong id)
    {
        var a = serverSettings.Where(x => x.ServerId == id).FirstOrDefault();
        if (a == null)
        {
            a = new ServerSettings
            {
                ServerId = id
            };
            serverSettings.Add(a);
            SaveChanges();
        }

        return a;
    }

    public string GetLangCodeGuild(ulong id)
    {
        return serverSettings.FirstOrDefault(x => x.ServerId == id)?.LangName ?? "en";
    }

    public bool IsOptedInEmotes(ulong id)
    {
        return serverSettings.Any(x => x.ServerId == id && x.EmotesOptin);
    }

    public bool IsBanned(ulong id)
    {
        return userSettings.Any(x => x.Id == id && x.IsBanned);
    }

    public void OptIntoEmotes(ulong id)
    {
        var serversettings = serverSettings.FirstOrDefault(x => x.ServerId == id);
        if (serversettings is not null)
        {
            serversettings.EmotesOptin = true;
            serverSettings.Update(serversettings);
        }
        else
        {
            serverSettings.Add(new ServerSettings
            {
                EmotesOptin = true,
                ServerId = id
            });
        }

        SaveChanges();
    }

    public void SetServerStatsCategory(ulong sid, ulong? id)
    {
        var serversettings = serverSettings.FirstOrDefault(x => x.ServerId == sid);
        if (serversettings is not null)
        {
            serversettings.ServerStatsCategoryId = id;
            serverSettings.Update(serversettings);
        }
        else
        {
            serverSettings.Add(new ServerSettings
            {
                ServerId = sid,
                ServerStatsCategoryId = id
            });
        }

        SaveChanges();
    }

    public void SetServerPrefixes(ulong sid, string[] prefixes)
    {
        var serversettings = serverSettings.FirstOrDefault(x => x.ServerId == sid);
        if (serversettings is not null)
        {
            serversettings.Prefixes = prefixes;
            serverSettings.Update(serversettings);
        }
        else
        {
            serverSettings.Add(new ServerSettings
            {
                ServerId = sid,
                Prefixes = prefixes
            });
        }

        SaveChanges();
    }

    public void SetServerStatStrings(ulong sid, ServerStatString[] id)
    {
        id ??= StatsTemplates;
        var serversettings = serverSettings.FirstOrDefault(x => x.ServerId == sid);
        if (serversettings is not null)
        {
            serversettings.ServerStatsTemplates = id;
            serverSettings.Update(serversettings);
        }
        else
        {
            serverSettings.Add(new ServerSettings
            {
                ServerId = sid,
                ServerStatsTemplates = id
            });
        }

        SaveChanges();
    }

    internal void ToggleBanUser(ulong id, bool BAN)
    {
        var usersettings = userSettings.FirstOrDefault(x => x.Id == id);
        if (usersettings is not null)
        {
            usersettings.IsBanned = BAN;
            userSettings.Update(usersettings);
        }
        else
        {
            userSettings.Add(new UserSettings
            {
                Id = id,
                IsBanned = BAN
            });
        }

        SaveChanges();
    }

    internal void InserOrUpdateLangCodeUser(ulong id, string lang)
    {
        var usersettings = userSettings.FirstOrDefault(x => x.Id == id);
        if (usersettings is not null)
        {
            usersettings.LangName = lang;
            userSettings.Update(usersettings);
        }
        else
        {
            userSettings.Add(new UserSettings
            {
                LangName = lang,
                Id = id,
                IsBanned = false
            });
        }

        SaveChanges();
    }

    public async Task<Tuple<string, Stream>> RunSqlAsync(string sql, IBrowser browser)
    {
        await using var cmd = Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;
        Database.OpenConnection();
        using var result = await cmd.ExecuteReaderAsync();
        try
        {
            var dataTable = new DataTable();
            dataTable.Load(result);
            StringBuilder thing = new(HtmlStart);
            if (dataTable.Rows.Count == 0)
            {
                return new Tuple<string, Stream>("nodata", null);
            }

            if (browser is null)
            {
                StringBuilder builder = new("```" + Environment.NewLine);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        builder.Append('|').AppendFormat("{0,5}", item);
                    }

                    builder.AppendLine();
                }

                return new Tuple<string, Stream>(builder.Append("```").ToString(), null);
            }

            foreach (DataRow dataRow in dataTable.Rows)
            {
                thing.AppendLine("<tr>");
                foreach (var item in dataRow.ItemArray)
                {
                    thing.Append("<td>").Append(item).AppendLine("</td>");
                }

                thing.AppendLine("</tr>");
            }

            thing.AppendLine("</table></body></html>");
            return new Tuple<string, Stream>(null, await browser.RenderHtmlAsync(thing.ToString()));
        }
        catch (Exception e)
        {
            Program.SendLog(e);
            return new Tuple<string, Stream>("Error", null);
        }
    }

    internal void InserOrUpdateLangCodeGuild(ulong id, string lang)
    {
        var serversettings = serverSettings.FirstOrDefault(x => x.ServerId == id);
        if (serversettings is not null)
        {
            serversettings.LangName = lang;
            serverSettings.Update(serversettings);
        }
        else
        {
            serverSettings.Add(new ServerSettings
            {
                LangName = lang,
                ServerId = id
            });
        }

        SaveChanges();
    }

#pragma warning disable IDE1006 // Naming Styles
    public DbSet<ServerSettings> serverSettings { get; set; }
    public DbSet<UserSettings> userSettings { get; set; }
    public DbSet<UserExperience> userExperiences { get; set; }
    public DbSet<UserQuote> userQuotes { get; set; }
    public DbSet<PlannedEvent> plannedEvents { get; set; }
    public DbSet<TranslatorSettings> translatorSettings { get; set; }
    public DbSet<ReactionRoleMapping> ReactionRoleMappings { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}