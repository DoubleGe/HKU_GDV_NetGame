using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NetGame.Server
{
    internal static class RequestManager
    {
        public const string DEFAULT_URL = "https://api.wesleydegraaf.com/GDVNet/";
        public static async Task<O> SendRequestAsync<T, O>(string url, T requestData) where O : class
        {
            WWWForm form = new WWWForm();
            form.AddField("request", JsonUtility.ToJson(requestData));
            Debug.Log(JsonUtility.ToJson(requestData));
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
                    return data;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    internal class Request
    {
        public string SessionID;

        public Request() => SessionID = ServerGlobalData.sessionID;
    }

    [System.Serializable]
    internal class Response
    {
        public string status;
        public string customMessage;
    }
}