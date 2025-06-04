using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NetGame.Client
{
    public class LoginManager : GenericSingleton<LoginManager>
    {
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField passwordInput;

        [SerializeField] private Button signInButton, signUpButton;

        public void EnableLoginButton() => signInButton.interactable = true;

        public void LoginButton()
        {
            if (string.IsNullOrEmpty(emailInput.text)) return;
            if (string.IsNullOrEmpty(passwordInput.text)) return;

            signInButton.interactable = false;
            signUpButton.interactable = false;

            string email = emailInput.text;
            string password = passwordInput.text;

            ClientSend.SendLogin(email, password);
        }

        public void LoginResult(bool result)
        {
            signInButton.interactable = true;
            signUpButton.interactable = true;

            UIManager.Instance.ShowWaitingOpponent(result);
            if(result) gameObject.SetActive(false);
        }

        public void ClearLoginData()
        {
            emailInput.text = string.Empty;
            passwordInput.text = string.Empty;
        }
    }
}