﻿/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SilverBot.Web.Controllers
{
    [ApiController]
    [Route("[controller].json")]
    [Route("site.webmanifest")]
    public class Manifest : Controller
    {
        public Rootobject Index()
        {
            var obj = new Rootobject
            {
                Name = "SilverBot",
                ShortName = "Silverbot",
                StartUrl = "/",
                BackgroundColor = "#C0C0C0",
                ThemeColor = "#01dff0",
                Display = "standalone"
            };
            switch (DateTime.UtcNow.Month)
            {
                case 6:
                    obj.Icons = new[]
                    {
                        new Icon
                        {
                            Sizes = "any", Src = "/pride/silverbot.svg", Type = "image/svg+xml", Purpose = "any"
                        },
                        new Icon
                        {
                            Sizes = "any", Src = "/pride/silverbot.svg", Type = "image/svg+xml", Purpose = "maskable"
                        }
                    };
                    break;
                case 10 when DateTime.UtcNow.Day == 31:
                    obj.Icons = new[]
                    {
                        new Icon
                        {
                            Sizes = "any", Src = "/halloween/silverbot.svg", Type = "image/svg+xml", Purpose = "any"
                        },
                        new Icon
                        {
                            Sizes = "any", Src = "/halloween/silverbot.svg", Type = "image/svg+xml",
                            Purpose = "maskable"
                        }
                    };
                    obj.ThemeColor = "#EF5B31";
                    break;
                default:
                    obj.Icons = new[]
                    {
                        new Icon { Sizes = "any", Src = "/silverbot.svg", Type = "image/svg+xml", Purpose = "any" },
                        new Icon { Sizes = "any", Src = "/silverbot.svg", Type = "image/svg+xml", Purpose = "maskable" }
                    };
                    break;
            }

            return obj;
        }

        public class Rootobject
        {
            [JsonPropertyName("name")] public string Name { get; set; }

            [JsonPropertyName("short_name")] public string ShortName { get; set; }

            [JsonPropertyName("icons")] public Icon[] Icons { get; set; }

            [JsonPropertyName("start_url")] public string StartUrl { get; set; }

            [JsonPropertyName("display")] public string Display { get; set; }

            [JsonPropertyName("background_color")] public string BackgroundColor { get; set; }

            [JsonPropertyName("theme_color")] public string ThemeColor { get; set; }
        }

        public class Icon
        {
            [JsonPropertyName("src")] public string Src { get; set; }

            [JsonPropertyName("sizes")] public string Sizes { get; set; }

            [JsonPropertyName("type")] public string Type { get; set; }

            [JsonPropertyName("purpose")] public string Purpose { get; set; }
        }
    }
}