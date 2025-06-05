using NetGame.Client;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace NetGame.Server
{
    public class ScoreManager : GenericSingleton<ScoreManager>
    {
        public async void ShowScores()
        {
            await SendScoresToClient(ScoreType.RECENT, 1);
            await SendScoresToClient(ScoreType.HIGHSCORE, 1);
        }

        private async Task SendScoresToClient(ScoreType scoreType, int gameID)
        {
            GetScoresRequest sendScoreRequest = new GetScoresRequest(gameID);
            string url = RequestManager.DEFAULT_URL + (scoreType == ScoreType.HIGHSCORE ? "GetHighScores.php" : "GetLastAddedScores.php");

            GetScoresResponse resp = await RequestManager.SendRequestAsync<GetScoresRequest, GetScoresResponse>(url, sendScoreRequest);

            if (resp == null) return;

            if (resp.status == "OK")
            {
                Debug.Log(resp.scores);

                ServerSend.SendScoreData(resp.scores, scoreType);
            }
            else
            {
                //FAILED
                Debug.Log(resp.customMessage);
            }
        }

        [System.Serializable]
        private class GetScoresRequest : Request
        {
            public int gameID;

            public GetScoresRequest(int gameID) : base()
            {
                this.gameID = gameID;
            }
        }

        [System.Serializable]
        private class GetScoresResponse : Response
        {
            public List<ScoreInfo> scores;
        }
    }
}