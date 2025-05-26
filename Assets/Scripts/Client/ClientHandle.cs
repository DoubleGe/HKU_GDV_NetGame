using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace NetGame.Client
{
    public class ClientHandle
    {
        public static void GetLoginResult(DataStreamReader reader)
        {
            LoginManager.Instance.LoginResult(reader.ReadBool());
        }

        public static void GetScoreResult(DataStreamReader reader)
        {
            List<ScoreInfo> playerScores = new List<ScoreInfo>();

            ScoreType scoreType = (ScoreType)reader.ReadByte();
            int scoreCount = reader.ReadInt();
            for (int i = 0; i < scoreCount; i++)
            {
                string nickname =  reader.ReadFixedString32().ToString();
                int score = reader.ReadInt();
                long dateTicks = reader.ReadLong();

                ScoreInfo scoreInfo = new ScoreInfo(nickname, score, new System.DateTime(dateTicks));
            }

            ScoreDisplay.Instance.ShowScore(playerScores, scoreType);
        }


        public static void ReadInt(DataStreamReader reader)
        {
            Debug.Log("CLIENT: Number: " + reader.ReadInt());
        }

        public static void GetPosition(DataStreamReader reader)
        {
            Debug.Log("CLIENT: Position: " + reader.ReadVector3());
        }

        public static void GetKey(DataStreamReader reader)
        {
            Debug.Log("CLIENT: Key: " + reader.ReadFixedString32());
        }
    }
}