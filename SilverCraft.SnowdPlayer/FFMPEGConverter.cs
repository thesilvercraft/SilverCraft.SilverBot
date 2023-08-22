using System.Diagnostics;
namespace SilverCraft.SnowdPlayer
{
    public static class FFMPEGConverter
    {
        public static Tuple<Stream, Process> GetOutput(Stream inputStream, string additionalArguments= "-ac 2 -f s16le -ar 48000")
        {
            var psi = new ProcessStartInfo("ffmpeg", $"-i - {additionalArguments} -")
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var p = Process.Start(psi);
            if (p is null)
            {
                throw new NullReferenceException("Starting the ffmpeg process resulted in a null");
            }
            _ = Task.Run(async ()=>
            {
                await inputStream.CopyToAsync(p.StandardInput.BaseStream);
                await p.StandardInput.BaseStream.FlushAsync();
                p.StandardInput.BaseStream.Close();
            }) ;
            return new Tuple<Stream, Process>(p.StandardOutput.BaseStream, p);
        }
    }
}