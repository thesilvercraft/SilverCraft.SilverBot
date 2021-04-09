using SilverBotDS.Commands;
using SilverBotDS.Converters;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SilverBotDS.Objects
{
    internal class ImageSteps : IDisposable
    {
        public Step[] steps;
        private bool disposedValue;

        public static async Task<ImageSteps> Create(string url, HttpClient client)
        {
            var serializer = new XmlSerializer(typeof(Step[]), "SilverBotDS.Objects");
            var rm = await client.GetAsync(url);
            var ist = new ImageSteps();
            if (rm.StatusCode == HttpStatusCode.OK)
            {
                using StringReader stringReader = new(await rm.Content.ReadAsStringAsync());
                ist.steps = (Step[])serializer.Deserialize(stringReader);
            }
            else
            {
                throw new Exception($"Request yielded a status code that isn't OK it is {rm.StatusCode}");
            }
            return ist;
        }

        public async Task<Bitmap> ExecuteStepsAsync(Step[] filledsteps)
        {
            Bitmap Bitmap = null;
            Graphics graphics = null;
            foreach (var step in filledsteps)
            {
                if (step is TemplateStep step2)
                {
                    if (Bitmap is null)
                    {
                        Bitmap = new Bitmap(step2.Image());
                        graphics = Graphics.FromImage(Bitmap);
                    }
                    else
                    {
                        graphics.DrawImage(step2.Image(), new Point(0, 0));
                    }
                }
                else if (step is PictureStep step1)
                {
                    using var resizedbytes = ImageModule.Resize(await step1.Image().GetBytesAsync(), new Size((int)step1.xSize, (int)step1.ySize));
                    using var resizedimg = new Bitmap(resizedbytes);
                    graphics.DrawImage(resizedimg, new Point((int)step.x, (int)step.y));
                }
                else
                {
                    //bug oh no
                }
            }
            graphics.Flush();
            graphics.Save();
            return Bitmap;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    for (int i = 0; i < steps.Length; i++)
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
                disposedValue = true;
            }
        }

        ~ImageSteps()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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

        public ulong x;
        public ulong y;
    }

    public class TemplateStep : Step, IDisposable
    {
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

        [XmlIgnore]
        private Bitmap _image = null;

        public Bitmap Image()
        {
            if (_image is null)
            {
                if (isUrl)
                {
                    using var memorystream = new MemoryStream(new SdImage(template).GetBytesAsync().GetAwaiter().GetResult());
                    _image = new(memorystream);
                }
                else
                {
                    using var FileStream = new FileStream(template, FileMode.Open);
                    _image = new(FileStream);
                }
            }
            return _image;
        }

        public string template;
        public bool isUrl;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TemplateStep()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ((IDisposable)_image).Dispose();
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
            this.url = url;
            this.isPfp = isPfp;
        }

        public string url;
        public bool isPfp;

        [XmlIgnore]
        public SdImage _image;

        public SdImage Image()
        {
            if (_image is null)
            {
                _image = new SdImage(url);
            }
            return _image;
        }

        public ulong xSize;
        public ulong ySize;

        ~PictureStep()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _image.Dispose();
            }
            url = null;
            // Cleanup
        }
    }
}