using PlancakeSerializer;
using PlancakeSerializer.Headers;
using PlancakeSerializer.Serialization;
using Example.Factories;
using Example.Serializers;

namespace Example
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GlobalSerializer serializer = new(
                new StringSerializer()    
            );
            TextHeaderFactory headerFactory = new();

            using (FileStream fs = new("test.mbt", FileMode.OpenOrCreate, FileAccess.Write))
            {
                // DebugStreams output all of their actions to the console.
                //DebugStream stream = new(fs);
                FileStream stream = fs;

                DataConstructor constructor = new(serializer, stream);

                // Append headers
                Header h1 = Header.FromData("SERVER-ORIGIN", "https://example.com", headerFactory);
                constructor.WriteHeader(h1);

                Header h2 = Header.FromData("PLAYER-ORIGIN", "Player1", headerFactory);
                constructor.WriteHeader(h2);

                // Write strings.
                // Only possible because 'serializer' includes a 'StringSerializer'
                constructor.WriteObject("Hello, world!");
                constructor.WriteObject("How's it going?");

                // Complete the packet.
                constructor.Complete();
            }

            using (FileStream fs = new("test.mbt", FileMode.OpenOrCreate, FileAccess.Read))
            {
                // DebugStreams output all of their actions to the console.
                //DebugStream stream = new(fs);
                FileStream stream = fs;

                DataDestructor des = new(serializer, stream);

                if (des.TryGetHeader("SERVER-ORIGIN", out Header? header))
                {
                    Console.WriteLine("Message server origin header: {0}", headerFactory.FromHeader(header));
                } else
                {
                    Console.WriteLine("No message origin header! :(");
                }

                if (des.TryGetHeader("PLAYER-ORIGIN", out header))
                {
                    Console.WriteLine("Message player origin header: {0}", headerFactory.FromHeader(header));
                }
                else
                {
                    Console.WriteLine("No message origin header! :(");
                }

                while (!des.IsComplete())
                {
                    if (des.TryReadObject(out string? value))
                    {
                        Console.WriteLine(value);
                    }
                }
            }
        }
    }
}
