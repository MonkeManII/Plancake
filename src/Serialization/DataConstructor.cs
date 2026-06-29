using PlancakeSerializer.Headers;
using System.Diagnostics;

namespace PlancakeSerializer.Serialization
{
    /// <summary>
    /// Creates a data packet from a series of objects and headers.
    /// </summary>
    public class DataConstructor
    {
        enum WriteState
        {
            Headers,
            Objects
        }

        readonly GlobalSerializer _serializer;
        readonly Stream _outStream;
        readonly List<Header> _headers = [];

        /// <summary>
        /// Whether this constructor is "completed".
        /// </summary>
        /// <remarks>
        /// "Completed" constructors cannot have additional data written to them.
        /// </remarks>
        [Obsolete($"Constructors have no \"complete\" state anymore, so this will always return false.")]
        public bool IsComplete => false;

        /// <summary>
        /// The identifier for the currently-written object.
        /// </summary>
        /// <remarks>
        /// This can be used to generate unique headers for each
        /// serialized object if necessary.
        /// </remarks>
        public long CurrentWriteNum => _writeNum;
        long _writeNum;

        WriteState _state;

        /// <summary>
        /// Creates a new <see cref="DataConstructor"/> from a <see cref="GlobalSerializer"/> and <see cref="Stream"/>.
        /// </summary>
        /// <param name="serializer">The set of serializers to use.</param>
        /// <param name="outStream">The stream to write the serialized data to.</param>
        public DataConstructor(GlobalSerializer serializer, Stream outStream)
        {
            _outStream = outStream;
            _serializer = serializer;
            _state = WriteState.Headers;
        }

        void RequireState(WriteState state, string errorName)
        {
            if (_state <= WriteState.Headers && state > WriteState.Headers)
            {
                WriteHeaders();
            }

            if (_state <= state) _state = state;
            else throw new InvalidOperationException($"{errorName} (expected <={state}, got {_state}");
        }

        void WriteHeaders()
        {
            _outStream.Write(BitConverter.GetBytes((ushort)_headers.Count));

            foreach (Header h in _headers)
            {
                h.WriteTo(_outStream);
            }
        }

        /// <summary>
        /// Writes a header to the constructor.
        /// </summary>
        /// <remarks>
        /// While headers can be <em>read</em> at any time during deserialization, they must be <em>written</em> before
        /// writing any objects.
        /// </remarks>
        /// <param name="header">The header to write.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void WriteHeader(Header header)
        {
            RequireState(WriteState.Headers, $"Cannot call {nameof(WriteHeader)} after writing an object!");
            _headers.Add(header);
        }

        /// <summary>
        /// Writes an object to the constructor using the provided <see cref="GlobalSerializer"/>.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void WriteObject(object obj)
        {
            RequireState(WriteState.Objects, $"Cannot call {nameof(WriteObject)} in this state!");
            if (!_serializer.TrySerialize(obj, this))
            {
                throw new InvalidOperationException($"Cannot serialize object {obj}, as there is no serializer for type '{obj.GetType().Name}'!");
            }
            ++_writeNum;
        }

        /// <summary>
        /// Writes a block of bytes to the constructor, readable via <see cref="DataDestructor.ReadNextBlock"/>.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <para>
        /// This should not be used except when defining an <see cref="ISerializer"/>.
        /// Calling this directly (as you would <see cref="WriteObject(object)"/> or <see cref="WriteHeader(Header)"/>)
        /// can lead to unexpected outcomes. 
        /// </para>
        /// <para>
        /// Note that <see cref="DataDestructor.ReadRaw(int)"/> will not recognize this. Use <see cref="DataDestructor.ReadNextBlock"/>
        /// to read a whole block of variable-width data.
        /// </para>
        /// </remarks>
        public void WriteByteBlock(ReadOnlySpan<byte> bytes)
        {
            _outStream.Write(BitConverter.GetBytes((ushort)bytes.Length));
            _outStream.Write(bytes);
        }

        /// <summary>
        /// Writes a raw series of bytes to the constructor, readable via <see cref="DataDestructor.ReadRaw(int)">.
        /// </summary>
        /// <param name="bytes">The bytes to write.</param>
        /// <remarks>
        /// <para>
        /// This should not be used except when defining an < see cref="ISerializer"/>.
        /// Calling this directly (as you would <see cref="WriteObject(object)"/> or <see cref="WriteHeader(Header)"/>)
        /// can lead to unexpected outcomes. 
        /// </para>
        /// <para>
        /// Note that <see cref="DataDestructor.ReadNextBlock"/> will not recognize this. Use <see cref="DataDestructor.ReadRaw(int)"/>
        /// to read n bytes as raw data.
        /// </para>
        /// </remarks>
        public void WriteRaw(ReadOnlySpan<byte> bytes)
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
        [Obsolete("Calling \"Complete\" to finish a packet is no longer necessary, as packets are constructed on-the-fly.")]
        public void Complete() { }
    }
}
