using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

namespace NetGame.Client
{
    public class ClientSend
    {
        public static void SendLogin(string email, string password)
        {
            DataStreamWriter writer = ClientBehaviour.Instance.StartNewStream(ClientNetPacket.SEND_LOGIN);

            writer.WriteFixedString64(email);
            writer.WriteFixedString32(password);

            ClientBehaviour.Instance.EndStream(writer);
        }

        public static void SendKey(KeyCode key)
        {
            DataStreamWriter writer = ClientBehaviour.Instance.StartNewStream(ClientNetPacket.TEMP_SEND_KEY);

            //Data
            writer.WriteInt((int)key);

            ClientBehaviour.Instance.EndStream(writer);
        }
    }
}