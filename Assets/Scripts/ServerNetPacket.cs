using UnityEngine;

public enum ServerNetPacket
{
   SEND_LOGIN_RESULT = 1,
   SEND_SCORE = 2,
   SEND_BOARD_INFO = 3,
   SEND_PIECE_MOVE_RESULT = 4,
   SEND_PIECE_POSTION_TO_OTHER = 5,
}

public enum ClientNetPacket
{
    SEND_LOGIN = 1,
    SEND_PIECE_MOVE = 2,
    SEND_PIECE_POSITION = 3
}