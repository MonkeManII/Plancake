using PlancakeSerializer.Serialization;

namespace Test.Serializers
{
    internal class IntSerializer : TypeSafeSerializer<int>
    {
        public static readonly IntSerializer Serializer = new();

        public override int DeserializeSpecific(in DataDestructor des)
        {
            return BitConverter.ToInt32(des.ReadRaw(sizeof(int)));
        }

        public override void SerializeSpecific(int obj, in DataConstructor con)
        {
            con.WriteRaw(BitConverter.GetBytes(obj));
        }
    }
}
