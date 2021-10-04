using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SilverBotDS.Controlllers
{
    [ApiController]
    [Route("[controller].json")]
    [Route("site.webmanifest")]
    public class Manifest : Controller
    {
        public Rootobject Index()
        {
            var obj = new Rootobject { Name = "SilverBot", ShortName = "Silverbot", StartUrl = "/", BackgroundColor = "#C0C0C0", ThemeColor = "#01dff0", Display = "standalone" };
            if (DateTime.UtcNow.Month == 6)
            {
                obj.Icons = new[] { new Icon { Sizes = "any", Src = "/pride/silverbot.svg", Type = "image/svg+xml", Purpose="any" }, new Icon { Sizes = "any", Src = "/pride/silverbot.svg", Type = "image/svg+xml", Purpose = "maskable" }, new Icon { Sizes = "512x512", Src = "/pride/android-chrome-512x512.png", Type = "image/png" }, new Icon { Sizes = "192x192", Src = "/pride/android-chrome-192x192.png", Type = "image/png" } };
            }
            else if(DateTime.UtcNow.Month == 10 && DateTime.UtcNow.Day == 31)
            {
                obj.Icons = new[] { new Icon { Sizes = "any", Src = "/halloween/silverbot.svg", Type = "image/svg+xml", Purpose = "any" }, new Icon { Sizes = "any", Src = "/halloween/silverbot.svg", Type = "image/svg+xml", Purpose = "maskable" }, new Icon { Sizes = "512x512", Src = "/halloween/android-chrome-512x512.png", Type = "image/png" }, new Icon { Sizes = "192x192", Src = "/halloween/android-chrome-192x192.png", Type = "image/png" } };
                obj.ThemeColor = "#EF5B31";
            }
            else
            {
                obj.Icons = new[] { new Icon {Sizes = "any", Src = "/silverbot.svg", Type = "image/svg+xml", Purpose = "any" }, new Icon { Sizes = "any", Src = "/silverbot.svg", Type = "image/svg+xml", Purpose = "maskable" }, new Icon { Sizes = "512x512", Src = "/android-chrome-512x512.png", Type = "image/png" }, new Icon { Sizes = "192x192", Src = "/android-chrome-192x192.png", Type = "image/png" } };
            }
            return obj;
        }

        public class Rootobject
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("short_name")]
            public string ShortName { get; set; }
            [JsonPropertyName("icons")]
            public Icon[] Icons { get; set; }
            [JsonPropertyName("start_url")]
            public string StartUrl { get; set; }
            [JsonPropertyName("display")]
            public string Display { get; set; }
            [JsonPropertyName("background_color")]
            public string BackgroundColor { get; set; }
            [JsonPropertyName("theme_color")]
            public string ThemeColor { get; set; }
        }

        public class Icon
        {
            [JsonPropertyName("src")]
            public string Src { get; set; }
            [JsonPropertyName("sizes")]
            public string Sizes { get; set; }
            [JsonPropertyName("type")]
            public string Type { get; set; }
            [JsonPropertyName("purpose")]
            public string Purpose { get; set; }
        }
    }
}