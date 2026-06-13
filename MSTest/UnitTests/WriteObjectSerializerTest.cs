using PlancakeSerializer;
using PlancakeSerializer.Serialization;
using Test.Serializers;
using Test.TestLibrary;

namespace Test.UnitTests
{
    internal class WriteObjectSerializerTest : ITest
    {
        public void RunTest()
        {
            GlobalSerializer s = new(ExampleClassSerializerInnerObject.Serializer, IntSerializer.Serializer);

            using (Stream fs = File.OpenWrite("garbage1"))
            {
                DataConstructor cons = new(s, fs);
                ExampleClass obj1 = new(100, 100);
                cons.WriteObject(obj1);
                cons.Complete();
            }

            using (Stream fs = File.OpenRead("garbage1"))
            {
                DataDestructor destructor = new(s, fs);

                if (!destructor.TryReadObject(out ExampleClass? c))
                {
                    throw new Exception("Failed to read object!");
                }

                if (!(c.Foo == 100 && c.Bar == 100)) throw new Exception($"Read object, but object output wrong values!\n(Expected: 100, 100, got: {c.Foo}, {c.Bar})");
            }
        }
    }
}
