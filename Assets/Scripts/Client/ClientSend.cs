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
    }
}