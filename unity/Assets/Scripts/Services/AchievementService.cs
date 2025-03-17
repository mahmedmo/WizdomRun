using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class AchievementService : BaseService
{
    // Unlock (create) a new achievement.
    public IEnumerator UnlockAchievement(int campaignID, string title, string description, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/achievements/unlock";
        string jsonData = $"{{\"campaignID\":{campaignID},\"title\":\"{title}\",\"description\":\"{description}\"}}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get achievements for a campaign.
    public IEnumerator GetAchievements(int campaignID, string firebaseToken, System.Action<List<Achievement>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/achievements/{campaignID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

        yield return SendRequest(request,
            response =>
            {
                // Wrap response into an object to allow parsing a JSON array.
                AchievementListWrapper wrapper = JsonUtility.FromJson<AchievementListWrapper>("{\"achievements\":" + response + "}");
                onSuccess?.Invoke(wrapper.achievements);
            },
            error => { onError?.Invoke(error); }
        );
    }


    // Local classes for JSON parsing.
    [System.Serializable]
    public class Achievement
    {
        public int achievementID;
        public string title;
        public string description;
    }

    [System.Serializable]
    public class AchievementListWrapper
    {
        public List<Achievement> achievements;
    }
}