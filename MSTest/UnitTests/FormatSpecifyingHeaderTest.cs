using PlancakeSerializer;
using PlancakeSerializer.Headers;
using PlancakeSerializer.Serialization;
using Test.Headers;
using Test.Serializers;
using Test.TestLibrary;

namespace Test.UnitTests
{
    internal class FormatSpecifyingHeaderTest : ITest
    {
        const string TEST_STR = "Hello, World! This is a string encoding test!";
        const string HEADER_NAME = "StringTypeHeader";

        public void RunTest()
        {
            foreach (StringEncoding e in Enum.GetValues<StringEncoding>())
            {
                Test(e);
            }
        }

        static void Test(StringEncoding e)
        {
            StringEncodingHeaderFactory hFac = new();

            using (Stream fs = File.OpenWrite("garbage2"))
            {
                Header h = Header.FromData(HEADER_NAME, e, hFac);
                GlobalSerializer s = new(new StringSerializer(e));
                DataConstructor cons = new(s, fs);
                cons.WriteHeader(h);
                cons.WriteObject(TEST_STR);
                cons.Complete();
            }

            using (Stream fs = File.OpenRead("garbage2"))
            {
                StringSerializer strser = new StringSerializer(e);
                GlobalSerializer s = new(strser);
                DataDestructor des = new(s, fs);
                if (!des.TryGetHeader(HEADER_NAME, out Header? h)) throw new Exception($"Header {HEADER_NAME} not found!");
                strser.SetEncoding(hFac.FromHeader(h));
                des.TryReadObject(out string? str);
                if (str != TEST_STR) throw new Exception($"Read string \"{str}\", expected \"{TEST_STR}\"!");
            }
        }
    }
}
