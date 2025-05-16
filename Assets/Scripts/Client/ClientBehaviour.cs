using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Collections.Generic;

namespace NetGame.Client
{
    public class ClientBehaviour : GenericSingleton<ClientBehaviour>
    {
        private NetworkDriver netDriver;
        private NetworkConnection netConnection;

        private delegate void PacketHandler(DataStreamReader reader);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        private void Start()
        {
            PacketInitialize();

            netDriver = NetworkDriver.Create();

            NetworkEndpoint endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(7777);
            netConnection = netDriver.Connect(endpoint);
        }

        private void OnDestroy()
        {
            netDriver.Dispose();
        }

        private void Update()
        {
            netDriver.ScheduleUpdate().Complete();

            if (!netConnection.IsCreated)
            {
                return;
            }

            Unity.Collections.DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = netConnection.PopEvent(netDriver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("CLIENT: We are now connected to the server.");
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    byte value = stream.ReadByte();
                    Debug.Log($"CLIENT: Got the packet {value} from server.");

                    packetHandlers[value]?.Invoke(stream);

                    //netConnection.Disconnect(netDriver);
                    //netConnection = default;
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("CLIENT: Client got disconnected from server.");
                    netConnection = default;
                }
            }
        }

        public DataStreamWriter StartNewStream()
        {
            netDriver.BeginSend(netConnection, out var writer);
            return writer;
        }

        public void EndStream(DataStreamWriter writer)
        {
            netDriver.EndSend(writer);
        }

        private void PacketInitialize()
        {
            packetHandlers = new Dictionary<byte, PacketHandler>
            {
                { (byte)ServerNetPacket.TEMP_SEND_NUMBER, ClientHandle.ReadInt},
                { (byte) ServerNetPacket.TEMP_SEND_POSITION, ClientHandle.ReadInt },
                { (byte) ServerNetPacket.TEMP_SEND_KEYSTRING, ClientHandle.GetKey },
            };
        }
    }
}