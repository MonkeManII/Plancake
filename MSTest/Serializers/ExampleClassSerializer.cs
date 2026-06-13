using PlancakeSerializer.Serialization;

namespace Test.Serializers
{
    internal class ExampleClassSerializer : TypeSafeSerializer<ExampleClass>
    {
        public static readonly ExampleClassSerializer Serializer = new();

        ExampleClassSerializer() { }

        public override ExampleClass DeserializeSpecific(in DataDestructor des)
        {
            int foo = BitConverter.ToInt32(des.ReadNextBlock());
            int bar = BitConverter.ToInt32(des.ReadNextBlock());
            return new(foo, bar);
        }

        public override void SerializeSpecific(ExampleClass obj, in DataConstructor con)
        {
            con.WriteByteBlock(BitConverter.GetBytes(obj.Foo));
            con.WriteByteBlock(BitConverter.GetBytes(obj.Bar));
        }
    }
}
