using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance { get; private set; }
    private CampaignService campaignService;
    public Campaign currCampaign;  // Now using the global Campaign class

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            campaignService = GetComponent<CampaignService>();
            if (campaignService == null)
            {
                Debug.LogError("CampaignService component not found on CampaignManager object!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Creates a new campaign by calling the backend service.
    /// </summary>
    public IEnumerator CreateNewCampaign(string userID, string title, string campaignLengthStr, int currLevel, string firebaseToken)
    {
        yield return campaignService.CreateCampaign(
            userID,
            title,
            campaignLengthStr,
            currLevel,
            firebaseToken,
            (CampaignService.CampaignDBO csCampaign) =>
            {
                // Convert the service campaign to the global campaign.
                currCampaign = CampaignAdapter(csCampaign);
                Debug.Log("Campaign created successfully: " + currCampaign.CampaignID);
                // Optionally, load the campaign scene or perform further initialization here.
            },
            (string error) =>
            {
                Debug.LogError("Error creating campaign: " + error);
                // Optionally, notify the UI about the error.
            }
        );
    }
    private Campaign CampaignAdapter(CampaignService.CampaignDBO csCampaign)
    {
        Campaign campaign = new Campaign();
        campaign.CampaignID = csCampaign.campaignID;
        campaign.LastUpdated = csCampaign.lastUpdated;
        campaign.UserID = csCampaign.userID;
        campaign.Title = csCampaign.title;
        campaign.CampaignLength = (CampaignLength)System.Enum.Parse(typeof(CampaignLength), csCampaign.campaignLength, true);
        campaign.CurrLevel = csCampaign.currLevel;
        campaign.RemainingTries = csCampaign.remainingTries;
        campaign.AchievementList = new List<Achievement>();
        campaign.QuestionList = new List<Question>();
        return campaign;
    }
}