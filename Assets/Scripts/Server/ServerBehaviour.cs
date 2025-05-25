using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

namespace NetGame.Server
{
    public class ServerBehaviour : GenericSingleton<ServerBehaviour>
    {
        private NetworkDriver netDriver;
        private NativeList<NetworkConnection> netConnection;

        private delegate void PacketHandler(DataStreamReader reader, int client);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        void Start()
        {
            PacketInitialize();

            netDriver = NetworkDriver.Create();
            netConnection = new NativeList<NetworkConnection>(16, Allocator.Persistent);

            NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4.WithPort(7777);
            if (netDriver.Bind(endpoint) != 0)
            {
                Debug.LogError("SERVER: Failed to bind to port 7777.");
                return;
            }
            netDriver.Listen();
        }

        private void OnDestroy()
        {
            if (netDriver.IsCreated)
            {
                netDriver.Dispose();
                netConnection.Dispose();
            }
        }

        void Update()
        {
            netDriver.ScheduleUpdate().Complete();

            for (int i = 0; i < netConnection.Length; i++)
            {
                if (!netConnection[i].IsCreated)
                {
                    netConnection.RemoveAtSwapBack(i);
                    i--;
                }
            }

            NetworkConnection c;
            while ((c = netDriver.Accept()) != default)
            {
                netConnection.Add(c);
                Debug.Log("SERVER: Accepted a connection.");

                ServerSend.SendInt(1000, netConnection.Length - 1);
            }

            for (int i = 0; i < netConnection.Length; i++)
            {
                DataStreamReader stream;
                NetworkEvent.Type cmd;
                while ((cmd = netDriver.PopEventForConnection(netConnection[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        byte packetID = stream.ReadByte();
                        Debug.Log($"SERVER: Got packet: {packetID}. from a client.");

                        packetHandlers[packetID]?.Invoke(stream, i);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.Log("SERVER: Client disconnected from the server.");
                        netConnection[i] = default;
                        break;
                    }
                }
            }
        }

        public DataStreamWriter StartNewStream(int playerID)
        {
            netDriver.BeginSend(netConnection[playerID], out var writer);
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
                { (byte)ClientNetPacket.SEND_LOGIN, ServerHandle.GetLogin },
                { (byte)ClientNetPacket.TEMP_SEND_KEY, ServerHandle.GetKey},
            };
        }
    }
}