using Unity.Collections;
using UnityEngine;

namespace NetGame.Server
{
    public class ServerHandle
    {
        public async static void GetLogin(DataStreamReader reader, int client)
        {
            string email = reader.ReadFixedString64().ToString();
            string password = reader.ReadFixedString32().ToString();

            LoginRequest loginRequest = new LoginRequest(email, password);
            LoginResponse resp = await RequestManager.SendRequestAsync<LoginRequest, LoginResponse>(RequestManager.DEFAULT_URL + "UserLogin.php", loginRequest);

            //FAILED
            if (resp == null) return;

            if (resp.status == "OK")
            {
                ServerGlobalData.AddClient(client, resp.UID);
                ServerSend.SendLoginResult(true, client);

                //Should check for 2
                if (ServerGlobalData.clients.Count >= 2)
                {
                    CheckersBoard.Instance.CreateCheckersBoard();
                }
            }
            else
            {
                //FAILED
                Debug.Log("SERVER: " + resp.customMessage);
                ServerSend.SendLoginResult(false, client);
            }
        }

        public static void GetPieceMove(DataStreamReader reader, int client)
        {
            int pieceID = reader.ReadInt();
            int pieceMove = reader.ReadInt();

            MoveValidator.Instance.ValidateMove(pieceID, pieceMove, client);
        }

        public static void SendToOtherPlayerPiecePosition(DataStreamReader reader, int client)
        {
            int pieceID = reader.ReadInt();
            Vector2 piecePosition = reader.ReadVector2();

            ServerCheckerPiece piece = CheckersBoard.Instance.GetPieceWithID(pieceID);
            if(piece == null) return;
            if (piece.ownerID != client) return;

            ServerSend.SendPiecePosition(pieceID, piecePosition, client);
        }
    }


    [System.Serializable]
    internal class LoginRequest : Request
    {
        public string email;
        public string password;

        public LoginRequest(string email, string password) : base()
        {
            this.email = email;
            this.password = password;
        }
    }

    [System.Serializable]
    internal class LoginResponse : Response
    {
        public int UID;
    }
}