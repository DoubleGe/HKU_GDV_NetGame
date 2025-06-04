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
            TryPromoteToKing(piece);

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

            if (!IsInsideBoard(targetPos)) return false;

            // KING
            if (isKing && absX == absY)
            {
                Vector2Int step = new Vector2Int((int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y));
                Vector2Int current = piece.currentPosition + step;

                bool foundEnemy = false;
                Vector2Int enemyPos = new Vector2Int(-1, -1);

                while (current != targetPos)
                {
                    ServerCheckerPiece currentPiece = CheckersBoard.Instance.GetCheckerBoard()[current.x, current.y].Piece;
                    if (currentPiece != null)
                    {
                        if (currentPiece.ownerID == piece.ownerID) return false;
                        if (foundEnemy) return false;
                        foundEnemy = true;
                        enemyPos = current;
                    }
                    current += step;
                }

                if (foundEnemy)
                {
                    isJump = true;
                    jumpedPiecePos = enemyPos;
                }

                return true;
            }

            // Normall
            if (absX == 2 && absY == 2)
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

            // Normall
            if (absX == 1 && absY == 1)
            {
                if (!isKing)
                {
                    if (isWhite && direction.y < 0) return false;
                    if (!isWhite && direction.y > 0) return false;
                }
                return true;
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
            int absX = Mathf.Abs(direction.x);
            int absY = Mathf.Abs(direction.y);

            if (absX != absY) return false; 

            if (!piece.isKing)
            {
                return absX == 2 && absY == 2;
            }
            else
            {
                Vector2Int step = new Vector2Int((int)Mathf.Sign(direction.x), (int)Mathf.Sign(direction.y));
                Vector2Int current = piece.currentPosition + step;

                bool foundEnemy = false;

                while (current != movePos)
                {
                    var board = CheckersBoard.Instance.GetCheckerBoard();
                    if (!IsInsideBoard(current)) return false;

                    var checkPiece = board[current.x, current.y].Piece;

                    if (checkPiece != null)
                    {
                        if (checkPiece.ownerID == piece.ownerID) return false;
                        if (foundEnemy) return false;
                        foundEnemy = true;
                    }

                    current += step;
                }

                return foundEnemy;
            }
        }


        private bool CanJumpAgain(ServerCheckerPiece piece)
        {
            ServerCheckerSquare[,] board = CheckersBoard.Instance.GetCheckerBoard();
            Vector2Int pos = piece.currentPosition;
            bool isKing = piece.isKing;

            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1)
            };

            foreach (Vector2Int dir in directions)
            {
                if (!isKing)
                {
                    // Normall
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
                else
                {
                    //King
                    Vector2Int current = pos + dir;
                    bool foundEnemy = false;

                    while (IsInsideBoard(current))
                    {
                        ServerCheckerPiece currentPiece = board[current.x, current.y].Piece;

                        if (currentPiece != null)
                        {
                            if (currentPiece.ownerID == piece.ownerID) break;
                            if (foundEnemy) break;
                            foundEnemy = true;
                            current += dir;
                            continue;
                        }

                        if (foundEnemy)
                        {
                            if (board[current.x, current.y].AllowPlacement)
                                return true;
                        }

                        current += dir;
                    }
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

        private void TryPromoteToKing(ServerCheckerPiece piece)
        {
            int y = piece.currentPosition.y;
            bool isWhite = piece.ownerID == 0;

            if (!piece.isKing && ((isWhite && y == 9) || (!isWhite && y == 0)))
            {
                piece.isKing = true;
                ServerSend.PromoteToKing(piece.ID);
            }
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