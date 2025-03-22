using UnityEngine;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;

// Response Objects
[System.Serializable]
public class AuthResponse
{
    public string message;

    public string userID;
}

[System.Serializable]
public class ErrorResponse
{
    public string error;
}

public class AuthService : BaseService
{
    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void Signup(string email, string password, string screenName)
    {
        AuthManager.Instance.OnAuthAwait();
        StartCoroutine(SendSignupToBackend(email, password, screenName));
    }

    IEnumerator SendSignupToBackend(string email, string password, string screenName)
    {
        string url = $"{baseUrl}/auth/signup";
        string jsonPayload = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\", \"screenName\":\"" + screenName + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        Debug.Log("Raw JSON response: " + responseText);

        if (request.result != UnityWebRequest.Result.Success)
        {
            string errorMsg = "";
            if (!string.IsNullOrEmpty(responseText) && responseText.TrimStart().StartsWith("{"))
            {
                ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(responseText);
                errorMsg = errorResponse.error;
            }
            else
            {
                errorMsg = "Unknown error occured.";
            }
            AuthManager.Instance.OnAuthError(errorMsg);
        }
        else
        {
            if (string.IsNullOrEmpty(responseText) || !responseText.TrimStart().StartsWith("{"))
            {
                AuthManager.Instance.OnAuthError("Signup response is not valid JSON.");
            }
            else
            {
                AuthResponse response = JsonUtility.FromJson<AuthResponse>(responseText);
                Debug.Log("Parsed response message: " + response.message);
                Login(email, password);
            }
        }

        MainMenuManager.Instance?.OnLoadDone();
    }

    public void Login(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                string errorMsg = "Email or Password is incorrect.";
                AuthManager.Instance.OnAuthError(errorMsg);
                return;
            }

            FirebaseUser user = task.Result.User;
            Debug.Log($"User signed in: {user.Email}");

            user.TokenAsync(true)
                .ContinueWithOnMainThread(tokenTask =>
            {
                if (tokenTask.IsFaulted || tokenTask.IsCanceled)
                {
                    string errorMsg = "Email or Password is incorrect.";
                    AuthManager.Instance.OnAuthError(errorMsg);
                    return;
                }

                string idToken = tokenTask.Result;
                Debug.Log("ID token: " + idToken);
                AuthManager.Instance.OnAuthAwait();
                StartCoroutine(SendTokenToBackend(idToken));
            });
        });
    }

    IEnumerator SendTokenToBackend(string idToken)
    {
        string url = $"{baseUrl}/auth/login";
        string jsonPayload = "{\"idToken\":\"" + idToken + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string errorMsg = "Login backend error: " + request.error;
            Debug.LogError(errorMsg);
            AuthManager.Instance.OnAuthError(errorMsg);
        }
        else
        {
            Debug.Log("Login backend response: " + request.downloadHandler.text);
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            AuthManager.Instance.OnAuthSuccess(response.userID, idToken);
        }
    }
}