using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }

    // The currently logged in user.
    private User currentUser;
    public User CurrentUser { get { return currentUser; } }

    // Services for user and campaign endpoints.
    private UserService userService;
    private CampaignService campaignService;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Get or add the UserService component.
            userService = GetComponent<UserService>();

            campaignService = GetComponent<CampaignService>();

            LoadUser(AuthManager.Instance.UserId);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// Loads the user data and then loads their campaigns.
    /// </summary>
    /// <param name="userID">The user's ID</param>
    public void LoadUser(string userId)
    {
        StartCoroutine(userService.GetUser(userId, AuthManager.Instance.AuthToken, OnUserLoaded, OnUserLoadError));
    }
    public void Logout()
    {
        currentUser = null;
        Destroy(gameObject);
    }

    // Called when the user is successfully loaded.
    private void OnUserLoaded(UserService.User userResponse)
    {
        if (userResponse != null)
        {
            // Populate our User object.
            currentUser = new User()
            {
                UserID = userResponse.userID,
                ScreenName = userResponse.screenName,
                CampaignList = new List<Campaign>()
            };

            Debug.Log("User loaded: " + currentUser.ScreenName);
            Debug.Log("User loaded: " + currentUser.UserID);
            // Now load the user's campaigns.
            StartCoroutine(campaignService.GetCampaigns(currentUser.UserID, AuthManager.Instance.AuthToken, OnCampaignsLoaded, OnCampaignsError));
        }
        else
        {
            Debug.LogError("User response was null.");
        }
    }

    private void OnUserLoadError(string error)
    {
        Debug.LogError("Error loading user: " + error);
    }

    // Called when the campaigns are loaded.
    private void OnCampaignsLoaded(List<CampaignService.CampaignDBO> campaigns)
    {
        currentUser.CampaignList = new List<Campaign>();

        // Convert each campaign from the service response into our UserManager Campaign type.
        foreach (var camp in campaigns)
        {
            Campaign campaign = new Campaign()
            {
                CampaignID = camp.campaignID,
                LastUpdated = camp.lastUpdated,
                UserID = camp.userID,
                Title = camp.title,
                // Convert the string campaignLength (from the API) into our CampaignLength enum.
                CampaignLength = (CampaignLength)System.Enum.Parse(typeof(CampaignLength), camp.campaignLength, true),
                CurrLevel = camp.currLevel,
                RemainingTries = camp.remainingTries,
                AchievementList = new List<Achievement>(),
                QuestionList = new List<Question>()
            };

            currentUser.CampaignList.Add(campaign);
        }
        Debug.Log("Campaigns loaded: " + currentUser.CampaignList.Count);
    }

    private void OnCampaignsError(string error)
    {
        Debug.LogError("Error loading campaigns: " + error);
    }
}