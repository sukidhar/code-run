using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using Realms;
public class WebManager
{
    public bool isLoading = false;
    private static string baseUrl = "http://localhost:5000";
    private WebManager() {}
    private static WebManager instance;
    public static WebManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WebManager();
            }
            return instance;
        }
    }

    public IEnumerator RequestRegistration(string email,string password, string nickname, System.Action<GameUser,String> callback)
    {
        isLoading = true;
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("nickname", nickname);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/auth/register",form))
        {

            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isHttpError || request.isNetworkError)
            {
                callback(null, request.error);
            }
            else
            {
                GameUser gameUser = JsonConvert.DeserializeObject<GameUser>(request.downloadHandler.text);
                callback(gameUser, "");
            }
        }
    }

    public IEnumerator RequestLogin(string email,string password, System.Action<GameUser,string> callback)
    {
        isLoading = true;
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        using (UnityWebRequest request = UnityWebRequest.Post(baseUrl + "/auth/login",form))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isHttpError || request.isNetworkError)
            {
                callback(null, request.error);
            }
            else
            {
                GameUser gameUser = JsonConvert.DeserializeObject<GameUser>(request.downloadHandler.text);
                callback(gameUser, "");
            }
        }
    }

    public IEnumerator RequestLevelCoroutine(string level,System.Action<Level,string> callback)
    {
        isLoading = true;
        
        using(UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/" + level))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isNetworkError || request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                Level levelObj = JsonConvert.DeserializeObject<Level>(request.downloadHandler.text);
                callback(levelObj, "");
            }
        }
    }

    public IEnumerator RequestAllChapters(System.Action<List<Chapter>,string> callback)
    {
        isLoading = true;
        var token = GetApiKey();
        if (token.Length <= 0)
        {
            callback(null, "No API KEY Found");
            yield return "";
        }
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl+"/chapter/all/"+token))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                callback(null, request.error);
            }
            else
            {
                ChapterResult result = JsonConvert.DeserializeObject<ChapterResult>(request.downloadHandler.text);
                callback(result.chapters, "");
            }
        }
    }

    private string GetApiKey()
    {
        Realm realm = Realm.GetInstance();
        var users = realm.All<GameUser>();
        return users.Count() > 0 ? users.First().token : "";
    }

    public IEnumerator RequestCodeCompilation(string language,string code,System.Action<CompilationResult,string> callback)
    {
        isLoading = true;
        string url = "http://localhost:5000/" + language;
        WWWForm form = new WWWForm();
        form.AddField("code", code);
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();
            isLoading = false;
            if (request.isNetworkError || request.isHttpError)
                callback(null, request.error);
            else
            {
                Debug.Log(request.downloadHandler.text);
                CompilationResult result = JsonConvert.DeserializeObject<CompilationResult>(request.downloadHandler.text);
                callback(result, "");
            }
        }
    }
}
