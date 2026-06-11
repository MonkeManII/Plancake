using PlancakeSerializer.Serialization;
using System.Text;

namespace Test.Serializers
{
    internal class StringSerializer : TypeSafeSerializer<string>
    {
        readonly Encoding _encoding;

        public StringSerializer(Encoding e)
        {
            _encoding = e;
        }

        public override string DeserializeSpecific(in DataDestructor des)
        {
            return _encoding.GetString(des.ReadNextBlock());
        }

        public override void SerializeSpecific(string obj, in DataConstructor con)
        {
            con.WriteBytes(_encoding.GetBytes(obj));
        }
    }
}
