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

        public static void SendPieceMove(int pieceID, int moveToSquare)
        {
            DataStreamWriter writer = ClientBehaviour.Instance.StartNewStream(ClientNetPacket.SEND_PIECE_MOVE);

            writer.WriteInt(pieceID);
            writer.WriteInt(moveToSquare);


            ClientBehaviour.Instance.EndStream(writer);
        }

        public static void SendPiecePosition(int pieceID, Vector2 piecePosition)
        {
            DataStreamWriter writer = ClientBehaviour.Instance.StartNewStream(ClientNetPacket.SEND_PIECE_POSITION, SendType.UDP);

            writer.WriteInt(pieceID);
            writer.WriteVector2(piecePosition);

            ClientBehaviour.Instance.EndStream(writer);
        }
    }
}