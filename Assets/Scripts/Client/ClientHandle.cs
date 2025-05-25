using Unity.Collections;
using UnityEngine;

namespace NetGame.Client
{
    public class ClientHandle
    {
        public static void GetLoginResult(DataStreamReader reader)
        {
            LoginManager.Instance.LoginResult(reader.ReadBool());
        }

        public static void ReadInt(DataStreamReader reader)
        {
            Debug.Log("CLIENT: Number: " + reader.ReadInt());
        }

        public static void GetPosition(DataStreamReader reader)
        {
            Debug.Log("CLIENT: Position: " + reader.ReadVector3());
        }

        public static void GetKey(DataStreamReader reader)
        {
            Debug.Log("CLIENT: Key: " + reader.ReadFixedString32());
        }
    }
}