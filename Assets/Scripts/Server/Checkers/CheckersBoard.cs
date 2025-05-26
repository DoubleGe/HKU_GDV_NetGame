using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NetGame.Server
{
    public class CheckersBoard : GenericSingleton<CheckersBoard>
    {
        private ServerCheckerSquare[,] gameBoard;

        [SerializeField] private Vector2Int boardSize;
        [SerializeField] private string boardString;

        private List<ServerCheckerPiece> whitePiecesList;
        private List<ServerCheckerPiece> blackPiecesList;
        private List<ServerCheckerPiece> globalPieces;

        private void Start()
        {
            whitePiecesList = new List<ServerCheckerPiece>();
            blackPiecesList = new List<ServerCheckerPiece>();
            globalPieces = new List<ServerCheckerPiece>();
        }

        public void CreateCheckersBoard()
        {
            if (gameBoard != null) return;

            gameBoard = new ServerCheckerSquare[boardSize.x, boardSize.y];

            CreateCheckersBoard();

            for (int x = 0; x < boardSize.x; x++)
            {
                for (int y = 0; y < boardSize.y; y++)
                {
                    gameBoard[x, y] = new ServerCheckerSquare();
                }
            }

            GenerateBoardPieces(boardString);
            ServerSend.SendCheckersBoardInfo(boardSize, boardString);
        }

        public ServerCheckerSquare[,] GetCheckerBoard() => gameBoard;

        private void GenerateBoardPieces(string fen)
        {
            string[] parts = fen.Split(':');
            if (parts.Length != 3) throw new ArgumentException("Invalid KingsRow FEN-string");

            string whitePieces = parts[1];
            SetupPieces(whitePieces, "W", whitePiecesList, gameBoard);

            string blackPieces = parts[2];
            SetupPieces(blackPieces, "B", blackPiecesList, gameBoard);
        }

        private void SetupPieces(string colorPieces, string color, List<ServerCheckerPiece> pieceList, ServerCheckerSquare[,] board)
        {
            if (colorPieces.StartsWith(color)) colorPieces = colorPieces.Substring(1);

            if (!string.IsNullOrWhiteSpace(colorPieces))
            {
                string[] pieces = colorPieces.Split(',');
                foreach (string piece in pieces)
                {
                    if (string.IsNullOrWhiteSpace(piece))
                        continue;

                    bool isKing = piece.StartsWith("K") || piece.StartsWith(color);
                    string numStr = isKing ? piece.Substring(1) : piece;
                    if (int.TryParse(numStr, out int squareNum))
                    {
                        (int x, int y) = SquareNumToCoords(squareNum);

                        ServerCheckerPiece tempPiece = new ServerCheckerPiece(isKing);

                        pieceList.Add(tempPiece);
                        globalPieces.Add(tempPiece);
                        tempPiece.ID = globalPieces.Count;
                        board[x, y].SetPiece(tempPiece);
                    }
                }
            }


        }

        private (int x, int y) SquareNumToCoords(int squareNum)
        {
            int zeroBasedNum = squareNum - 1;
            int row = zeroBasedNum / 5;
            int colIndex = zeroBasedNum % 5;

            int y = 9 - row;

            int x;
            if (row % 2 == 0)
                x = colIndex * 2 + 1;
            else
                x = colIndex * 2;

            return (x, y);
        }
    }

    public class ServerCheckerSquare
    {
        private ServerCheckerPiece piece;

        public void SetPiece(ServerCheckerPiece newPiece)
        {
            piece = newPiece;
        }
    }

    public class ServerCheckerPiece
    {
        public int ID;
        public bool isKing;

        public ServerCheckerPiece(bool isKing)
        {
            this.isKing = isKing;
        }
    }
}