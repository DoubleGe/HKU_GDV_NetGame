using System.Collections.Generic;
using UnityEngine;

namespace NetGame.Server
{
    public class MoveValidator : GenericSingleton<MoveValidator>
    {
        private bool whiteTurn = true;

        public void ValidateMove(int pieceID, int moveSquare, int client)
        {
            ServerCheckerPiece piece = CheckersBoard.Instance.GetPieceWithID(pieceID);
            int prevPosition = CheckersBoard.Instance.SquareCoordsToNum(piece.currentPosition);

            if (!IsPlayerTurn(client))
            {
                ServerSend.MoveResult(pieceID, prevPosition);
                return;
            }

            if (moveSquare < 0 || piece == null)
            {
                ServerSend.MoveResult(pieceID, prevPosition);
                return;
            }

            if (piece.ownerID != client)
            {
                ServerSend.MoveResult(pieceID, prevPosition);
                return;
            }

            ServerCheckerSquare[,] board = CheckersBoard.Instance.GetCheckerBoard();
            Vector2Int movePos = CheckersBoard.Instance.SquareNumToCoords(moveSquare);

            if (movePos.y > board.GetLength(0)) return;

            ServerCheckerSquare boardSquare = board[movePos.x, movePos.y];

            if (!boardSquare.AllowPlacement || boardSquare.Piece != null)
            {
                ServerSend.MoveResult(pieceID, prevPosition);
                return;
            }

            if (HasMandatoryJump(client) && !IsAttemptingJump(piece, movePos))
            {
                ServerSend.MoveResult(pieceID, prevPosition);
                return;
            }

            if (!IsValidMove(piece, movePos, out bool isJump, out Vector2Int jumpedPiecePos))
            {
                ServerSend.MoveResult(pieceID, prevPosition);
                return;
            }

            board[piece.currentPosition.x, piece.currentPosition.y].SetPiece(null);
            board[movePos.x, movePos.y].SetPiece(piece);
            piece.currentPosition = movePos;

            ServerSend.MoveResult(pieceID, moveSquare);

            if (isJump) //Remove Piece
            {
                int delPieceID = board[jumpedPiecePos.x, jumpedPiecePos.y].Piece.ID;

                CheckersBoard.Instance.RemovePiece(delPieceID);

                if (!CanJumpAgain(piece))
                {
                    whiteTurn = !whiteTurn;

                    ServerSend.SendTurnInfo(whiteTurn);
                }
            }
            else
            {
                whiteTurn = !whiteTurn;

                ServerSend.SendTurnInfo(whiteTurn);
            }           
        }

        private bool IsValidMove(ServerCheckerPiece piece, Vector2Int targetPos, out bool isJump, out Vector2Int jumpedPiecePos)
        {
            isJump = false;
            jumpedPiecePos = new Vector2Int(-1, -1);

            Vector2Int direction = targetPos - piece.currentPosition;
            int absX = Mathf.Abs(direction.x);
            int absY = Mathf.Abs(direction.y);

            bool isWhite = piece.ownerID == 0;
            bool isKing = piece.isKing;

            if (!isKing)
            {
                if (isWhite && direction.y < 0) return false;
                if (!isWhite && direction.y > 0) return false;
            }

            if (absX == 1 && absY == 1)
            {
                return true;
            }
            else if (absX == 2 && absY == 2)
            {
                Vector2Int middle = piece.currentPosition + direction / 2;
                ServerCheckerPiece middlePiece = CheckersBoard.Instance.GetCheckerBoard()[middle.x, middle.y].Piece;

                if (middlePiece != null && middlePiece.ownerID != piece.ownerID)
                {
                    isJump = true;
                    jumpedPiecePos = middle;
                    return true;
                }
            }

            return false;
        }

        private bool HasMandatoryJump(int clientID)
        {
            foreach (var piece in CheckersBoard.Instance.GetAllPieces())
            {
                if (piece.ownerID != clientID) continue;
                if (CanJumpAgain(piece)) return true;
            }
            return false;
        }

        private bool IsAttemptingJump(ServerCheckerPiece piece, Vector2Int movePos)
        {
            Vector2Int direction = movePos - piece.currentPosition;
            return Mathf.Abs(direction.x) == 2 && Mathf.Abs(direction.y) == 2;
        }

        private bool CanJumpAgain(ServerCheckerPiece piece)
        {
            ServerCheckerSquare[,] board = CheckersBoard.Instance.GetCheckerBoard();
            Vector2Int pos = piece.currentPosition;
            bool isWhite = piece.ownerID == 0;
            bool isKing = piece.isKing;

            List<Vector2Int> directions = new List<Vector2Int>();

            if (isKing)
            {
                directions.AddRange(new[] {
                    new Vector2Int(1, 1), new Vector2Int(-1, 1),
                    new Vector2Int(1, -1), new Vector2Int(-1, -1)
                });
            }
            else
            {
                int forward = isWhite ? 1 : -1;
                directions.AddRange(new[] {
                    new Vector2Int(1, forward), new Vector2Int(-1, forward)
                });
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int midPos = pos + dir;
                Vector2Int jumpPos = pos + dir * 2;

                if (!IsInsideBoard(jumpPos)) continue;

                ServerCheckerPiece middlePiece = board[midPos.x, midPos.y].Piece;
                ServerCheckerSquare targetSquare = board[jumpPos.x, jumpPos.y];

                if (middlePiece != null &&
                    middlePiece.ownerID != piece.ownerID &&
                    targetSquare.Piece == null &&
                    targetSquare.AllowPlacement)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInsideBoard(Vector2Int pos)
        {
            ServerCheckerSquare[,] board = CheckersBoard.Instance.GetCheckerBoard();
            return pos.x >= 0 && pos.x < board.GetLength(0) &&
                   pos.y >= 0 && pos.y < board.GetLength(1);
        }

        public bool SetStartingPlayer()
        {
            whiteTurn = Random.Range(0, 2) == 0;
            return whiteTurn;
        }

        public bool IsPlayerTurn(int clientID)
        {
            bool isWhite = clientID == 0;
            bool isBlack = clientID == 1;

            return ((whiteTurn && isWhite) || (!whiteTurn && isBlack));
        }
    }
}