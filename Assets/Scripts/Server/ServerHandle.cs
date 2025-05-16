using Unity.Collections;
using UnityEngine;

namespace NetGame.Server
{
    public class ServerHandle
    {
        public static void GetKey(DataStreamReader reader, int client)
        {
            KeyCode key = (KeyCode)reader.ReadInt();

            ServerSend.SendKeyString(key.ToString(), client);
        }
    }
}