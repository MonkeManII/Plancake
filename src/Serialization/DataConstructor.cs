using PlancakeSerializer.Headers;

namespace PlancakeSerializer.Serialization
{
    /// <summary>
    /// Creates a data packet from a series of objects and headers.
    /// </summary>
    public class DataConstructor
    {
        readonly static byte[] sizePlaceholder = new byte[sizeof(ushort)];
        readonly List<Header> _headers = [];
        readonly List<object> _objects = [];
        readonly GlobalSerializer _serializer;
        readonly Stream _outStream;

        /// <summary>
        /// Whether this constructor is "completed".
        /// </summary>
        /// <remarks>
        /// "Completed" constructors cannot have additional data written to them.
        /// </remarks>
        public bool IsComplete => _isComplete;
        bool _isComplete;

        /// <summary>
        /// The identifier for the currently-written object.
        /// </summary>
        /// <remarks>
        /// This can be used to generate unique headers for each
        /// serialized object if necessary.
        /// </remarks>
        public long CurrentWriteNum => _writeNum;
        long _writeNum;

        /// <summary>
        /// Creates a new <see cref="DataConstructor"/> from a <see cref="GlobalSerializer"/> and <see cref="Stream"/>.
        /// </summary>
        /// <param name="serializer">The set of serializers to use.</param>
        /// <param name="outStream">The stream to write the serialized data to.</param>
        public DataConstructor(GlobalSerializer serializer, Stream outStream)
        {
            _outStream = outStream;
            _serializer = serializer;
        }

        /// <summary>
        /// Writes a header to the constructor.
        /// </summary>
        /// <param name="header">The header to write.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void WriteHeader(Header header)
        {
            if (IsComplete) throw new InvalidOperationException($"Cannot call {nameof(WriteHeader)} on a complete {nameof(DataConstructor)}!");
            _headers.Add(header);
        }

        /// <summary>
        /// Writes an object to the constructor using the provided <see cref="GlobalSerializer"/>.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void WriteObject(object obj)
        {
            if (IsComplete) throw new InvalidOperationException($"Cannot call {nameof(WriteObject)} on a complete {nameof(DataConstructor)}!");
            _objects.Add(obj);
        }

        /// <summary>
        /// Writes a series of bytes to the constructor.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <remarks>
        /// This should not be used except when defining an <see cref="ISerializer"/>.
        /// Calling this directly (as you would <see cref="WriteObject(object)"/> or <see cref="WriteHeader(Header)"/>)
        /// can lead to unexpected outcomes.
        /// </remarks>
        public void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            _outStream.Write(bytes);
        }

        /// <summary>
        /// Finalizes the data packet and writes it to the stream.
        /// </summary>
        /// <remarks>
        /// Completed packets cannot have additional data written to them.
        /// </remarks>
        /// <exception cref="InvalidOperationException"></exception>
        public void Complete()
        {
            if (IsComplete) throw new InvalidOperationException($"Cannot call {nameof(Complete)} on an already-completed {nameof(DataConstructor)}!");

            _outStream.Write(BitConverter.GetBytes((ushort)_headers.Count));

            foreach (Header h in _headers)
            {
                h.WriteTo(_outStream);
            }

            _writeNum = 0;
            foreach (object o in _objects)
            {
                long preWritePos = _outStream.Position;

                if (!_serializer.TrySerialize(o, this, in sizePlaceholder, out int offset))
                {
                    throw new InvalidOperationException($"Cannot serialize object {o}, as there is no serializer for type '{o.GetType().Name}'!");
                }

                long lenIdx = preWritePos + offset;
                preWritePos = lenIdx + sizeof(ushort);
                long postWritePos = _outStream.Position;
                BitConverter.GetBytes((ushort)(postWritePos - preWritePos)).CopyTo(sizePlaceholder);

                _outStream.Position = lenIdx;
                _outStream.Write(sizePlaceholder);
                _outStream.Position = postWritePos;

                ++_writeNum;
            }

            _isComplete = true;
        }
    }
}
