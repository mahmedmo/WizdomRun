using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class CampaignService : BaseService
{
    // Create a new campaign.
    public IEnumerator CreateCampaign(string userID, string title, string campaignLength, int currLevel, string firebaseToken, System.Action<CampaignDBO> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/campaigns/create";
        string jsonData = $"{{\"userID\":\"{userID}\",\"title\":\"{title}\",\"campaignLength\":\"{campaignLength.ToString().ToLower()}\",\"currLevel\":{currLevel}}}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

        yield return SendRequest(request,
            response =>
            {
                CampaignDBO campaign = JsonUtility.FromJson<CampaignDBO>(response);
                onSuccess?.Invoke(campaign);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Get all campaigns for a user.
    public IEnumerator GetCampaigns(string userID, string firebaseToken, System.Action<List<CampaignDBO>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/campaigns/{userID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

        yield return SendRequest(request,
            response =>
            {
                CampaignListWrapper wrapper = JsonUtility.FromJson<CampaignListWrapper>("{\"campaigns\":" + response + "}");
                onSuccess?.Invoke(wrapper.campaigns);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Update a campaign's currLevel and remainingTries.
    public IEnumerator UpdateCampaign(int campaignID, int currLevel, int remainingTries, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/campaigns/update/{campaignID}";
        string jsonData = $"{{\"currLevel\":{currLevel},\"remainingTries\":{remainingTries}}}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Delete a campaign.
    public IEnumerator DeleteCampaign(int campaignID, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/campaigns/delete/{campaignID}";
        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get a single campaign.
    public IEnumerator GetCampaign(int campaignID, string firebaseToken, System.Action<CampaignDBO> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/campaigns/single/{campaignID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                CampaignDBO campaign = JsonUtility.FromJson<CampaignDBO>(response);
                onSuccess?.Invoke(campaign);
            },
            error => { onError?.Invoke(error); }
        );
    }


    // Local classes for JSON parsing.
    [System.Serializable]
    public class CampaignDBO
    {
        public int campaignID;
        public string lastUpdated;
        public int userID;
        public string title;
        public string campaignLength;
        public int currLevel;
        public int remainingTries;
        // Note: AchievementList and QuestionList are not returned by these endpoints.
    }

    [System.Serializable]
    public class CampaignListWrapper
    {
        public List<CampaignDBO> campaigns;
    }
}