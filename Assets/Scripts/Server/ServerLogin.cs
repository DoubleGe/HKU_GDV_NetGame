using UnityEngine;

public class ServerLogin : MonoBehaviour
{
    private async void Start()
    {
        ServerLoginRequest serverLoginRequest = new ServerLoginRequest(2, "asfdgjkl");

        (bool succes, ServerLoginResponse data) resp = await RequestManager.SendRequestAsync<ServerLoginRequest, ServerLoginResponse>(RequestManager.DEFAULT_URL + "ServerLogin.php", serverLoginRequest);

        if (resp.succes == false) return;

        if (resp.data.status == "OK")
        {
            ServerGlobalData.sessionID = resp.data.sessionID;
            Debug.Log(resp.data.sessionID);

            gameObject.SetActive(false);
        }
        else
        {
            //FAILED
            Debug.Log(resp.data.customMessage);
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
