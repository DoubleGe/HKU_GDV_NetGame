using UnityEngine;

public enum ServerNetPacket
{
   SEND_LOGIN_RESULT = 1,

  
   TEMP_SEND_NUMBER = 2,
   TEMP_SEND_POSITION = 3,
   TEMP_SEND_KEYSTRING = 4
}

public enum ClientNetPacket
{
    SEND_LOGIN = 1,


    TEMP_SEND_KEY = 2
}