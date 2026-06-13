using PlancakeSerializer.Serialization;

namespace Test.Serializers
{
    internal class ExampleClassSerializerInnerObject : TypeSafeSerializer<ExampleClass>
    {
        public static readonly ExampleClassSerializerInnerObject Serializer = new();

        ExampleClassSerializerInnerObject() { }

        public override ExampleClass DeserializeSpecific(in DataDestructor des)
        {
            if (des.TryReadObject(out int foo) && des.TryReadObject(out int bar))
            {
                return new(foo, bar);
            }
            throw new InvalidDataException($"Could not deserialize a {SerializableType().Name} from the provided data stream!");
        }

        public override void SerializeSpecific(ExampleClass obj, in DataConstructor con)
        {
            con.WriteObject(obj.Foo);
            con.WriteObject(obj.Bar);
        }
    }
}
