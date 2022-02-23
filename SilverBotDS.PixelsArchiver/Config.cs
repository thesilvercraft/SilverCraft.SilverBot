using SilverBotDS.Attributes;
using SilverBotDS.Utils;
using System.Xml;
using System.Xml.Serialization;

namespace SilverBotDS.PixelsArchiver.Objects;

[Serializable]
public class PixelsArchiverConfig
{
    private const ulong CurrentConfVer = 1;

    [XmlDescription("The current config version, don't change unless insctructed by dev")]
    public ulong? ConfigVer { get; set; }

    public bool SendErrorsThroughSegment { get; set; } = false;

    [XmlDescription("Webhooks for archiving")]
    public string[] ArchiveWebhooks { get; set; } = new[] { "https://discordapp.com/api/webhooks/id/key" };

    [XmlDescription("Should a zip be saved after sending to the webhooks")]
    public bool SaveZip { get; set; } = false;

    [XmlDescription("Where the hell do we get our data from anlong with da bear-er token")]
    public SerializableDictionary<string, string> ApisToArchivePicturesFrom { get; set; } = new() { { "https://silverdiamond.cf/getpixels", "69" } };

    private const string ConfigLocation = "pixelconfig.xml";
    private const string OldConfigLocation = "pixelconfig.xml.old";

    private static XmlDocument MakeDocumentWithComments(XmlDocument xmlDocument)
    {
        foreach (var i in typeof(PixelsArchiverConfig).GetMembers())
        {
            foreach (var e in i.GetCustomAttributes(false))
            {
                if (e.GetType() == typeof(XmlDescriptionAttribute))
                {
                    xmlDocument = XmlUtils.CommentBeforeObject(xmlDocument, $"/PixelsArchiverConfig/{i.Name}",
                    ((XmlDescriptionAttribute)e).Description);
                }
                else if (e.GetType() == typeof(XmlCommentInsideAttribute))
                {
                    xmlDocument = XmlUtils.CommentInObject(xmlDocument, $"/PixelsArchiverConfig/{i.Name}",
                    ((XmlCommentInsideAttribute)e).Comment);
                }
            }
        }

        return xmlDocument;
    }

    public static async Task OutdatedConfigTask(PixelsArchiverConfig readconfig)
    {
        using var streamReader = new StreamReader(ConfigLocation);
        await using var streamWriter = new StreamWriter(OldConfigLocation, false);
        await streamWriter.WriteAsync(await streamReader.ReadToEndAsync());
    }

    public static async Task<PixelsArchiverConfig?> GetAsync()
    {
        var serializer = new XmlSerializer(typeof(PixelsArchiverConfig));
        if (!File.Exists(ConfigLocation))
        {
            var conf = new PixelsArchiverConfig
            {
                ConfigVer = CurrentConfVer
            };

            await using (var streamWriter = new StreamWriter(ConfigLocation))
            {
                MakeDocumentWithComments(XmlUtils.SerializeToXmlDocument(conf)).Save(streamWriter);
                streamWriter.Close();
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{ConfigLocation} should exist in the CWD, edit it, save it and restart silverbot OR reenable PixelsArchiver");
            return null;
        }
        PixelsArchiverConfig? cnf = null;
        using (var fs = new StreamReader(ConfigLocation))
        {
            cnf = (PixelsArchiverConfig?)serializer.Deserialize(fs);
            if (cnf is { ConfigVer: not CurrentConfVer })
            {
                fs.Dispose();
                await OutdatedConfigTask(cnf);
                return await GetAsync();
            }
        }
        return cnf;
    }
}