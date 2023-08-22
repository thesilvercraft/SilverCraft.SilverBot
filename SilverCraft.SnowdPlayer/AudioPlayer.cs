using System.Diagnostics;
using System.Threading.Channels;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using SilverAudioPlayer.Shared;
using HttpClient = System.Net.Http.HttpClient;

namespace SilverCraft.SnowdPlayer;

public class AudioPlayer : IPlay
{
    public AudioPlayer(VoiceNextConnection connection, HttpClient client)
    {
        Connection = connection;
        Guild = connection.TargetChannel.Guild;
        Client = client;
        Sink = connection.GetTransmitSink();
    }

    public HttpClient Client { get; set; }
    public DiscordGuild Guild { get; set; }
    public VoiceNextConnection Connection { get; set; }
    public Stream FFmpegStream { get; set; }
    public Task RunningCopyToStream { get; set; }
    public Stream InputStream { get; set; }
    public VoiceTransmitSink Sink { get; set; }
    public CancellationToken Stopper { get; set; }
    public CancellationTokenSource StopperSource { get; set; }

    public async Task CopyToStream()
    {
        if (FFmpegStream != null && Sink != null)
        {
            if (FFmpegStream.CanRead)
            {
                _state = PlaybackState.Playing;
                FFmpegProcess.Exited += (_,_) =>
                {
                    StopperSource.Cancel();
                    StopperSource = null;
                };
                await FFmpegStream.CopyToAsync(Sink, cancellationToken:Stopper);
                
                Stop();
            }
        }

    }

    private IProgress<float> ProgressWhenKnown { get; set; }
    private PlaybackState _state = PlaybackState.Stopped;

    public  void LoadMedia(string URL)
    {
        LoadedMedia = URL;
    }

    private string LoadedMedia;
    Process FFmpegProcess;
    public async void Play()
    {
        if (_state == PlaybackState.Stopped)
        {
            if (string.IsNullOrEmpty(LoadedMedia))
            {
                return;
            }

            var i = await Client.GetStreamAsync(LoadedMedia);
            WrappedHttpStream stream = new(LoadedMedia);
            Song song = new(stream, LoadedMedia, new Guid());
            if (i.CanSeek)
            {
                var mt = SilverMagicBytes.MagicByteCombos.Match(i, 0);
                InputStream = i;

            }

            var (ffstream, b) = FFMPEGConverter.GetOutput(InputStream);
            FFmpegProcess = b;
            FFmpegStream = ffstream;
            StopperSource = new();
            Stopper = StopperSource.Token;
            RunningCopyToStream = Task.Run(async ()=> await CopyToStream());
        }
    }

    public void Stop()
    {
        if (_state == PlaybackState.Stopped)
        {
            return;
        }

        if (StopperSource != null)
        {
            StopperSource.Cancel();

        }
        InputStream = null;
        FFmpegStream.Dispose();
        RunningCopyToStream = null;
        TrackEnd?.Invoke(this,this);
        _state = PlaybackState.Stopped;
    }

    public void Pause()
    {
        if (_state != PlaybackState.Playing)
        {
            return;
        }
        Sink.Pause();
        TrackPause?.Invoke(this, this);
        _state = PlaybackState.Paused;
    }

    public async void Resume()
    {
        if (_state != PlaybackState.Paused)
        {
            return;
        }
        await Sink.ResumeAsync();
        _state = PlaybackState.Playing;
    }

    public uint? ChannelCount()
    {
        throw new NotImplementedException();
    }

    public void SetVolume(byte volume)
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetPosition()
    {
        throw new NotImplementedException();
    }

    public void SetPosition(TimeSpan position)
    {
        if (InputStream.CanSeek)
        {
            //TODO magic
            //find AverageBytesPerSecond somehow
            InputStream.Position = (long)(position.TotalSeconds );
        }
    }

    public PlaybackState? GetPlaybackState()
    {
        return _state;
    }

    public TimeSpan? Length()
    {
        throw new NotImplementedException();
    }

    public long? GetSampleRate()
    {
        return null;
    }

    public uint? GetBitsPerSample()
    {
        return null;
    }

    public event EventHandler<object>? TrackEnd;
    public event EventHandler<object>? TrackPause;
}