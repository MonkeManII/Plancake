namespace PlancakeSerializer.Serialization
{
    public abstract class TypeSafeSerializer<T> : ISerializer
    {
        private readonly static Type _serializable = typeof(T);

        public object? Deserialize(in DataDestructor des)
        {
            return DeserializeSpecific(des);
        }

        /// <summary>
        /// A type-safe implementation of <see cref="ISerializer.Deserialize(in DataDestructor)"/>.
        /// </summary>
        public abstract T DeserializeSpecific(in DataDestructor des);

        public void Serialize(object? obj, in DataConstructor con)
        {
            if (obj is T t)
            {
                SerializeSpecific(t, in con);
            } else
            {
                throw new ArgumentException($"Passed an object of type {obj?.GetType().Name} into {nameof(Serialize)} instead of {typeof(T).Name}!");
            }
        }

        /// <summary>
        /// A type-safe implementation of <see cref="ISerializer.Serialize(object?, in DataConstructor)"/>.
        /// </summary>
        public abstract void SerializeSpecific(T obj, in DataConstructor con);

        public Type SerializableType() => _serializable;
    }
}
