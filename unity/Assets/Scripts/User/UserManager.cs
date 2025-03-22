using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }

    private User currentUser;
    public User CurrentUser { get { return currentUser; } }

    private UserService userService;
    private CampaignService campaignService;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            userService = GetComponent<UserService>();

            campaignService = GetComponent<CampaignService>();

            LoadUser(AuthManager.Instance.UserId);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Calls an async function to gather user and campaign data
    public void LoadUser(string userId)
    {
        StartCoroutine(userService.GetUser(userId, AuthManager.Instance.AuthToken, OnUserLoaded, OnUserLoadError));
    }

    public void Logout()
    {
        currentUser = null;
        Destroy(gameObject);
    }

    private void OnUserLoaded(UserService.User userResponse)
    {
        if (userResponse == null) return;

        currentUser = new User()
        {
            UserID = userResponse.userID,
            ScreenName = userResponse.screenName,
            CampaignList = new List<Campaign>()
        };

        Debug.Log("User loaded: " + currentUser.ScreenName);
        Debug.Log("User loaded: " + currentUser.UserID);

        StartCoroutine(campaignService.GetCampaigns(currentUser.UserID, AuthManager.Instance.AuthToken, OnCampaignsLoaded, OnCampaignsError));

    }

    private void OnUserLoadError(string error)
    {
        Debug.LogError("Error loading user: " + error);
    }

    public event System.Action OnCampaignsLoadedEvent;

    private void OnCampaignsLoaded(List<CampaignService.Campaign> campaigns)
    {
        currentUser.CampaignList = new List<Campaign>();

        foreach (var camp in campaigns)
        {
            Campaign campaign = new Campaign()
            {
                CampaignID = camp.campaignID,
                LastUpdated = ConvertDateTime(camp.lastUpdated),
                UserID = camp.userID,
                Title = camp.title,
                CampaignLength = (CampaignLength)System.Enum.Parse(typeof(CampaignLength), camp.campaignLength, true),
                CurrLevel = camp.currLevel,
                RemainingTries = camp.remainingTries,
                AchievementList = new List<Achievement>(),
                QuestionList = new List<Question>()
            };

            currentUser.CampaignList.Add(campaign);
        }
        Debug.Log("Campaigns loaded: " + currentUser.CampaignList.Count);


        OnCampaignsLoadedEvent?.Invoke();
    }

    // Helper method to convert backend's date value to local time
    private string ConvertDateTime(string dateTime)
    {
        DateTime utcDateTime = DateTime.ParseExact(dateTime,
                                                   "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                                   CultureInfo.InvariantCulture,
                                                   DateTimeStyles.AssumeUniversal);

        // Convert to local timezone
        DateTime localDateTime = utcDateTime.ToLocalTime();

        // Force MM/DD/YYYY h:mm tt format (ensuring slashes and no leading zero in hours)
        string formattedDateTime = localDateTime.ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);

        return formattedDateTime;
    }
    private void OnCampaignsError(string error)
    {
        Debug.LogError("Error loading campaigns: " + error);
    }
}