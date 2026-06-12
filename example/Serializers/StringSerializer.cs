using PlancakeSerializer.Serialization;
using System.Text;

namespace Example.Serializers
{
    /// <summary>
    /// An example serializer. Serializes UTF-8 strings.
    /// </summary>
    internal class StringSerializer : TypeSafeSerializer<string>
    {
        public override string DeserializeSpecific(in DataDestructor des)
        {
            return Encoding.UTF8.GetString(des.ReadNextBlock());
        }

        public override void SerializeSpecific(string obj, in DataConstructor con)
        {
            con.WriteByteBlock(Encoding.UTF8.GetBytes(obj));
        }
    }
}
