using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;

    [SerializeField] private Button signInButton, signUpButton;

    public async void LoginButton()
    {
        if (string.IsNullOrEmpty(emailInput.text)) return;
        if (string.IsNullOrEmpty(passwordInput.text)) return;

        signInButton.interactable = false;
        signUpButton.interactable = false;

        string email = emailInput.text;
        string password = passwordInput.text;

        LoginRequest loginRequest = new LoginRequest(email, password);
        (bool succes, LoginResponse data) resp = await RequestManager.SendRequestAsync<LoginRequest, LoginResponse>(RequestManager.DEFAULT_URL + "UserLogin.php", loginRequest);

        //FAILED
        if (resp.succes == false) return;

        if(resp.data.status == "OK")
        {
            UserGlobalData.sessionID = resp.data.sessionID;
            Debug.Log(resp.data.sessionID);

            gameObject.SetActive(false);
        }
        else
        {
            //FAILED
            Debug.Log(resp.data.customMessage);
        }

        signInButton.interactable = true;
        signUpButton.interactable = true;
    }

    public void ClearLoginData()
    {
        emailInput.text = string.Empty;
        passwordInput.text = string.Empty;
    }
}

[System.Serializable]
public class LoginRequest
{
    public string email;
    public string password;

    public LoginRequest(string email, string password)
    {
        this.email = email;
        this.password = password;
    }
}

[System.Serializable]
public class LoginResponse : Response
{
    public string sessionID;
}