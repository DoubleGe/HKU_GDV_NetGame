using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

namespace NetGame.Client
{
    public class ClientSend
    {
        public static void SendKey(KeyCode key)
        {
            DataStreamWriter writer = ClientBehaviour.Instance.StartNewStream();
            writer.WriteByte((byte)ClientNetPacket.TEMP_SEND_KEY);

            //Data
            writer.WriteInt((int)key);

            ClientBehaviour.Instance.EndStream(writer);
        }
    }
}