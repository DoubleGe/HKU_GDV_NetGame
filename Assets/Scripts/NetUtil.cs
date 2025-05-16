using Unity.Collections;
using UnityEngine;

public static class NetUtil
{
    public static void WriteVector3(Vector3 vector3, ref DataStreamWriter writer)
    {
        writer.WriteFloat(vector3.x);
        writer.WriteFloat(vector3.y);
        writer.WriteFloat(vector3.z);
    }

    public static Vector3 ReadVector3(ref DataStreamReader reader)
    {
        return new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
    }
}
