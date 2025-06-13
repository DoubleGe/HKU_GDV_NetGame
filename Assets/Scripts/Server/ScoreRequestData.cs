using UnityEngine;


namespace NetGame.Server
{
    [System.Serializable]
    public class ScoreSendRequest
    {
        public string SessionID;
        public int gameID;
        public int UID;
        public int score;

        public ScoreSendRequest(int gameID, int UID, int score)
        {
            SessionID = ServerGlobalData.sessionID;

            this.gameID = gameID;
            this.UID = UID;
            this.score = score;
        }
    }
}