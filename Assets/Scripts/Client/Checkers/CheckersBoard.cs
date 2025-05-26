using UnityEngine;
using UnityEngine.UI;

public class CheckersBoard : GenericSingleton<CheckersBoard>
{
    private CheckersSquare[,] gameBoard;

    [SerializeField] private Vector2Int boardSize;

    [SerializeField] private Color32 lightColor;
    [SerializeField] private Color32 darkColor;

    [SerializeField] private Sprite boardSprite;

    protected override void Awake()
    {
        base.Awake();

        gameBoard = new CheckersSquare[boardSize.x, boardSize.y];

        GenerateChessBoard();
    }

    private void GenerateChessBoard()
    {
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                GameObject pieceObj = new GameObject($"Chess Grid[{x}, {y}]");
                pieceObj.transform.parent = transform;
                SpriteRenderer sr = pieceObj.AddComponent<SpriteRenderer>();
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
        for (int x = 0; x < boardSize.x; x++)
        {
            for (int y = 0; y < boardSize.y; y++)
            {
                bool isLightSquare = (x + y) % 2 == 0;

                gameBoard[x, y].UpdateColor((isLightSquare ? lightColor : darkColor));
            }
        }
    }

    public CheckersSquare[,] GetCheckerBoard() => gameBoard;

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