using UnityEngine;
using UnityEngine.EventSystems;

public class CheckerPiece : MonoBehaviour
{
    private Vector3 offset;
    private Vector2Int prevPosition;

    private bool isKing;

    public void InitPiece(bool isKing)
    {

    }

    private void OnMouseDown()
    {
        prevPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; 
        offset = transform.position - mousePosition;
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; 
        transform.position = mousePosition + offset;
    }

    private void OnMouseUp()
    {
        Vector2Int boardPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        transform.position = (Vector3Int)boardPos;

        CheckersBoard.Instance.GetCheckerBoard()[boardPos.x, boardPos.y].SetPiece(this, true);
    }
}

[System.Serializable]
public class PieceData
{
    public char refChar;
    public CheckerPiece piece;
}