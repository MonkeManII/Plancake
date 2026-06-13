using PlancakeSerializer.Headers;
using Test.Serializers;

namespace Test.Headers
{
    internal class StringEncodingHeaderFactory : IHeaderFactory<StringEncoding>
    {
        public StringEncoding FromHeader(Header header)
        {
            return (StringEncoding)header.Content[0];
        }

        public byte[] ToHeaderData(StringEncoding obj)
        {
            return [ (byte)obj ];
        }
    }
}
