using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardSpawner : MonoBehaviour
{
    [SerializeField] private string boardString;

    [SerializeField] private CheckersBoard checkersBoard;

    [SerializeField] private CheckerPiece whitePiece, blackPiece;


    private void Start()
    {
        GenerateBoardPieces(boardString);
    }

    private void GenerateBoardPieces(string fen)
    {
        CheckersSquare[,] board = checkersBoard.GetCheckerBoard();

        string[] parts = fen.Split(':');
        if (parts.Length != 3) throw new ArgumentException("Invalid KingsRow FEN-string");

        string whitePieces = parts[1];
        SetupPieces(whitePieces, "W", whitePiece, board);

        string blackPieces = parts[2];
        SetupPieces(blackPieces, "B", blackPiece, board);
    }

    private void SetupPieces(string colorPieces, string color, CheckerPiece piecePrefab, CheckersSquare[,] board)
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

                    CheckerPiece tempPiece = Instantiate(piecePrefab, new Vector2(x,y), Quaternion.identity);
                    tempPiece.InitPiece(isKing);

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

