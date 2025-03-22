using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class UserService : BaseService
{
    public IEnumerator GetUser(string userID, string firebaseToken, System.Action<User> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/users/{userID}";
        bool success = false;

        while (!success)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    User user = JsonUtility.FromJson<User>(request.downloadHandler.text);
                    onSuccess?.Invoke(user);
                    success = true;
                }
                else if (request.responseCode == 403)
                {
                    Debug.LogWarning("Received 403 Forbidden. Retrying...");
                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    onError?.Invoke("Error loading user: " + request.error);
                }
            }
        }
    }

    // Response Objects
    [System.Serializable]
    public class User
    {
        public string userID;
        public string screenName;
    }


}