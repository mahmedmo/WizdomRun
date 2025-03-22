using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CampaignService : BaseService
{
    public IEnumerator CreateCampaign(string userID, string title, string campaignLength, int currLevel, string firebaseToken,
        System.Action<Campaign> onSuccess, System.Action<string> onError)
    {
        bool requestSuccess = false;
        string lastError = "";

        // Loop till request is successful (expected to always pass eventually, due to lack of cloud reliability)
        while (!requestSuccess)
        {
            string url = $"{baseUrl}/campaigns/create";
            string jsonData = $"{{\"userID\":\"{userID}\",\"title\":\"{title}\",\"campaignLength\":\"{campaignLength.ToLower()}\",\"currLevel\":{currLevel}}}";
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

            bool done = false;
            yield return SendRequest(request,
                (response) =>
                {
                    Campaign campaign = JsonUtility.FromJson<Campaign>(response);
                    onSuccess?.Invoke(campaign);
                    requestSuccess = true;
                    done = true;
                },
                (error) =>
                {
                    lastError = error;
                    Debug.Log("Error in campaign creation:" + error);
                    done = true;
                }
            );

            yield return new WaitUntil(() => done);
            if (!requestSuccess)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        if (!requestSuccess)
        {
            onError?.Invoke(lastError);
        }
    }

    public IEnumerator GetCampaign(int campaignID, string firebaseToken,
        System.Action<Campaign> onSuccess, System.Action<string> onError)
    {
        bool requestSuccess = false;
        string lastError = "";

        while (!requestSuccess)
        {
            string url = $"{baseUrl}/campaigns/single/{campaignID}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

            bool done = false;
            yield return SendRequest(request,
                (response) =>
                {
                    Campaign campaign = JsonUtility.FromJson<Campaign>(response);
                    onSuccess?.Invoke(campaign);
                    requestSuccess = true;
                    done = true;
                },
                (error) =>
                {
                    lastError = error;
                    done = true;
                }
            );

            yield return new WaitUntil(() => done);
            if (!requestSuccess)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        if (!requestSuccess)
        {
            onError?.Invoke(lastError);
        }
    }

    public IEnumerator UpdateCampaign(int campaignID, int currLevel, int remainingTries, string firebaseToken,
        System.Action onSuccess, System.Action<string> onError)
    {
        bool requestSuccess = false;
        string lastError = "";

        while (!requestSuccess)
        {
            string url = $"{baseUrl}/campaigns/update/{campaignID}";
            string jsonData = $"{{\"currLevel\":{currLevel},\"remainingTries\":{remainingTries}}}";
            UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

            bool done = false;
            yield return SendRequest(request,
                (response) =>
                {
                    onSuccess?.Invoke();
                    requestSuccess = true;
                    done = true;
                },
                (error) =>
                {
                    lastError = error;
                    done = true;
                }
            );

            yield return new WaitUntil(() => done);
            if (!requestSuccess)
            {
                yield return new WaitForSeconds(1f);

            }
        }

        if (!requestSuccess)
        {
            onError?.Invoke(lastError);
        }
    }
    public IEnumerator GetCampaigns(string userID, string firebaseToken,
        System.Action<List<Campaign>> onSuccess, System.Action<string> onError)
    {
        bool requestSuccess = false;
        string lastError = "";

        while (!requestSuccess)
        {
            string url = $"{baseUrl}/campaigns/{userID}";
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

            bool done = false;
            yield return SendRequest(request,
                (response) =>
                {
                    CampaignListWrapper wrapper = JsonUtility.FromJson<CampaignListWrapper>("{\"campaigns\":" + response + "}");
                    onSuccess?.Invoke(wrapper.campaigns);
                    requestSuccess = true;
                    done = true;
                },
                (error) =>
                {
                    lastError = error;
                    done = true;
                }
            );

            yield return new WaitUntil(() => done);
            if (!requestSuccess)
            {
                yield return new WaitForSeconds(1f);

            }
        }

    }

    // Response Objects
    [System.Serializable]
    public class Campaign
    {
        public int campaignID;
        public string lastUpdated;
        public string userID;
        public string title;
        public string campaignLength;
        public int currLevel;
        public int remainingTries;
    }

    [System.Serializable]
    public class CampaignListWrapper
    {
        public List<Campaign> campaigns;
    }
}