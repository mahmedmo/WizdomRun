using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// Base class to be inherited by all services.
public class BaseService : MonoBehaviour
{
    protected const string baseUrl = "https://wizdomrun.onrender.com";

    // Template Unity web request
    protected IEnumerator SendRequest(UnityWebRequest request, System.Action<string> onSuccess, System.Action<string> onError)
    {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            onError?.Invoke(request.error);
        else
            onSuccess?.Invoke(request.downloadHandler.text);
    }
}