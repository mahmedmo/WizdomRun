using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class UserService : BaseService
{
    // Get a single user.
    public IEnumerator GetUser(string userID, string firebaseToken, System.Action<User> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/users/{userID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                User user = JsonUtility.FromJson<User>(response);
                onSuccess?.Invoke(user);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Local classes for JSON parsing.
    [System.Serializable]
    public class User
    {
        public string userID;
        public string screenName;
        // CampaignList is not provided directly by these endpoints.
    }


}