using PlancakeSerializer.BuiltIns;

namespace PlancakeSerializer.Headers
{
    /// <summary>
    /// Represents <see cref="byte"/> data appended to the beginning of the file.
    /// </summary>
    /// <remarks>
    /// Can be written to at any point during serialization, and read at any
    /// point during deserialization.
    /// </remarks>
    public sealed class Header
    {
        /// <summary>
        /// The raw byte content of the header.
        /// </summary>
        public ReadOnlySpan<byte> Content => payload;
        
        internal const int NAME_HASH_CST = 3;

        readonly byte[] length = new byte[1];
        readonly byte[] payload;
        readonly long nameHash;
        readonly byte[] hashBytes = new byte[sizeof(long)];

        // data structure:
        // bytes 0-7 are the name hash
        // bytes 8 is the length 'n'
        // byte(s) 9-8+n is/are the payload


        Header(Stream stream)
        {
            stream.ReadExactly(hashBytes);
            nameHash = BitConverter.ToInt64(hashBytes);
            stream.ReadExactly(length);
            payload = new byte[length[0]];
            stream.ReadExactly(payload); 
        }

        Header(byte[] data, long nameHash)
        {
            this.nameHash = nameHash;
            hashBytes = BitConverter.GetBytes(nameHash);
            length[0] = (byte)data.Length;
            payload = data;
        }

        internal static Header FromStream(Stream s)
        {
            return new Header(s);
        }

        /// <summary>
        /// Creates a <see cref="Header"/> from a <typeparamref name="TResult"/> and a <see cref="IHeaderFactory{TResult}"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of object to convert to a header.</typeparam>
        /// <param name="name">The name ID of the header.</param>
        /// <param name="data">The data ("payload") of the header.</param>
        /// <param name="converter">The <see cref="IHeaderFactory{TResult}"/> used to convert the payload to byte data.</param>
        /// <returns>A <see cref="Header"/> constructed from the provided <typeparamref name="TResult"/>.</returns>
        public static Header FromData<TResult>(string name, TResult data, IHeaderFactory<TResult> converter)
        {
            byte[] converted = converter.ToHeaderData(data);
            return new Header(converted, StringHasher.HashString(name, NAME_HASH_CST));
        }

        internal void WriteTo(Stream s)
        {
            s.Write(BitConverter.GetBytes(nameHash));
            s.Write(length);
            s.Write(payload);
        }

        internal long LongHash() => nameHash;
    }
}
