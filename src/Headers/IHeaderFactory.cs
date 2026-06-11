namespace PlancakeSerializer.Headers
{
    /// <summary>
    /// Represents a <see cref="Header"/> factory.
    /// </summary>
    /// <typeparam name="TResult">The object to convert the <see cref="Header"/> into.</typeparam>
    public interface IHeaderFactory<TResult>
    {
        /// <summary>
        /// Converts a <see cref="Header"/> into a <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="header">The header to convert.</param>
        /// <returns>The resulting <typeparamref name="TResult"/>.</returns>
        public TResult FromHeader(Header header);

        /// <summary>
        /// Converts a <typeparamref name="TResult"/> into raw <see cref="Header"/> byte data.
        /// </summary>
        /// <param name="obj">The <typeparamref name="TResult"/> to convert.</param>
        /// <returns>A series of raw header bytes.</returns>
        public byte[] ToHeaderData(TResult obj);
    }
}
