using System;
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
                string dateString = reader.ReadFixedString32().ToString();

                ScoreInfo scoreInfo = new ScoreInfo(nickname, score, dateString);
                playerScores.Add(scoreInfo);
            }

            ScoreDisplay.Instance.ShowScore(playerScores, scoreType);
        }

        public static void GetCheckerboardSettings(DataStreamReader reader)
        {
            Vector2Int boardSize = reader.ReadVector2Int();
            string boardString = reader.ReadFixedString128().ToString();
            int clientID = reader.ReadInt();
            bool startingPlayer = reader.ReadBool();

            string[] clientStrings = new string[2]; 
            //Write Client Names
            for (int i = 0; i < 2; i++)
            {
                clientStrings[i] = reader.ReadFixedString32().ToString();
            }

            UserGlobalData.clientID = clientID;
            BoardDisplay.Instance.CreateCheckersBoard(boardSize);
            BoardDisplay.Instance.GenerateBoardPieces(boardString);

            UIManager.Instance.SetupGame(clientStrings[0], clientStrings[1]);
            UIManager.Instance.SetPlayerTurn(startingPlayer);
        }

        public static void GetMoveResult(DataStreamReader reader)
        {
            int pieceID = reader.ReadInt();
            int pieceMove = reader.ReadInt();

            BoardDisplay.Instance.MovePieceToSquare(pieceID, pieceMove);
        }

        public static void GetPiecePosition(DataStreamReader reader)
        {
            int pieceID = reader.ReadInt();
            Vector2 piecePosition = reader.ReadVector2();

            CheckerPiece checkerPiece = BoardDisplay.Instance.GetPieceWithID(pieceID);

            if (checkerPiece == null) return;
            checkerPiece.transform.position = piecePosition;
        }

        public static void GetPlayerTurn(DataStreamReader reader)
        {
            bool isWhiteTurn = reader.ReadBool();

            UIManager.Instance.SetPlayerTurn(isWhiteTurn);
        }
    }
}