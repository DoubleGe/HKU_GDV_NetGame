using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
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

        public static void SendScoreData(List<ScoreInfo> score, ScoreType scoreType)
        {
            foreach (int client in ServerGlobalData.clients.Keys)
            {
                if (score == null)
                {
                    Debug.LogError("Scores is null!");
                    return;
                }

                DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
                writer.WriteByte((byte)ServerNetPacket.SEND_SCORE);

                //Data
                writer.WriteByte((byte)scoreType);
                writer.WriteInt(score.Count);
                for (int i = 0; i < score.Count; i++)
                {
                    ScoreInfo scoreInfo = score[i];
                    writer.WriteFixedString64(scoreInfo.nickname);
                    writer.WriteInt(scoreInfo.score);

                    DateTime date = DateTime.Parse(scoreInfo.date);
                    writer.WriteFixedString32(date.ToString("HH:mm dd-MM-yyyy"));
                }

                ServerBehaviour.Instance.EndStream(writer);
            }
        }

        public static void SendCheckersBoardInfo(Vector2Int boardSize, string boardString)
        {
            bool playerTurn = MoveValidator.Instance.SetStartingPlayer();

            foreach (int client in ServerGlobalData.clients.Keys)
            {
                DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
                writer.WriteByte((byte)ServerNetPacket.SEND_BOARD_INFO);

                //Data
                writer.WriteVector2Int(boardSize);
                writer.WriteFixedString128(boardString);
                writer.WriteInt(client);
                writer.WriteBool(playerTurn);

                //Write Client Names
                for (int i = 0; i < 2; i++)
                {
                    writer.WriteFixedString32(ServerGlobalData.clients[i].username);
                }

                ServerBehaviour.Instance.EndStream(writer);
            }
        }

        public static void MoveResult(int pieceID, int moveToSquare)
        {
            foreach (int client in ServerGlobalData.clients.Keys)
            {
                DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
                writer.WriteByte((byte)ServerNetPacket.SEND_PIECE_MOVE_RESULT);

                //Data
                writer.WriteInt(pieceID);
                writer.WriteInt(moveToSquare);

                ServerBehaviour.Instance.EndStream(writer);
            }
        }

        public static void RemovePiece(int pieceID)
        {
            foreach (int client in ServerGlobalData.clients.Keys)
            {
                DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
                writer.WriteByte((byte)ServerNetPacket.SEND_REMOVE_PIECE);

                //Data
                writer.WriteInt(pieceID);

                ServerBehaviour.Instance.EndStream(writer);
            }
        }

        public static void SendPiecePosition(int pieceID, Vector2 piecePosition, int clientExclude)
        {
            foreach(int client in ServerGlobalData.clients.Keys)
            {
                if (client == clientExclude) continue;

                DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
                writer.WriteByte((byte)ServerNetPacket.SEND_PIECE_POSTION_TO_OTHER);

                //Data
                writer.WriteInt(pieceID);
                writer.WriteVector2(piecePosition);

                ServerBehaviour.Instance.EndStream(writer);
            }
        }

        public static void SendTurnInfo(bool isWhiteTurn)
        {
            foreach (int client in ServerGlobalData.clients.Keys)
            {
                DataStreamWriter writer = ServerBehaviour.Instance.StartNewStream(client);
                writer.WriteByte((byte)ServerNetPacket.SEND_PLAYER_TURN);

                //Data
                writer.WriteBool(isWhiteTurn);

                ServerBehaviour.Instance.EndStream(writer);
            }
        }
    }
}