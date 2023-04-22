/* This file is part of SilverBot.
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

namespace SilverBot.Shared.Objects.Database
{
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
            UpsertServerSettings(sid, (settings, _) => settings.ServerStatsCategoryId = id);
        }

        public void UpsertServerSettings(ulong serverId, Action<ServerSettings, bool> action)
        {
            var serversettings = serverSettings.FirstOrDefault(x => x.ServerId == serverId);
            if (serversettings is not null)
            {
                action.Invoke(serversettings, true);
                serverSettings.Update(serversettings);
            }
            else
            {
                serversettings = new ServerSettings
                {
                    ServerId = serverId
                };
                serverSettings.Add(serversettings);
                action.Invoke(serversettings, false);
            }

            SaveChanges();
        }

        public void UpsertUserSettings(ulong userId, Action<UserSettings, bool> action)
        {
            var usersettings = userSettings.Find(userId);
            if (usersettings is not null)
            {
                action.Invoke(usersettings, true);
                userSettings.Update(usersettings);
            }
            else
            {
                usersettings = new UserSettings
                {
                    Id = userId
                };
                userSettings.Add(usersettings);
                action.Invoke(usersettings, false);
            }

            SaveChanges();
        }

        public void SetServerPrefixes(ulong sid, string[] prefixes)
        {
            UpsertServerSettings(sid, (settings, _) => settings.Prefixes = prefixes);
        }

        public void SetServerStatStrings(ulong sid, ServerStatString[]? id)
        {
            id ??= StatsTemplates;
            UpsertServerSettings(sid, (settings, _) => settings.ServerStatsTemplates = id);
        }

        public void ToggleBanUser(ulong id, bool BAN)
        {
            UpsertUserSettings(id, (settings, _) => settings.IsBanned = BAN);
        }

        public void UpsertUserLanguageCode(ulong id, string lang)
        {
            UpsertUserSettings(id, (settings, _) => settings.LangName = lang);
        }

        public void UpsertBing(ulong userId, ServerSettings s, Action<BingRankingData, bool> action)
        {
            var userRankingData = s.BingRankingData.FirstOrDefault(x => x.UserId == userId);
            if (userRankingData is not null)
            {
                action.Invoke(userRankingData, true);
                serverSettings.Update(s);
            }
            else
            {
                userRankingData = new BingRankingData
                {
                    UserId = userId, ServerId = s.ServerId, BingCount = 0
                };
                s.BingRankingData.Add(userRankingData);
                UpsertUserSettings(userId, (settings, b) => { settings.BingRankingData.Add(userRankingData); });
                action.Invoke(userRankingData, false);
            }
        }

        public ulong UpsertIncrementUserBing(ulong userId, ulong serverId)
        {
            ulong bc = 0;
            UpsertServerSettings(serverId, (x, y) => { UpsertBing(userId, x, (z, u) => { bc = ++z.BingCount; }); });
            return bc;
        }

        public async Task<string> RunSqlAsync(string sql)
        {
            await using var cmd = Database.GetDbConnection().CreateCommand();
            cmd.CommandText = sql;
            await Database.OpenConnectionAsync();
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

        public void UpsertGuildLanguageCode(ulong id, string lang)
        {
            UpsertServerSettings(id, (settings, _) => settings.LangName = lang);
        }

#pragma warning disable IDE1006 // Naming Styles
        public DbSet<ServerSettings> serverSettings => Set<ServerSettings>();
        public DbSet<UserSettings> userSettings => Set<UserSettings>();
        public DbSet<UserExperience> userExperiences => Set<UserExperience>();
        public DbSet<PlannedEvent> plannedEvents => Set<PlannedEvent>();
        public DbSet<TranslatorSettings> translatorSettings => Set<TranslatorSettings>();
        public DbSet<ReactionRoleMapping> ReactionRoleMappings => Set<ReactionRoleMapping>();
#pragma warning restore IDE1006 // Naming Styles
    }
}