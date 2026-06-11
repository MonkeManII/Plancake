namespace PlancakeSerializer.Serialization
{
    /// <summary>
    /// Represents a serializer.
    /// </summary>
    /// <remarks>
    /// For a type-safe version of this, extend <see cref="TypeSafeSerializer{T}"/>.
    /// </remarks>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes the next object from a <see cref="DataDestructor"/>.
        /// </summary>
        /// <param name="des">The destructor to read from.</param>
        /// <returns>An object constructed from the data in the destructor.</returns>
        public object? Deserialize(in DataDestructor des);

        /// <summary>
        /// Serializes a given object to a <see cref="DataConstructor"/>.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="cons">The constructor to write the data to.</param>
        public void Serialize(object? obj, in DataConstructor cons);

        /// <summary>
        /// Gets the type that this serializer can properly serialize.
        /// </summary>
        /// <remarks>
        /// Dynamically changing this in its implementation can result in unexpected behaviour.
        /// </remarks>
        public Type SerializableType();
    }
}
