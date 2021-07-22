using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Controllers
{
    [ApiController]
    [Route("browserconfig.xml")]
    [Produces("application/xml")]
    public class BrowserConfig : Controller
    {
        public browserconfig Index()
        {
            var obj = new browserconfig();
            obj.msapplication = new();
            obj.msapplication.tile = new();
            obj.msapplication.tile.square70x70logo = new();
            obj.msapplication.tile.square150x150logo = new();
            obj.msapplication.tile.square310x310logo = new();
            obj.msapplication.tile.wide310x150logo = new();
            if (DateTime.UtcNow.Month == 6)
            {
                obj.msapplication.tile.square70x70logo.src = "/pride/mstile-70x70.png?v=69";
                obj.msapplication.tile.square150x150logo.src = "/pride/mstile-150x150.png?v=69";
                obj.msapplication.tile.square310x310logo.src = "/pride/mstile-310x310.png?v=69";
                obj.msapplication.tile.wide310x150logo.src = "/pride/mstile-310x150.png?v=69";
                obj.msapplication.tile.TileColor = "#34f0fd";
            }
            else if (DateTime.UtcNow.Month == 10 && DateTime.UtcNow.Day == 31)
            {
                obj.msapplication.tile.square70x70logo.src = "/halloween/mstile-70x70.png?v=69";
                obj.msapplication.tile.square150x150logo.src = "/halloween/mstile-150x150.png?v=69";
                obj.msapplication.tile.square310x310logo.src = "/halloween/mstile-310x310.png?v=69";
                obj.msapplication.tile.wide310x150logo.src = "/halloween/mstile-310x150.png?v=69";
                obj.msapplication.tile.TileColor = "#EF5B31";
            }
            else
            {
                obj.msapplication.tile.square70x70logo.src = "/mstile-70x70.png?v=69";
                obj.msapplication.tile.square150x150logo.src = "/mstile-150x150.png?v=69";
                obj.msapplication.tile.square310x310logo.src = "/mstile-310x310.png?v=69";
                obj.msapplication.tile.wide310x150logo.src = "/mstile-310x150.png?v=69";
                obj.msapplication.tile.TileColor = "#34f0fd";
            }
            return obj;
        }
        //From here on out blame vs
#pragma warning disable IDE1006 // Naming Styles
        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        [System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]

        public partial class browserconfig

        {
            public browserconfigMsapplication msapplication
            {
                get;
                set;
            }
        }

        [SerializableAttribute()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class browserconfigMsapplication
        {
            public browserconfigMsapplicationTile tile
            {
                get;
                set;
            }
        }

        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class browserconfigMsapplicationTile
        {
            public browserconfigMsapplicationTileSquare70x70logo square70x70logo
            {
                get;
                set;
            }

            public browserconfigMsapplicationTileSquare150x150logo square150x150logo
            {
                get;
                set;
            }

            public browserconfigMsapplicationTileSquare310x310logo square310x310logo
            {
                get;
                set;
            }

            public browserconfigMsapplicationTileWide310x150logo wide310x150logo
            {
                get;
                set;
            }

            public string TileColor
            {
                get;
                set;
            }
        }

        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class browserconfigMsapplicationTileSquare70x70logo
        {
            [System.Xml.Serialization.XmlAttribute()]
            public string src
            {
                get;
                set;
            }
        }

        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class browserconfigMsapplicationTileSquare150x150logo
        {

            [System.Xml.Serialization.XmlAttribute()]
            public string src
            {
                get;
                set;
            }
        }

        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class browserconfigMsapplicationTileSquare310x310logo
        {
            [System.Xml.Serialization.XmlAttribute()]
            public string src
            {
                get;
                set;
            }
        }

        [Serializable()]
        [System.ComponentModel.DesignerCategory("code")]
        [System.Xml.Serialization.XmlType(AnonymousType = true)]
        public partial class browserconfigMsapplicationTileWide310x150logo
        {
            [System.Xml.Serialization.XmlAttribute()]
            public string src
            {
                get;
                set;
            }
        }
#pragma warning restore IDE1006 // Naming Styles

    }
}
