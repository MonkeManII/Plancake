using PlancakeSerializer;
using PlancakeSerializer.Serialization;
using Test.Serializers;
using Test.TestLibrary;

namespace Test.UnitTests
{
    internal class InheritanceSerializerTest : ITest
    {
        public bool RunTest(out string? error)
        {
            GlobalSerializer s = new(ExampleClassSerializer.Serializer, ExampleClassChildSerializer.Serializer);

            using (Stream fs = File.OpenWrite("garbage0"))
            {
                DataConstructor cons = new(s, fs);
                ExampleClass obj1 = new(100, 100);
                ExampleClassChild child1 = new(30, 30, "Hello, World!");

                cons.WriteObject(obj1);
                cons.WriteObject(child1);
                cons.Complete();
            }

            using (Stream fs = File.OpenRead("garbage0"))
            {
                DataDestructor des = new(s, fs);
                if (!des.TryReadObject(out ExampleClass? val))
                {
                    error = "Failed to read object 1 (parent type) from constructed data.";
                }
                if (!des.TryReadObject(out ExampleClassChild? val2))
                {
                    error = "Failed to read object 2 (child type) from constructed data.";
                }
            }

            error = null;
            return true;
        }
    }
}
