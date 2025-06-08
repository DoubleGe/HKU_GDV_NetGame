using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BoardDisplay : GenericSingleton<BoardDisplay>
{
    private CheckersSquare[,] gameBoard;

    [Header("Visuals")]
    [SerializeField] private Color32 lightColor;
    [SerializeField] private Color32 darkColor;
    [SerializeField] private Sprite boardSprite;
    [SerializeField] private Color32 moveColor;

    [Header("Pieces")]
    [SerializeField] private CheckerPiece whitePiece;
    [SerializeField] private CheckerPiece blackPiece;
    public List<CheckerPiece> GlobalPieces { private set; get; }

    private List<CheckersSquare> prevColored;

    private void Start()
    {
        GlobalPieces = new List<CheckerPiece>();
        prevColored = new List<CheckersSquare>();
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

                gameBoard[x, y] = new CheckersSquare(sr, isLightSquare);
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
                    Vector2Int moveSquare = SquareNumToCoords(squareNum);

                    CheckerPiece tempPiece = Instantiate(piecePrefab, new Vector2(moveSquare.x, moveSquare.y), Quaternion.identity);
                    tempPiece.InitPiece(isKing, (color == "W" ? 0 : 1));

                    GlobalPieces.Add(tempPiece);
                    tempPiece.ID = GlobalPieces.Count;

                    board[moveSquare.x, moveSquare.y].SetPiece(tempPiece);
                }
            }
        }
    }

    public void MovePieceToSquare(int pieceID, int moveSquare)
    {
        CheckerPiece piece = GetPieceWithID(pieceID);

        Vector2Int squarePos = SquareNumToCoords(moveSquare);

        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(piece.transform.position.x), Mathf.RoundToInt(piece.transform.position.y));

        piece.transform.position = (Vector3Int)squarePos;
        if (squarePos == piece.prevPosition) return;

        prevColored.ForEach(cs => cs.UpdateColor(lightColor));
        prevColored.Clear();

        CheckersSquare newSquare = gameBoard[squarePos.x, squarePos.y];
        CheckersSquare prevSquare = gameBoard[piece.prevPosition.x, piece.prevPosition.y];

        prevSquare.UpdateColor(moveColor);
        newSquare.UpdateColor(moveColor);

        prevColored.Add(prevSquare);
        prevColored.Add(newSquare);

        prevSquare.SetPiece(null);
        newSquare.SetPiece(piece);

        piece.prevPosition = squarePos;
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

    public CheckerPiece GetPieceWithID(int id) => GlobalPieces.First(p => p.ID == id);

    public void RemovePiece(int pieceID)
    {
        CheckerPiece checkerPiece = GlobalPieces.Find(p => p.ID == pieceID);

        GlobalPieces.Remove(checkerPiece);
        Destroy(checkerPiece.gameObject);
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

    public CheckerPiece Piece { private set; get; }
    public bool AllowPlacement { private set; get; }

    public CheckersSquare(SpriteRenderer sr, bool allowPlacement)
    {
        this.sr = sr;
        AllowPlacement = allowPlacement;
    }

    public void UpdateColor(Color32 color) => sr.color = color;

    public void SetPiece(CheckerPiece newPiece, bool destroyOld = false)
    {
        if (Piece != null && destroyOld) MonoBehaviour.Destroy(Piece.gameObject);
        Piece = newPiece;
    }
}