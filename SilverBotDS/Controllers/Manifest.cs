using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var obj = new Rootobject { name = "SilverBot", short_name = "Silverbot", start_url = "/", background_color = "#C0C0C0", theme_color = "#01dff0", display = "standalone" };
            if (DateTime.UtcNow.Month == 6)
            {
                obj.icons = new Icon[] { new Icon { sizes = "any", src = "/pride/silverbot.svg", type = "image/svg+xml" }, new Icon { sizes = "512x512", src = "/pride/android-chrome-512x512.png", type = "image/png" }, new Icon { sizes = "192x192", src = "/pride/android-chrome-192x192.png.png", type = "image/png" } };
            }
            else
            {
                obj.icons = new Icon[] { new Icon { sizes = "any", src = "/silverbot.svg", type = "image/svg+xml" }, new Icon { sizes = "512x512", src = "/android-chrome-512x512.png", type = "image/png" }, new Icon { sizes = "192x192", src = "/android-chrome-192x192.png.png", type = "image/png" } };
            }
            return obj;
        }

        public class Rootobject
        {
            public string name { get; set; }
            public string short_name { get; set; }
            public Icon[] icons { get; set; }
            public string start_url { get; set; }
            public string display { get; set; }
            public string background_color { get; set; }
            public string theme_color { get; set; }
        }

        public class Icon
        {
            public string src { get; set; }
            public string sizes { get; set; }
            public string type { get; set; }
        }
    }
}