using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Collections;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    private string userId;
    private string authToken;
    public string UserId { get { return userId; } }
    public string AuthToken { get { return authToken; } }
    private AuthService authService;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            userId = null;
            Firebase.FirebaseApp.LogLevel = Firebase.LogLevel.Error;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        authService = gameObject.GetComponent<AuthService>();
    }

    // Clears credentials
    public void Logout()
    {
        userId = null;
        authToken = null;
    }

    public void Signup(string name, string email, string password)
    {
        authService.Signup(email, password, name);
    }

    public void Login(string email, string password)
    {
        authService.Login(email, password);
    }

    // Starts loading screen during auth
    public void OnAuthAwait()
    {
        MainMenuManager.Instance?.OnLoadStart();
    }

    // AuthService success callback
    public void OnAuthSuccess(string id, string token)
    {
        userId = id;
        authToken = token;
        Debug.Log("AuthManager: User authenticated with ID: " + userId + " and token: " + authToken);
        StartCoroutine(LoadDelay(1.5f));
        MainMenuManager.Instance?.LoadUserMenu();
    }

    // async function that adds a buffer for a loading screen
    private IEnumerator LoadDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (MainMenuManager.Instance != null) MainMenuManager.Instance.OnLoadDone();
    }

    // AuthService error callback
    public void OnAuthError(string errorMsg)
    {
        Debug.LogError("AuthManager Error: " + errorMsg);
        StartCoroutine(LoadDelay(1.5f));
        MainMenuManager.Instance?.DisplayFirebaseError(errorMsg);
    }

}