using NetGame.Server;
using System;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccount : MonoBehaviour
{
    [SerializeField] private Button createButton, goBackButton;

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private TMP_InputField birthdayDayInput;
    [SerializeField] private TMP_InputField birthdayMonthInput;
    [SerializeField] private TMP_InputField birthdayYearInput;

    public async void SendAccountCreate()
    {
        if (!InputCheck()) return;

        int day = int.Parse(birthdayDayInput.text);
        int month = int.Parse(birthdayMonthInput.text);
        int year = int.Parse(birthdayYearInput.text);

        if (day > 31 || day < 0) return;
        if (month > 12 || month < 0) return;
        if (year < 0) return;

        createButton.interactable = false;
        goBackButton.interactable = false;

        DateTime birthDay = new DateTime(year, month, day);



        CreateAccountRequest createAccountRequest = new CreateAccountRequest(emailInput.text, passwordInput.text, nickNameInput.text, birthDay);
        Response resp = await RequestManager.SendRequestAsync<CreateAccountRequest, Response>(RequestManager.DEFAULT_URL + "CreateUser.php", createAccountRequest);

        //FAILED
        if (resp == null) return;

        if (resp.status == "OK")
        {
            Debug.Log(resp.customMessage);
        }
        else
        {
            //FAILED
            Debug.Log(resp.customMessage);
        }

        createButton.interactable = true;
        goBackButton.interactable = true;
    }

    private bool InputCheck()
    {
        if (string.IsNullOrEmpty(emailInput.text)) return false;
        if (string.IsNullOrEmpty(passwordInput.text)) return false;
        if (string.IsNullOrEmpty(nickNameInput.text)) return false;
        if (string.IsNullOrEmpty(birthdayDayInput.text)) return false;
        if (string.IsNullOrEmpty(birthdayMonthInput.text)) return false;
        if (string.IsNullOrEmpty(birthdayYearInput.text)) return false;

        return true;
    }

    public void ClearInput()
    {
        emailInput.text = string.Empty;
        passwordInput.text = string.Empty;
        nickNameInput.text = string.Empty;
        birthdayDayInput.text = string.Empty;
        birthdayMonthInput.text = string.Empty;
        birthdayYearInput.text = string.Empty;
    }

    [System.Serializable]
    private class CreateAccountRequest
    {
        public string email;
        public string password;
        public string nickName;
        public string birthday;

        public CreateAccountRequest(string email, string password, string nickName, DateTime birthday)
        {
            this.email = email;
            this.password = password;
            this.nickName = nickName;
            this.birthday = birthday.ToString("yyyy-MM-dd");
        }
    }
}