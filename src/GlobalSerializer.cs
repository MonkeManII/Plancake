using PlancakeSerializer.BuiltIns;
using PlancakeSerializer.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace PlancakeSerializer
{
    /// <summary>
    /// A collection of <see cref="ISerializer"/>s that are used to serialize and
    /// deserialize any object, provided it has an associated serializer.
    /// </summary>
    public class GlobalSerializer
    {
        readonly InheritanceHash _serializerHashes;
        readonly Dictionary<Type, ISerializer> _serializers;

        /// <summary>
        /// Creates a new <see cref="GlobalSerializer"/> with the given set of serializers.
        /// </summary>
        /// <param name="serializers">The serializers to use. There must not be any type overlap between them.</param>
        /// <exception cref="Exception"></exception>
        public GlobalSerializer(params ISerializer[] serializers)
        {
            Type[] hashedTypes = new Type[serializers.Length];

            _serializers = new(serializers.Length);
            foreach (ISerializer s in serializers)
            {
                hashedTypes[_serializers.Count] = s.SerializableType();
                if (!_serializers.TryAdd(s.SerializableType(), s))
                {
                    throw new Exception($"Cannot add multiple serializers for one type! (Type: {s.SerializableType()})");
                }
            }

            _serializerHashes = new(hashedTypes);
        }

        internal bool TrySerialize(object obj, in DataConstructor str, in byte[] reserve, out int reserveOffset)
        {
            reserveOffset = 0;
            if (_serializers.TryGetValue(obj.GetType(), out ISerializer? s))
            {
                if (!_serializerHashes.TryGetHash(obj.GetType(), out long hash))
                    throw new Exception($"Object type {s.GetType()} is unhashed! Please report this error to the Github repo!");
                byte[] hashBytes = BitConverter.GetBytes(hash);
                str.WriteBytes(hashBytes);
                reserveOffset = hashBytes.Length;
                str.WriteBytes(reserve);
                s.Serialize(obj, str);
                return true;
            }
            return false;
        }

        internal bool TryDeserialize(
            in DataDestructor des,
            [NotNullWhen(true)] out object? deserialized,
            [NotNullWhen(true)] out Type? objectType
        )
        {
            deserialized = null;

            // Object type hash
            long typeHash = BitConverter.ToInt64(des.ReadExactly(sizeof(long)));
            
            if (
                !_serializerHashes.TryGetType(typeHash, out objectType) ||
                !_serializers.TryGetValue(objectType, out ISerializer? s)
            )
            {
                return false;
            }

            deserialized = s.Deserialize(des);
            if (deserialized is null) return false;
            return true;
        }
    }
}
