using SilverBotDS.Commands;
using SilverBotDS.Converters;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBotDS.Objects;

internal class ImageSteps : IDisposable
{
    private HttpClient client;
    private bool disposedValue;
    public Step[] steps;

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void SetClient(HttpClient c)
    {
        client = c;
    }

    public static async Task<ImageSteps> Create(string url, HttpClient c)
    {
        var serializer = new XmlSerializer(typeof(Step[]), "SilverBotDS.Objects");
        var rm = await c.GetAsync(url);
        var ist = new ImageSteps();
        using StringReader stringReader = new(await rm.Content.ReadAsStringAsync());
        ist.steps = (Step[])serializer.Deserialize(stringReader);
        ist.SetClient(c);
        return ist;
    }

    public async Task<Image> ExecuteStepsAsync(Step[] filledsteps)
    {
        Image Bitmap = null;
        foreach (var step in filledsteps)
        {
            if (step is TemplateStep step2)
            {
                if (Bitmap is null)
                {
                    Bitmap = step2.GetImage(client);
                }
                else
                {
                    Bitmap.Mutate(x => x.DrawImage(step2.GetImage(client), new Point(0, 0), 1));
                }
            }
            else if (step is PictureStep step1)
            {
                using var resizedbytes = (await ImageModule.ResizeAsync(await step1.Image().GetBytesAsync(client),
                    new Size((int)step1.xSize, (int)step1.ySize), PngFormat.Instance)).Item1;
                using var resizedimg = await Image.LoadAsync(resizedbytes);
                Bitmap.Mutate(x => x.DrawImage(resizedimg, new Point((int)step.x, (int)step.y), 1));
            }
        }

        return Bitmap;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                for (var i = 0; i < steps.Length; i++)
                {
                    if (steps[i] is TemplateStep step)
                    {
                        step.Dispose();
                    }

                    if (steps[i] is PictureStep step1)
                    {
                        step1.Dispose();
                    }
                }
            }

            steps = null;
            client = null;
            disposedValue = true;
        }
    }

    ~ImageSteps()
    {
        Dispose(false);
    }
}

[DataContract]
[XmlInclude(typeof(TemplateStep))]
[XmlInclude(typeof(PictureStep))]
[KnownType(typeof(TemplateStep))]
[KnownType(typeof(PictureStep))]
public class Step
{
    public Step()
    {
        //leave for XML serialization
    }

    public Step(ulong x, ulong y)
    {
        this.x = x;
        this.y = y;
    }

    public ulong x { get; set; }
    public ulong y { get; set; }
}

public class TemplateStep : Step, IDisposable
{
    [XmlIgnore] private Image _image;

    public bool isUrl;

    public string template;

    public TemplateStep()
    {
        //leave for XML serialization
    }

    public TemplateStep(ulong x, ulong y, string template, bool isUrl)
    {
        this.x = x;
        this.y = y;
        this.template = template;
        this.isUrl = isUrl;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Image GetImage(HttpClient e = null)
    {
        e ??= new HttpClient();
        if (_image is null)
        {
            if (isUrl)
            {
                using var memorystream =
                    new MemoryStream(new SdImage(template).GetBytesAsync(e).GetAwaiter().GetResult());
                _image = Image.Load(memorystream);
            }
            else
            {
                using var FileStream = new FileStream(template, FileMode.Open);
                _image = Image.Load(FileStream);
            }
        }

        return _image;
    }

    ~TemplateStep()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _image.Dispose();
        }

        template = null;
    }
}

public class PictureStep : Step, IDisposable
{
    public PictureStep()
    {
    }

    public PictureStep(ulong x, ulong y, ulong xSize, ulong ySize, string url, bool isPfp)
    {
        this.x = x;
        this.y = y;
        this.xSize = xSize;
        this.ySize = ySize;
        Url = url;
        IsPfp = isPfp;
    }

    public string Url { get; set; }
    public bool IsPfp { get; set; }

    [XmlIgnore] public SdImage ImageF { get; set; }

    public ulong xSize { get; set; }
    public ulong ySize { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public SdImage Image()
    {
        if (ImageF is null)
        {
            ImageF = new SdImage(Url);
        }

        return ImageF;
    }

    ~PictureStep()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            ImageF.Dispose();
        }

        Url = null;
        // Cleanup
    }
}