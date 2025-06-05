using TMPro;
using UnityEngine;

namespace NetGame.Client
{
    public class UIManager : GenericSingleton<UIManager>
    {
        [Header("Waiting For Opponent")]
        [SerializeField] private GameObject waitingOpponentObj;

        [Header("Game Info")]
        [SerializeField] private GameObject gameUI;
        [SerializeField] private TextMeshProUGUI player1NameText;
        [SerializeField] private TextMeshProUGUI player2NameText;
        [SerializeField] private GameObject[] playerTurnMarker;

        [Header("Win UI")]
        [SerializeField] private GameObject resultObject;
        [SerializeField] private GameObject winObject;
        [SerializeField] private GameObject loseObject;
        [SerializeField] private GameObject whiteWinsObject;
        [SerializeField] private GameObject blackWinsObject;

        public void ShowWaitingOpponent(bool show)
        {
            waitingOpponentObj.SetActive(show);
        }

        public void SetupGame(string player1Name, string player2Name)
        {
            player1NameText.text = player1Name;
            player2NameText.text = player2Name;

            gameUI.SetActive(true);
        }

        public void SetPlayerTurn(bool isWhiteTurn)
        {
            playerTurnMarker[0].SetActive(isWhiteTurn);
            playerTurnMarker[1].SetActive(!isWhiteTurn);

            UserGlobalData.isMyTurn = (UserGlobalData.clientID == 0 && isWhiteTurn) || (UserGlobalData.clientID == 1 && !isWhiteTurn);
        }

        public void SetPlayerWin(int winningPlayer, int losingPlayer, bool didWhiteWin)
        {
            if (winningPlayer == UserGlobalData.clientID) winObject.SetActive(true);
            else if (losingPlayer == UserGlobalData.clientID) loseObject.SetActive(true);
            else if (didWhiteWin) whiteWinsObject.SetActive(true);
            else if (!didWhiteWin) whiteWinsObject.SetActive(false);

            resultObject.SetActive(true);
        }
    }
}
