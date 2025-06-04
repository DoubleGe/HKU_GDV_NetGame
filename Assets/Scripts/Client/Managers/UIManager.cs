using TMPro;
using UnityEngine;

namespace NetGame.Client
{
    public class UIManager : GenericSingleton<UIManager>
    {
        [Header("Game Info")]
        [SerializeField] private GameObject gameUI;
        [SerializeField] private TextMeshProUGUI player1NameText;
        [SerializeField] private TextMeshProUGUI player2NameText;
        [SerializeField] private GameObject[] playerTurnMarker;

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
    }
}
