using PlancakeSerializer.Headers;
using System.Diagnostics;

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
        bool _isCompleting;

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
        /// <remarks>
        /// While headers can be <em>read</em> at any time during deserialization, they must be written before
        /// calling <see cref="Complete"></see>.
        /// </remarks>
        /// <TODO>
        /// Allow writing of headers during a Complete() call.
        /// </TODO>
        /// <param name="header">The header to write.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void WriteHeader(Header header)
        {
            if (IsComplete) throw new InvalidOperationException($"Cannot call {nameof(WriteHeader)} on a complete {nameof(DataConstructor)}!");
            if (_isCompleting) throw new InvalidOperationException($"Cannot call {nameof(WriteHeader)} during a call to {nameof(Complete)}!");
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
            if (_isCompleting) DirectWrite(obj);
            else _objects.Add(obj);
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

        void DirectWrite(object o)
        {
            if (!_serializer.TrySerialize(o, this))
            {
                throw new InvalidOperationException($"Cannot serialize object {o}, as there is no serializer for type '{o.GetType().Name}'!");
            }
            ++_writeNum;
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
            _isCompleting = true;

            _outStream.Write(BitConverter.GetBytes((ushort)_headers.Count));

            foreach (Header h in _headers)
            {
                h.WriteTo(_outStream);
            }

            _writeNum = 0;
            foreach (object o in _objects)
            {
                DirectWrite(o);
            }

            _isComplete = true;
        }
    }
}
