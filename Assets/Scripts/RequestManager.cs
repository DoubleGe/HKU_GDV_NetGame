using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class RequestManager
{
    public const string DEFAULT_URL = "https://api.wesleydegraaf.com/GDVNet/";
    public static async Task<(bool succes, O data)> SendRequestAsync<T, O>(string url, T requestData)
    {
        WWWForm form = new WWWForm();
        form.AddField("request", JsonUtility.ToJson(requestData));

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                O data = JsonUtility.FromJson<O>(www.downloadHandler.text);
                return (true,  data);
            }
        }

        return default;  
    }
}

[System.Serializable]
public class Request
{
    public string SessionID => UserGlobalData.sessionID;
}

[System.Serializable]
public class Response
{
    public string status;
    public string customMessage;
}