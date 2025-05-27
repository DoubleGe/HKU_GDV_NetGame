using UnityEngine;

namespace NetGame.Server {
    public class MoveValidator : GenericSingleton<MoveValidator>
    {

        public void ValidateMove(int pieceID, int moveSquare, int client)
        {
            ServerCheckerPiece piece = CheckersBoard.Instance.GetPieceWithID(pieceID);
            int prevPosition = CheckersBoard.Instance.SquareCoordsToNum(piece.currentPosition);

            if (moveSquare < 0 || piece == null)
            {
                ServerSend.MoveResult(pieceID, prevPosition, client);
                return;
            }

            if (piece.ownerID != client)
            {
                ServerSend.MoveResult(pieceID, prevPosition, client);
                return;
            }

            ServerCheckerSquare[,] board = CheckersBoard.Instance.GetCheckerBoard();
            Vector2Int movePos = CheckersBoard.Instance.SquareNumToCoords(moveSquare);

            if (movePos.y > board.GetLength(0)) return;

            ServerCheckerSquare boardSquare = board[movePos.x, movePos.y];

            if (!boardSquare.AllowPlacement || boardSquare.Piece != null)
            {
                ServerSend.MoveResult(pieceID, prevPosition, client);
                return;
            }

            board[piece.currentPosition.x, piece.currentPosition.y].SetPiece(null);
            board[movePos.x, movePos.y].SetPiece(piece);
            piece.currentPosition = movePos;

            ServerSend.MoveResult(pieceID, moveSquare, client);
            //$$ SWITC SIDE
        }


    }
}