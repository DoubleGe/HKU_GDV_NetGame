using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<ServerCheckerPiece> GlobalPieces;

        private void Start()
        {
            whitePiecesList = new List<ServerCheckerPiece>();
            blackPiecesList = new List<ServerCheckerPiece>();
            GlobalPieces = new List<ServerCheckerPiece>();
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
                    bool allowPlacement = (x + y) % 2 == 0;
                    gameBoard[x, y] = new ServerCheckerSquare(allowPlacement);
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
                        Vector2Int moveCoords = SquareNumToCoords(squareNum);

                        ServerCheckerPiece tempPiece = new ServerCheckerPiece(moveCoords, isKing, (color == "W" ? 0 : 1));

                        pieceList.Add(tempPiece);
                        GlobalPieces.Add(tempPiece);
                        tempPiece.ID = GlobalPieces.Count;
                        board[moveCoords.x, moveCoords.y].SetPiece(tempPiece);
                    }
                }
            }
        }

        public Vector2Int SquareNumToCoords(int squareNum)
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

            return new Vector2Int(x, y);
        }

        public int SquareCoordsToNum(Vector2Int squareCoords)
        {
            int y = squareCoords.y;
            int x = squareCoords.x;

            int row = 9 - y;
            int colIndex;

            if (row % 2 == 0)
            {
                if ((x - 1) % 2 != 0)
                    throw new ArgumentException("Invalid x coordinate for even row.");
                colIndex = (x - 1) / 2;
            }
            else
            {
                if (x % 2 != 0)
                    throw new ArgumentException("Invalid x coordinate for odd row.");
                colIndex = x / 2;
            }

            if (colIndex < 0 || colIndex >= 5)
                throw new ArgumentException("x coordinate out of bounds.");

            int squareNum = row * 5 + colIndex + 1;
            return squareNum;
        }

        public ServerCheckerPiece GetPieceWithID(int id) => GlobalPieces.First(p => p.ID == id);

    }

    public class ServerCheckerSquare
    {
        public ServerCheckerPiece Piece { private set; get; }

        public bool AllowPlacement { private set; get; }

        public ServerCheckerSquare(bool allowPlacement)
        {
            AllowPlacement = allowPlacement;
        }

        public void SetPiece(ServerCheckerPiece newPiece) => Piece = newPiece;
    }

    public class ServerCheckerPiece
    {
        public int ID;
        public bool isKing;
        public Vector2Int currentPosition;
        public int ownerID;

        public ServerCheckerPiece(Vector2Int currentPosition, bool isKing, int ownerID)
        {
            this.currentPosition = currentPosition;
            this.isKing = isKing;
            this.ownerID = ownerID;
        }
    }
}