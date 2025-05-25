using Unity.Collections;
using UnityEngine;

public static class DataStreamWriterExtensions
{
    public static void WriteBool(this ref DataStreamWriter writer, bool value)
    {
        writer.WriteByte((byte)(value ? 1 : 0));
    }

    public static void WriteVector2(this ref DataStreamWriter writer, Vector2 vector2)
    {
        writer.WriteFloat(vector2.x);
        writer.WriteFloat(vector2.y);
    }

    public static void WriteVector3(this ref DataStreamWriter writer, Vector3 vector3)
    {
        writer.WriteFloat(vector3.x);
        writer.WriteFloat(vector3.y);
        writer.WriteFloat(vector3.z);
    }
}

public static class DataStreamReaderExtensions
{
    public static bool ReadBool(this ref DataStreamReader reader)
    {
        return reader.ReadByte() != 0;
    }

    public static Vector2 ReadVector2(this ref DataStreamReader reader)
    {
        return new Vector2(reader.ReadFloat(), reader.ReadFloat());
    }

    public static Vector3 ReadVector3(this ref DataStreamReader reader)
    {
        return new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
    }
}
