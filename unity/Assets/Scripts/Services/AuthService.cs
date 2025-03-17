using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Firebase.Extensions;

[System.Serializable]
public class AuthResponse
{
    public string message; // Optional: for signup messages

    public string userID;
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
        string url = "https://wizdomrun.onrender.com/auth/signup";
        string jsonPayload = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\", \"screenName\":\"" + screenName + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string errorMsg = "Signup backend error: " + request.error;
            Debug.LogError(errorMsg);
            AuthManager.Instance.OnAuthError(errorMsg);
        }
        else
        {
            Debug.Log("Signup backend response: " + request.downloadHandler.text);
            // Parse the response to extract the userID.
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            // Since the user was created in the backend, we now log the user in to get a Firebase token.
            // We assume the email and password used for signup are valid for login.
            Login(email, password);
        }
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
                // Send the token to the backend to verify and retrieve the userID.
                StartCoroutine(SendTokenToBackend(idToken));
            });
        });
    }

    IEnumerator SendTokenToBackend(string idToken)
    {
        string url = "https://wizdomrun.onrender.com/auth/login";
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
            // Parse the response to extract the userID.
            AuthResponse response = JsonUtility.FromJson<AuthResponse>(request.downloadHandler.text);
            // Update AuthManager with the backend userID.
            AuthManager.Instance.OnAuthSuccess(response.userID, idToken);
        }
    }
}