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

        public void SetPlayerTurn(bool whiteTurn)
        {
            playerTurnMarker[0].SetActive(whiteTurn);
            playerTurnMarker[1].SetActive(!whiteTurn);
        }
    }
}
