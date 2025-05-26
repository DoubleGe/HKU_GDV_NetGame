using UnityEngine;

public enum ServerNetPacket
{
   SEND_LOGIN_RESULT = 1,
   SEND_SCORE = 2,
}

public enum ClientNetPacket
{
    SEND_LOGIN = 1,
}