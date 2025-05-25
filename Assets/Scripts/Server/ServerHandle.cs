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
                Debug.Log("SERVER: " + resp.UID);
                ServerGlobalData.AddClient(client, resp.UID);
                ServerSend.SendLoginResult(true, client);
            }
            else
            {
                //FAILED
                Debug.Log("SERVER: " + resp.customMessage);
                ServerSend.SendLoginResult(false, client);
            }
        }

        public static void GetKey(DataStreamReader reader, int client)
        {
            KeyCode key = (KeyCode)reader.ReadInt();

            ServerSend.SendKeyString(key.ToString(), client);
        }
    }


    [System.Serializable]
    internal class LoginRequest : Request
    {
        public string email;
        public string password;

        public LoginRequest(string email, string password) : base(ServerGlobalData.sessionID)
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