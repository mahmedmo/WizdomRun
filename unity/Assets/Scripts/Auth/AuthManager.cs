using UnityEngine;
using Firebase;
using Firebase.Auth;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    private string userId;
    private string authToken;

    public string UserId { get { return userId; } }
    public string AuthToken { get { return authToken; } } // Fixed getter

    private AuthService authService;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            userId = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Logout()
    {
        userId = null;
        authToken = null;
    }
    void Start()
    {
        authService = gameObject.GetComponent<AuthService>();
        if (authService == null)
        {
            Debug.LogError("AuthService not found in the scene!");
        }
    }

    public void Signup(string name, string email, string password)
    {
        Debug.Log("AuthManager: Initiating signup for " + email);
        authService.Signup(email, password, name);
    }

    public void Login(string email, string password)
    {
        Debug.Log("AuthManager: Initiating login for " + email);
        authService.Login(email, password);
    }
    public void OnAuthAwait()
    {
        MainMenuManager.Instance?.OnLoadStart();
    }
    // Called on successful authentication with the definitive userID from the backend.
    public void OnAuthSuccess(string id, string token)
    {
        userId = id;
        authToken = token;
        Debug.Log("AuthManager: User authenticated with ID: " + userId + " and token: " + authToken);
        MainMenuManager.Instance?.OnLoadDone();
        MainMenuManager.Instance?.LoadUserMenu();
    }

    // Called when any error occurs.
    public void OnAuthError(string errorMsg)
    {
        MainMenuManager.Instance?.OnLoadDone();
        Debug.LogError("AuthManager Error: " + errorMsg);
        MainMenuManager.Instance?.DisplayFirebaseError(errorMsg);
    }

    public string GetUserId()
    {
        return userId;
    }
}