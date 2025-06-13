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
            //NetworkEndpoint endpoint = NetworkEndpoint.Parse("192.168.2.18", 7777, NetworkFamily.Ipv4); //IPV4 works tested it on local network.
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
                    LoginManager.Instance.EnableLoginButton();
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    byte value = stream.ReadByte();

                    packetHandlers[value]?.Invoke(stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    netConnection = default;
                }
            }
        }

        public DataStreamWriter StartNewStream(ClientNetPacket netPacket)
        {
            netDriver.BeginSend(netConnection, out var writer);
            writer.WriteByte((byte)netPacket);

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
                { (byte)ServerNetPacket.SEND_LOGIN_RESULT, ClientHandle.GetLoginResult },
                { (byte)ServerNetPacket.SEND_SCORE, ClientHandle.GetScoreResult },
                { (byte)ServerNetPacket.SEND_BOARD_INFO, ClientHandle.GetCheckerboardSettings },
                { (byte)ServerNetPacket.SEND_PIECE_MOVE_RESULT, ClientHandle.GetMoveResult },
                { (byte)ServerNetPacket.SEND_PIECE_POSTION_TO_OTHER, ClientHandle.GetPiecePosition },
                { (byte)ServerNetPacket.SEND_PLAYER_TURN, ClientHandle.GetPlayerTurn },
                { (byte)ServerNetPacket.SEND_REMOVE_PIECE, ClientHandle.RemovePiece },
                { (byte)ServerNetPacket.SEND_PIECE_PROMOTION, ClientHandle.PiecePromotion },
                { (byte)ServerNetPacket.SEND_GAME_RESULT, ClientHandle.GetWinResult },
            };
        }

        private void OnApplicationQuit()
        {
            netConnection.Disconnect(netDriver);
        }
    }
}