using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

namespace NetGame.Server
{
    public class ServerSend : MonoBehaviour
    {
        public static void SendLoginResult(bool result, int client)
        {
            DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
            writer.WriteByte((byte)ServerNetPacket.SEND_LOGIN_RESULT);

            //Data
            writer.WriteBool(result);

            ServerBehaviour.Instance.EndStream(writer);
        }

        public static void SendInt(int value, int client)
        {
            DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
            writer.WriteByte((byte)ServerNetPacket.TEMP_SEND_NUMBER);

            //Data
            writer.WriteInt(value);

            ServerBehaviour.Instance.EndStream(writer);
        }

        public static void SendKeyString(string key, int client)
        {
            DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
            writer.WriteByte((byte)ServerNetPacket.TEMP_SEND_KEYSTRING);

            //Data
            writer.WriteFixedString32(key);

            ServerBehaviour.Instance.EndStream(writer);
        }
    }
}