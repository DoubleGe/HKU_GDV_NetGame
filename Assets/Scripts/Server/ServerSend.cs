using System;
using System.Collections.Generic;
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

        public static void SendScoreData(List<ScoreInfo> score, ScoreType scoreType, int client)
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
}