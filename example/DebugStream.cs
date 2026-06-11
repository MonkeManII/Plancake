
namespace Example
{
    /// <summary>
    /// Wraps a <see cref="Stream"/>, and outputs all of its actions to the console.
    /// </summary>
    public class DebugStream : Stream
    {
        static string SpanToString<T>(Span<T> span)
        {
            string r = "[";
            foreach (T item in span)
            {
                r += (item?.ToString() ?? "") + ", ";
            }
            r += "]";
            return r;
        }

        readonly Stream bts;

        public DebugStream(Stream stream)
        {
            bts = stream;
        }

        public override bool CanRead => bts.CanRead;

        public override bool CanSeek => bts.CanSeek;

        public override bool CanWrite => bts.CanWrite;

        public override long Length => bts.Length;

        public override long Position { get => bts.Position; set { Console.WriteLine("Jump to {0}", value); bts.Position = value; } }

        public override void Flush() => bts.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            int o = bts.Read(buffer, offset, count);
            Console.WriteLine("Read {0} bytes: {1}", o.ToString(), SpanToString(buffer.AsSpan()[0..count]));
            return o;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long o = bts.Seek(offset, origin);
            Console.WriteLine("Sought to {0}", o.ToString());
            return o;
        }

        public override void SetLength(long value)
        {
            bts.SetLength(value);
            Console.WriteLine("Set length to {0}", value.ToString());
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            bts.Write(buffer, offset, count);
            Console.WriteLine("Wrote bytes: {0}", SpanToString(buffer.AsSpan()[0..count]));
        }
    }
}
