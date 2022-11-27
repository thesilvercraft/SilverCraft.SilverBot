/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using SilverBotDS.Objects.Database.Classes;
using SilverBotDS.Objects.Database.Classes.ReactionRole;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace SilverBotDS.Objects;

public class ServerSettings
{
    [Key] public Guid ServerSettingsId { get; set; }
    public ulong ServerId { get; set; }

    public string LangName { get; set; } = "en";
    public bool EmotesOptin { get; set; }
    public ulong? ServerStatsCategoryId { get; set; } = null;

    [NotMapped]
    public ServerStatString[] ServerStatsTemplates
    {
        get => JsonSerializer.Deserialize<ServerStatString[]>(string.IsNullOrWhiteSpace(ServerStatsTemplatesInJson)
            ? "[]"
            : ServerStatsTemplatesInJson);
        set => ServerStatsTemplatesInJson = JsonSerializer.Serialize(value);
    }

    public string ServerStatsTemplatesInJson { get; set; } = "[]";
    public bool RepeatThings { get; set; }
    public bool Reminders { get; set; }

    [NotMapped]
    public string[] Prefixes
    {
        get => JsonSerializer.Deserialize<string[]>(string.IsNullOrWhiteSpace(PrefixesInJson) ? "[]" : PrefixesInJson);
        set => PrefixesInJson = JsonSerializer.Serialize(value);
    }

    public string PrefixesInJson { get; set; } = "[]";
    public virtual List<ReactionRoleMapping> ReactionRoleMappings { get; set; } = new();
}