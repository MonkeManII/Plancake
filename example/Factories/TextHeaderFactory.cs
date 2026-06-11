using PlancakeSerializer.Headers;
using System.Text;

namespace Example.Factories
{
    /// <summary>
    /// An example header factory for string headers.
    /// </summary>
    internal class TextHeaderFactory : IHeaderFactory<string>
    {
        public string FromHeader(Header header)
        {
            return Encoding.UTF8.GetString(header.Content);
        }

        public byte[] ToHeaderData(string obj)
        {
            return Encoding.UTF8.GetBytes(obj);
        }
    }
}
