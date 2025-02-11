using Unity.Collections;
using Unity.Netcode;

//https://youtu.be/rFCFMkzFaog?list=PLQMQNmwN3FvyyeI1-bDcBPmZiSaDMbFTi
public struct NetworkString : INetworkSerializable //para poder usar string en las networkvariables y que no den problemas de serialización
{
    private FixedString32Bytes info;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref info);
    }

    public override string ToString()
    {
        return info.ToString();
    }

    public static implicit operator string(NetworkString s) => s.ToString();
    public static implicit operator NetworkString(string s) => new NetworkString() { info = new FixedString32Bytes(s) };
}
