using PlancakeSerializer;
using Test.TestLibrary;
using Test.Serializers;
using System.Text;
using PlancakeSerializer.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Test.UnitTests
{
    public class RandomStringSerializerTest : ITest
    {
        public void RunTest()
        {
            Test("Hello, world!");
            Test("WAKLHFASKLHF");
            Test("9876546887654!@$*(^!*@()$^!@)(*&%^$!@(*)&$%_()&!");
        }

        static void Test(string test)
        {
            GlobalSerializer s = new(new StringSerializer());
            byte[] bts = new byte[1024];
            
            using (MemoryStream stream = new(bts))
            {
                DataConstructor constructor = new(s, stream);

                constructor.WriteObject(test);
                
                constructor.Complete();
            }

            using (MemoryStream stream = new(bts))
            {
                DataDestructor destructor = new(s, stream);

                if (destructor.TryReadObject(out string? val))
                {
                    if (val != test)
                    {
                        throw new Exception($"String output was wrong. (Expected: \"{test}\", got \"{val}\")");
                    }
                } else
                {
                    throw new Exception($"Failed to read string. (Expected: \"{test}\")");
                }
            }
        }
    }
}
