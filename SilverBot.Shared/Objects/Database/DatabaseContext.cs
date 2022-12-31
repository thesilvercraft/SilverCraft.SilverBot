﻿/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SilverBot.Shared.Objects.Database.Classes;
using SilverBot.Shared.Objects.Database.Classes.ReactionRole;

namespace SilverBot.Shared.Objects.Database;

public class DatabaseContext : DbContext
{
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
        if (plannedevents.Count > 0)
        {
            plannedEvents.RemoveRange(plannedevents);
            await SaveChangesAsync();
        }
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
        var a = serverSettings.FirstOrDefault(x => x.ServerId == id);
        if (a != null)
        {
            return a;
        }
        a = new ServerSettings
        {
            ServerId = id
        };
        serverSettings.Add(a);
        SaveChanges();
        return a;
    }

    public string GetLangCodeGuild(ulong id)
    {
        return serverSettings.FirstOrDefault(x => x.ServerId == id)?.LangName ?? "en";
    }


    public bool IsBanned(ulong id)
    {
        return userSettings.Any(x => x.Id == id && x.IsBanned);
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

    public void SetServerStatStrings(ulong sid, ServerStatString[]? id)
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

    public void ToggleBanUser(ulong id, bool BAN)
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

    public void InserOrUpdateLangCodeUser(ulong id, string lang)
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

    public async Task<string> RunSqlAsync(string sql)
    {
        await using var cmd = Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;
        Database.OpenConnection();
        await using var result = await cmd.ExecuteReaderAsync();
        try
        {
            var dataTable = new DataTable();
            dataTable.Load(result);
            if (dataTable.Rows.Count == 0)
            {
                return "nodata";
            }
            StringBuilder builder = new("```" + Environment.NewLine);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                foreach (var item in dataRow.ItemArray)
                {
                    builder.Append('|').AppendFormat("{0,5}", item);
                }

                builder.AppendLine();
            }

            return builder.Append("```").ToString();
        }
        catch (Exception e)
        {
            Log.Error(e, "Error in dbCtx.RunSQL");
            return "Error";
        }
    }

    public void InserOrUpdateLangCodeGuild(ulong id, string lang)
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
    public DbSet<ServerSettings> serverSettings => Set<ServerSettings>();
    public DbSet<UserSettings> userSettings => Set<UserSettings>();
    public DbSet<UserExperience> userExperiences => Set<UserExperience>();
    public DbSet<UserQuote> userQuotes => Set<UserQuote>();
    public DbSet<PlannedEvent> plannedEvents => Set<PlannedEvent>();
    public DbSet<TranslatorSettings> translatorSettings => Set<TranslatorSettings>();
    public DbSet<ReactionRoleMapping> ReactionRoleMappings => Set<ReactionRoleMapping>();
#pragma warning restore IDE1006 // Naming Styles
}