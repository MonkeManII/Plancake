using PlancakeSerializer.Serialization;
using System.Text;

namespace Test.Serializers
{
    internal class StringSerializer : TypeSafeSerializer<string>
    {
        Encoding? _encoding;

        public StringSerializer(StringEncoding e)
        {
            SetEncoding(e);
        }

        public StringSerializer() : this(StringEncoding.UTF8) { }

        public void SetEncoding(StringEncoding e)
        {
            _encoding = e switch
            {
                StringEncoding.ASCII => Encoding.ASCII,
                StringEncoding.UTF8 => Encoding.UTF8,
                StringEncoding.UTF16 => Encoding.Unicode,
                StringEncoding.UTF32 => Encoding.UTF32,
                _ => Encoding.Unicode
            };
        }

        public override string DeserializeSpecific(in DataDestructor des)
        {
            if (_encoding is null) throw new NullReferenceException($"{nameof(_encoding)} is null!");
            return _encoding.GetString(des.ReadNextBlock());
        }

        public override void SerializeSpecific(string obj, in DataConstructor con)
        {
            if (_encoding is null) throw new NullReferenceException($"{nameof(_encoding)} is null!");
            con.WriteByteBlock(_encoding?.GetBytes(obj));
        }
    }
}
