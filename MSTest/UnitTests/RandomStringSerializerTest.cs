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
        public bool RunTest(out string? message)
        {
            if (!Test("Hello, world!", out message)) return false;
            if (!Test("WAKLHFASKLHF", out message)) return false;
            if (!Test("9876546887654!@$*(^!*@()$^!@)(*&%^$!@(*)&$%_()&!", out message)) return false;
            return true;
        }

        static bool Test(string test, [NotNullWhen(false)] out string? message)
        {
            GlobalSerializer s = new(new StringSerializer(Encoding.UTF8));
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
                    message = null;
                    return true;
                } else
                {
                    message = "Failed to read string. (Expected: " + test + ")";
                    return false;
                }
            }
        }
    }
}
