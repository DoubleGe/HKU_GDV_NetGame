using NetGame.Client;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckerPiece : MonoBehaviour
{
    public int ID;

    private Vector3 offset;
    private Vector2Int prevPosition;

    private bool isKing;
    private int owner;

    public void InitPiece(bool isKing, int owner)
    {
        this.isKing = isKing;
        this.owner = owner;
    }

    private void OnMouseDown()
    {
        if (owner != UserGlobalData.clientID || !UserGlobalData.isMyTurn) return;

        prevPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        offset = transform.position - mousePosition;
    }

    private void OnMouseDrag()
    {
        if (owner != UserGlobalData.clientID || !UserGlobalData.isMyTurn) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition + offset;

        ClientSend.SendPiecePosition(ID, transform.position);
    }

    private void OnMouseUp()
    {
        if (owner != UserGlobalData.clientID || !UserGlobalData.isMyTurn) return;

        Vector2Int boardPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        CheckersSquare[,] board = BoardDisplay.Instance.GetCheckerBoard();

        //Check if placed position is valid.
        if(boardPos.x < 0 || boardPos.y < 0 || boardPos.x > board.GetLength(1) || boardPos.y > board.GetLength(0))
        {
            transform.position = (Vector3Int)prevPosition;

            ClientSend.SendPiecePosition(ID, transform.position);
            return;
        }

        CheckersSquare checkersSquare = board[boardPos.x, boardPos.y];

        if (checkersSquare.AllowPlacement || checkersSquare.Piece != null)
        {
            transform.position = (Vector3Int)boardPos;

            board[prevPosition.x, prevPosition.y].SetPiece(null);
            board[boardPos.x, boardPos.y].SetPiece(this);


            ClientSend.SendPieceMove(ID, BoardDisplay.Instance.SquareCoordsToNum(boardPos));
        }
        else
        {
            transform.position = (Vector3Int)prevPosition;

            ClientSend.SendPiecePosition(ID, transform.position);
        }
    }
}