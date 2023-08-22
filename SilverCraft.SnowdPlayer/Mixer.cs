using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SilverCraft.SnowdPlayer
{
    public class MixedStream : Stream
    {
        public Stream RealBackend { get; set; }
        public bool Real { get; set; } = true;
        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Real)
            {
                return RealBackend.Read(buffer, offset, count);
            }
            Array.Clear(buffer, offset, count);
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return RealBackend.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();

        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead => RealBackend.CanRead;
        public override bool CanSeek => RealBackend.CanSeek;
        public override bool CanWrite => false;
        public override long Length=> RealBackend.Length;
        public override long Position { get=> RealBackend.Position; set=>RealBackend.Position=value; }
    }
}