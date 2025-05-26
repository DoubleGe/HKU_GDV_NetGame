using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardDisplay : GenericSingleton<BoardDisplay>
{
    private CheckersSquare[,] gameBoard;

    [Header("Visuals")]
    [SerializeField] private Color32 lightColor;
    [SerializeField] private Color32 darkColor;
    [SerializeField] private Sprite boardSprite;

    [Header("Pieces")]
    [SerializeField] private CheckerPiece whitePiece;
    [SerializeField] private CheckerPiece blackPiece;
    private List<CheckerPiece> globalPieces;

    private void Start()
    {
        globalPieces = new List<CheckerPiece>();
    }

    public void CreateCheckersBoard(Vector2Int boardSize)
    {
        gameBoard = new CheckersSquare[boardSize.x, boardSize.y];

        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                SpriteRenderer sr = null;

                GameObject pieceObj = new GameObject($"Chess Grid[{x}, {y}]");
                pieceObj.transform.parent = transform;
                sr = pieceObj.AddComponent<SpriteRenderer>();
                bool isLightSquare = (x + y) % 2 == 0;

                pieceObj.transform.position = new Vector3(x, y, 0);
                sr.sprite = boardSprite;
                sr.color = (isLightSquare ? lightColor : darkColor);

                gameBoard[x, y] = new CheckersSquare(sr);
            }
        }
    }
    private void UpdateCheckerBoardColors()
    {
        for (int x = 0; x < gameBoard.GetLength(1); x++)
        {
            for (int y = 0; y < gameBoard.GetLength(0); y++)
            {
                bool isLightSquare = (x + y) % 2 == 0;

                gameBoard[x, y].UpdateColor((isLightSquare ? lightColor : darkColor));
            }
        }
    }

    public CheckersSquare[,] GetCheckerBoard() => gameBoard;


    public void GenerateBoardPieces(string fen)
    {
        string[] parts = fen.Split(':');
        if (parts.Length != 3) throw new ArgumentException("Invalid KingsRow FEN-string");

        string whitePieces = parts[1];
        SetupPieces(whitePieces, "W", whitePiece, gameBoard);

        string blackPieces = parts[2];
        SetupPieces(blackPieces, "B", blackPiece, gameBoard);
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

                    CheckerPiece tempPiece = Instantiate(piecePrefab, new Vector2(x, y), Quaternion.identity);
                    tempPiece.InitPiece(isKing);

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


    private void OnValidate()
    {
        if (gameBoard == null) return;

        UpdateCheckerBoardColors();
    }
}

public class CheckersSquare
{
    private SpriteRenderer sr;

    private CheckerPiece piece;

    public CheckersSquare(SpriteRenderer sr)
    {
        this.sr = sr;
    }

    public void UpdateColor(Color32 color) => sr.color = color;

    public void SetPiece(CheckerPiece newPiece, bool destroyOld = false)
    {
        if (piece != null && destroyOld) MonoBehaviour.Destroy(piece.gameObject);
        piece = newPiece;
    }
}