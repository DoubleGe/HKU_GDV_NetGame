using UnityEngine;

namespace NetGame.Server
{
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

                SendTempScore();
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

        //TEMP LOCATION
        private async void SendTempScore()
        {
            SendScoreRequest sendScoreRequest = new SendScoreRequest(1, 100, ServerGlobalData.sessionID);
            Debug.Log(sendScoreRequest.SessionID);

            Response resp = await RequestManager.SendRequestAsync<SendScoreRequest, Response>(RequestManager.DEFAULT_URL + "AddScore.php", sendScoreRequest);

            if (resp == null) return;

            if (resp.status == "OK")
            {
                Debug.Log(resp.customMessage);

                gameObject.SetActive(false);
            }
            else
            {
                //FAILED
                Debug.Log(resp.customMessage);
            }
        }

        [System.Serializable]
        private class SendScoreRequest : Request
        {
            public int gameID;
            public int score;

            public SendScoreRequest(int gameID, int score, string sessionID) : base (sessionID)
            {
                this.gameID = gameID;
                this.score = score;
            }
        }
    }
}