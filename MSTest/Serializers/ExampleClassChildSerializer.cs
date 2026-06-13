using PlancakeSerializer.Serialization;
using System.Text;

namespace Test.Serializers
{
    internal class ExampleClassChildSerializer : TypeSafeSerializer<ExampleClassChild>
    {
        public static readonly ExampleClassChildSerializer Serializer = new();

        ExampleClassChildSerializer() { }

        public override ExampleClassChild DeserializeSpecific(in DataDestructor des)
        {
            int foo = BitConverter.ToInt32(des.ReadNextBlock());
            int bar = BitConverter.ToInt32(des.ReadNextBlock());
            string fod = Encoding.UTF8.GetString(des.ReadNextBlock());
            return new(foo, bar, fod);
        }

        public override void SerializeSpecific(ExampleClassChild obj, in DataConstructor con)
        {
            con.WriteByteBlock(BitConverter.GetBytes(obj.Foo));
            con.WriteByteBlock(BitConverter.GetBytes(obj.Bar));
            con.WriteByteBlock(Encoding.UTF8.GetBytes(obj.Fod));
        }
    }
}
