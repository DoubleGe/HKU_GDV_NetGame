using UnityEngine;

public class ServerLogin : MonoBehaviour
{
    private async void Start()
    {
        ServerLoginRequest serverLoginRequest = new ServerLoginRequest(2, "asfdgjkl");

       ServerLoginResponse resp = await RequestManager.SendRequestAsync<ServerLoginRequest, ServerLoginResponse>(RequestManager.DEFAULT_URL + "ServerLogin.php", serverLoginRequest);

        if (resp == null) return;

        if (resp.status == "OK")
        {
            ServerGlobalData.sessionID = resp.sessionID;
            Debug.Log(resp.sessionID);

            gameObject.SetActive(false);
        }
        else
        {
            //FAILED
            Debug.Log(resp.customMessage);
        }

    }

    [System.Serializable]
    private class ServerLoginRequest
    {
        public int ID;
        public string password;

        public ServerLoginRequest(int iD, string password)
        {
            ID = iD;
            this.password = password;
        }
    }

    [System.Serializable]
    private class ServerLoginResponse : Response
    {
        public string sessionID;
    }
}
